using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fildela.Resources;
using Fildela.Web.Helpers;
using System.Security.Claims;
using System.Net;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Fildela.Web.Models.UserModels;
using Fildela.Business.Domains.User;
using Fildela.Business.Domains.Administration;
using Fildela.Business.Domains.Account;
using Fildela.Business.Domains.Account.Models;
using Fildela.Business.Domains.Administration.Models;
using Fildela.Business.Domains.User.Models;
using Fildela.Business.Domains.News.Models;
using Fildela.Business.Domains.News;
using Fildela.Web.Models.AccountModels;
using Fildela.Web.Models;
using Microsoft.ApplicationServer.Caching;
using System.Configuration;
using Facebook;

namespace Fildela.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];

        private readonly IUserService _userService;
        private readonly IAdministrationService _administrationService;
        private readonly IAccountService _accountService;
        private readonly INewsService _newsService;

        public UserController(IUserService userService, IAdministrationService administrationService, IAccountService accountService, INewsService newsService)
        {
            _userService = userService;
            _administrationService = administrationService;
            _accountService = accountService;
            _newsService = newsService;
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [HttpPost]
        [AjaxOnly]
        [AllowAnonymous]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public JsonResult Register(RegisterViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                string ipAddress = IpAddressExtensions.GetIpAddress();

                //Validate ipaddress
                bool adminLogExist = _administrationService.AdminLogExist(ipAddress, "Register");
                if (!adminLogExist)
                {
                    //Validate if user registerverification already exist and is active
                    bool registerVerificationExist = _userService.RegisterVerificationExist(model.Email);
                    if (!registerVerificationExist)
                    {
                        //Validate if user already exist
                        bool userExist = _userService.UserExist(model.Email);
                        if (!userExist)
                        {
                            SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();
                            string passwordHash = crypto.Compute(model.Password);

                            string key = GuidExtensions.DateAndGuid();

                            RegisterVerificationModel registerVerification = new RegisterVerificationModel(
                                 model.Email.ToLower().Trim(), key, string.Empty, string.Empty,
                                 passwordHash, crypto.Salt, model.AgreeUserAgreement, ipAddress);

                            //Insert register verification
                            _userService.InsertRegisterVerification(registerVerification);

                            //Send register verification email
                            _userService.SendEmailRegisterVerification(registerVerification);

                            string rowKey = "registerverification-" + ipAddress.Replace(".", "") + "-" + key;

                            AdminLogModel adminLog = new AdminLogModel(
                            rowKey, 0, model.Email.ToLower().Trim(), 0, model.Email.ToLower().Trim(), ipAddress,
                            "Register");

                            //Insert admin log
                            _administrationService.InsertAdminLog(adminLog);

                            LogModel accountLog = new LogModel(
                                rowKey, 0, model.Email.ToLower().Trim(), 0, model.Email.ToLower().Trim(), Resource.User,
                                ipAddress, Resource.Register_account_started);

                            //Insert account log
                            _accountService.InsertAccountLog(accountLog);

                            message = Resource.An_activation_email_has_been_sent_to_your_email;
                            success = true;
                        }
                        else
                            message = Resource.Email_is_already_in_use;
                    }
                    else
                        message = Resource.An_activation_email_has_already_been_sent_to_your_email;
                }
                else
                    message = Resource.You_have_already_registered_an_account_try_again_later;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [ValidateInput(true)]
        public ViewResult RegisterVerification(RegisterVerificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                string ipAddress = IpAddressExtensions.GetIpAddress();

                //Get register verification
                RegisterVerificationModel registerVerification = _userService.GetRegisterVerification(model.Email, model.Key);
                if (registerVerification != null)
                {
                    //Validate expirytime on register verification
                    DateTime currentTime = TimeZoneExtensions.GetCurrentDate();
                    if ((registerVerification.DateExpires - currentTime).TotalDays > 0)
                    {
                        //Validate if user already exist
                        bool userExist = _userService.UserExist(model.Email);
                        if (!userExist)
                        {
                            UserModel newUser = new UserModel()
                            {
                                Email = registerVerification.RowKey.ToLower().Trim(),
                                PasswordHash = registerVerification.Password,
                                PasswordSalt = registerVerification.PasswordSalt,
                                FirstName = registerVerification.FirstName,
                                LastName = registerVerification.LastName,
                                DateRegistered = registerVerification.DateRegistered,
                                IsDeleted = registerVerification.IsDeleted,
                                AgreeUserAgreement = registerVerification.AgreeUserAgreement,
                                DefaultIpAddress = registerVerification.DefaultIpAddress,
                                DefaultEmailAddress = registerVerification.RowKey,
                                DateLastActive = registerVerification.DateRegistered
                            };

                            //Insert user
                            int userID = _userService.InsertUser(newUser);

                            RoleModel role = _userService.GetRole("AccountOwner");
                            if (role != null)
                                _userService.InsertUserRole(role, userID);

                            //Delete register verification
                            _userService.DeleteRegisterVerification(model.Email);

                            //Send confirmation email
                            _userService.SendEmailRegisterSuccess(model.Email);

                            string rowKey = "registerverification-" + userID + "-" + GuidExtensions.DateAndGuid();

                            LogModel accountLog = new LogModel(
                                rowKey, userID, newUser.Email.ToLower().Trim(), userID, newUser.Email.ToLower().Trim(), Resource.User,
                                ipAddress, Resource.Register_account_complete);

                            //Insert account log
                            _accountService.InsertAccountLog(accountLog);

                            ModelState.AddModelError("Success", Resource.Your_user_account_has_been_activated + ".");
                        }
                        else
                            ModelState.AddModelError("Error", Resource.Email_is_already_in_use);
                    }
                    else
                        ModelState.AddModelError("Error", Resource.The_key_to_activate_your_account_has_expired_please_register_again);
                }
                else
                    ModelState.AddModelError("Error", Resource.The_key_to_activate_your_account_was_not_found_please_register_again);
            }
            else
                ModelState.AddModelError("Error", Resource.An_unexpected_error_has_occurred_Please_try_again);

            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Registration;
            ViewBag.MessageTitle = Resource.Registration;

            return View("~/Views/Shared/Message.cshtml");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SignInRedirect()
        {
            NewsDTOModel newsDTO = _newsService.GetLatestNewsFromCacheOrDB();

            return View("~/Views/User/SignIn.cshtml", newsDTO);
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [AjaxOnly]
        [AllowAnonymous]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public JsonResult SignInUser(SignInViewModel model, string returnUrl)
        {
            CookieExtensions.createCookie("SignInUserType", 7, "AccountOwner", true);

            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                bool isPasswordValid = _userService.IsUserPasswordValid(model.Email, model.Password);

                if (isPasswordValid)
                {
                    SignInUserDTOModel signInModel = _userService.GetSignInUser(model.Email);

                    if (!signInModel.IsDeleted)
                    {
                        SignInUser(signInModel, model.Email);

                        success = true;
                    }
                    else
                        message = Resource.The_user_account_is_disabled_please +
                            " <a class='sign-in-account-disabled-link' href='/Contact/Index''>" +
                            Resource.Contact2 + "</a> " + Resource.Support2;
                }
                else
                    message = Resource.Incorrect_username_or_password;

            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                returnUrl = String.IsNullOrEmpty(returnUrl) ? null : returnUrl,
                success = success
            });
        }

        [HttpGet]
        [AjaxOnly]
        [AllowAnonymous]
        public ActionResult GetAccountLinksForGuest(SignInViewModel model, string returnUrl)
        {
            CookieExtensions.createCookie("SignInUserType", 7, "Guest", true);

            string message = string.Empty;
            bool success = false;
            string viewString = string.Empty;

            if (ModelState.IsValid)
            {
                bool isPasswordValid = _userService.IsGuestPasswordValid(model.Email, model.Password);

                if (isPasswordValid)
                {
                    SignInAccountLinksDTOModel signInModel = _userService.GetSignInAccountLinks(model.Email);

                    if (!signInModel.IsDeleted)
                    {
                        if (signInModel.Users != null && signInModel.Users.Count() > 0)
                        {
                            SignInGuestAccountLinksViewModel accountLinks = new SignInGuestAccountLinksViewModel();
                            List<UserSignInGuestAccountLinksModel> accountLinkListTemp = new List<UserSignInGuestAccountLinksModel>();

                            foreach (var item in signInModel.Users)
                            {
                                UserSignInGuestAccountLinksModel accountlinkTemp = new UserSignInGuestAccountLinksModel()
                                {
                                    AccountOwnerID = item.UserID,
                                    AccountOwnerEmail = item.Email
                                };

                                if (!String.IsNullOrEmpty(returnUrl))
                                    accountLinks.ReturnURL = returnUrl;

                                accountLinkListTemp.Add(accountlinkTemp);
                            }

                            accountLinks.AccountLinks = accountLinkListTemp;
                            accountLinks.Email = model.Email;
                            accountLinks.Password = model.Password;

                            viewString = RenderViewToStringExtensions.RenderViewToString(this, "~/Views/Shared/_SignInModalsAccountLinksPartial.cshtml", accountLinks);
                            message = Resource.Login_succeeded_Please_select_an_account_to_connect_to;
                            success = true;
                        }
                        else
                            message = Resource.The_guest_account_is_not_linked_with_any_user;
                    }
                    else
                        message = Resource.The_user_account_is_disabled_please +
                            " <a class='sign-in-account-disabled-link' href='/Contact/Index''>" +
                            Resource.Contact2 + "</a> " + Resource.Support2;
                }
                else
                    message = Resource.Incorrect_username_or_password;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                returnUrl = returnUrl,
                success = success,
                viewString = viewString
            }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public JsonResult SignInGuest(UserSignInGuestAccountLinksModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                bool isPasswordValid = _userService.IsGuestPasswordValid(model.Email, model.Password);

                if (isPasswordValid)
                {
                    SignInGuestDTOModel signInModel = _userService.GetSignInGuest(model.Email, model.AccountOwnerID);

                    if (!signInModel.IsDeleted)
                    {
                        if (signInModel.IsGuestLinkedWithUser)
                        {
                            string accountOwnerEmail = _userService.GetUserEmail(model.AccountOwnerID);

                            List<Claim> claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, model.Email.ToLower().Trim()),
                                new Claim(ClaimTypes.Name, model.Email.ToLower().Trim()),
                                new Claim(ClaimTypes.Sid, signInModel.GuestID.ToString()),
                                new Claim("AccountOwnerEmail", accountOwnerEmail),
                                new Claim("AccountOwnerID", model.AccountOwnerID.ToString()),
                                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity")
                            };

                            //Add claim for guest role
                            Claim claimsTemp = new Claim(ClaimTypes.Role, "Guest");
                            claims.Add(claimsTemp);

                            ClaimsIdentity identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                            AuthenticationManager.SignIn(identity);

                            string ipAddress = IpAddressExtensions.GetIpAddress();
                            string rowKeyLog = "signin-" + signInModel.GuestID + "-" + GuidExtensions.DateAndGuid();

                            LogModel accountLog = new LogModel(
                                rowKeyLog, model.AccountOwnerID, accountOwnerEmail.ToLower().Trim(), signInModel.GuestID, model.Email.ToLower().Trim(), Resource.Guest,
                                ipAddress, Resource.Login_by_guest);

                            //Insert account log
                            _accountService.InsertAccountLog(accountLog);

                            string rowKeyUsage = "signin-" + model.AccountOwnerID + "-" + GuidExtensions.DateAndGuid();

                            UsageModel usage = new UsageModel(rowKeyUsage, accountOwnerEmail.ToLower().Trim(), "SignIn");

                            //Insert usage
                            _accountService.InsertUsage(usage);

                            success = true;
                        }
                        else
                            message = Resource.Your_useraccount_is_not_linked_with_this_user;
                    }
                    else
                        message = Resource.The_user_account_is_disabled_please +
                            " <a class='sign-in-account-disabled-link' href='/Contact/Index''>" +
                            Resource.Contact2 + "</a> " + Resource.Support2;
                }
                else
                    message = Resource.Incorrect_username_or_password;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                returnUrl = String.IsNullOrEmpty(model.ReturnURL) ? null : model.ReturnURL,
                success = success
            });
        }

        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult SignOut()
        {
            AuthenticationManager.SignOut();

            TempData["IndexMessage"] = @"<i class=""fa fa-sign-out popup-icon""></i>" + Resource.You_have_been_logged_out;

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("SignOut")]
        public RedirectToRouteResult SignOutGet()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AjaxOnly]
        [AllowAnonymous]
        [ValidateInput(true)]
        public JsonResult ResetPasswordUser(ResetPasswordViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                string ipAddress = IpAddressExtensions.GetIpAddress();

                //Validate ipaddress
                bool adminLogExist = _administrationService.AdminLogExist(ipAddress, "ResetPasswordUser");
                if (!adminLogExist)
                {
                    //Validate if reset password already exist
                    bool resetPasswordExist = _userService.ResetPasswordExist(model.Email, true);
                    if (!resetPasswordExist)
                    {
                        bool IsDeleted = _userService.IsUserDeleted(model.Email);

                        if (!IsDeleted)
                        {
                            //Validate if user exists
                            bool userExist = _userService.UserExistAndActive(model.Email);
                            if (userExist)
                            {
                                string resetPasswordRowKey = "user-" + ipAddress.Replace(".", "") + "-" + GuidExtensions.DateAndGuid();

                                ResetPasswordModel resetPassword = new ResetPasswordModel(resetPasswordRowKey, model.Email.ToLower().Trim(), GuidExtensions.DateAndGuid(), true);

                                //Insert reset password
                                _userService.InsertResetPassword(resetPassword);

                                //Send reset password email
                                _userService.SendEmailResetPassword(resetPassword);

                                //Get userID
                                int userID = _userService.GetUserID(model.Email);

                                string logRowKey = "resetpassword-" + ipAddress.Replace(".", "") + "-" + GuidExtensions.DateAndGuid();

                                AdminLogModel adminLog = new AdminLogModel(
                                    logRowKey, userID, model.Email.ToLower().Trim(), 0,
                                    string.Empty, ipAddress, "ResetPasswordUser");

                                //Insert admin log
                                _administrationService.InsertAdminLog(adminLog);

                                //Insert account log
                                LogModel accountLog = new LogModel(
                                    logRowKey, userID, model.Email.ToLower().Trim(), 0, String.Empty, Resource.User,
                                    ipAddress, Resource.Password_recovery_started);

                                message = Resource.Instructions_to_reset_your_password_has_been_sent_to_your_email;
                                success = true;
                            }
                            else
                                message = Resource.No_user_with_that_email_address_was_found;
                        }
                        else
                            message = Resource.The_user_account_is_disabled_please +
                            " <a class='sign-in-account-disabled-link' href='/Contact/Index''>" +
                            Resource.Contact2 + "</a> " + Resource.Support2;

                    }
                    else
                        message = Resource.You_have_already_reset_your_password_try_again_later;
                }
                else
                    message = Resource.You_have_already_reset_one_password_try_again_later;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [ValidateInput(true)]
        public ViewResult ResetPasswordVerificationUser(ResetPasswordVerificationModel model)
        {
            ViewBag.Title = Resource.Password_reset;

            if (ModelState.IsValid)
            {
                //Get reset password
                ResetPasswordModel getResetPassword = _userService.GetResetPassword(model.RowKey, model.Key, true);
                if (getResetPassword != null)
                {
                    //Validate expirytime on register verification
                    DateTime currentTime = TimeZoneExtensions.GetCurrentDate();
                    if ((getResetPassword.DateExpires - currentTime).TotalDays > 0)
                    {
                        bool IsDeleted = _userService.IsUserDeleted(model.RowKey);

                        if (!IsDeleted)
                        {
                            //Rowkey = email
                            string newPassword = _userService.ResetPasswordUser(getResetPassword.Email);

                            //Send email with new password
                            _userService.SendEmailResetPasswordNewPassword(getResetPassword.Email, newPassword, true);

                            //Delete reset password
                            _userService.DeleteResetPassword(getResetPassword.RowKey, true);

                            //Get userID
                            int userID = _userService.GetUserID(model.RowKey);

                            string rowKey = "resetpasswordverification-" + userID + "-" + GuidExtensions.DateAndGuid();
                            string ipAddress = IpAddressExtensions.GetIpAddress();

                            //Insert account log
                            LogModel accountLog = new LogModel(
                                rowKey, userID, model.RowKey.ToLower().Trim(), 0, String.Empty, Resource.User,
                                ipAddress, Resource.Password_recovery_complete);

                            ModelState.AddModelError("Success", Resource.An_email_with_a_new_password_has_been_sent_to_your_email_address);
                        }
                        else
                            ModelState.AddModelError("Error", Resource.The_user_account_is_disabled_please +
                            " <a class='sign-in-account-disabled-link' href='/Contact/Index''>" +
                            Resource.Contact2 + "</a> " + Resource.Support2);
                    }
                    else
                        ModelState.AddModelError("Error", Resource.The_key_to_reset_your_password_has_expired);
                }
                else
                    ModelState.AddModelError("Error", Resource.The_key_to_reset_your_password_was_not_found_Please_reset_again);
            }
            else
                ModelState.AddModelError("Error", Resource.An_unexpected_error_has_occurred_Please_try_again);

            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Password_recovery;
            ViewBag.MessageTitle = Resource.Password_recovery;

            return View("~/Views/Shared/Message.cshtml");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPasswordVerification()
        {
            return View();
        }

        [HttpPost]
        [AjaxOnly]
        [AllowAnonymous]
        [ValidateInput(true)]
        public JsonResult ResetPasswordGuest(ResetPasswordViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                string ipAddress = IpAddressExtensions.GetIpAddress();

                //Validate ipaddress
                bool adminLogExist = _administrationService.AdminLogExist(ipAddress, "ResetPasswordGuest");
                if (!adminLogExist)
                {
                    //Validate if reset password already exist
                    bool resetPasswordExist = _userService.ResetPasswordExist(model.Email, false);
                    if (!resetPasswordExist)
                    {
                        bool IsDeleted = _userService.IsGuestDeleted(model.Email);

                        if (!IsDeleted)
                        {
                            //Validate if user exists
                            bool userExistGuest = _userService.GuestExistAndActive(model.Email);
                            if (userExistGuest)
                            {
                                string resetPasswordRowKey = "guest-" + ipAddress.Replace(".", "") + "-" + GuidExtensions.DateAndGuid();

                                ResetPasswordModel resetPassword = new ResetPasswordModel(resetPasswordRowKey, model.Email.ToLower().Trim(), GuidExtensions.DateAndGuid(), false);

                                //Insert reset password
                                _userService.InsertResetPassword(resetPassword);

                                //Send reset password email
                                _userService.SendEmailResetPassword(resetPassword);

                                //Get userID
                                int userID = _userService.GetGuestID(model.Email);

                                string adminLogRowKey = "resetpassword-" + ipAddress.Replace(".", "") + "-" + GuidExtensions.DateAndGuid();

                                AdminLogModel adminLog = new AdminLogModel(
                                    adminLogRowKey, userID, model.Email.ToLower().Trim(), 0,
                                    string.Empty, ipAddress, "ResetPasswordGuest");

                                //Insert admin log
                                _administrationService.InsertAdminLog(adminLog);

                                message = Resource.Instructions_to_reset_your_password_has_been_sent;
                                success = true;
                            }
                            else
                                message = Resource.No_user_with_that_email_address_was_found;
                        }
                        else
                            message = Resource.The_user_account_is_disabled_please +
                            " <a class='sign-in-account-disabled-link' href='/Contact/Index''>" +
                            Resource.Contact2 + "</a> " + Resource.Support2;
                    }
                    else
                        message = Resource.You_have_already_reset_your_password_try_again_later;
                }
                else
                    message = Resource.You_have_already_reset_one_password_try_again_later;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [ValidateInput(true)]
        public ViewResult ResetPasswordVerificationGuest(ResetPasswordVerificationModel model)
        {
            ViewBag.Title = Resource.Password_reset;

            if (ModelState.IsValid)
            {
                string ipAddress = IpAddressExtensions.GetIpAddress();

                //Get reset password
                ResetPasswordModel getResetPassword = _userService.GetResetPassword(model.RowKey, model.Key, false);
                if (getResetPassword != null)
                {
                    //Validate expirytime on register verification
                    DateTime currentTime = TimeZoneExtensions.GetCurrentDate();
                    if ((getResetPassword.DateExpires - currentTime).TotalDays > 0)
                    {
                        bool IsDeleted = _userService.IsGuestDeleted(model.RowKey);

                        if (!IsDeleted)
                        {
                            //Rowkey = email
                            string newPassword = _userService.ResetPasswordGuest(getResetPassword.Email);

                            //Send email with new password
                            _userService.SendEmailResetPasswordNewPassword(getResetPassword.Email, newPassword, false);

                            //Delete reset password
                            _userService.DeleteResetPassword(getResetPassword.RowKey, false);

                            ModelState.AddModelError("Success", Resource.An_email_with_a_new_password_has_been_sent_to_your_email_address);
                        }
                        else
                            ModelState.AddModelError("Error", Resource.The_user_account_is_disabled_please +
                            " <a class='sign-in-account-disabled-link' href='/Contact/Index''>" +
                            Resource.Contact2 + "</a> " + Resource.Support2);
                    }
                    else
                        ModelState.AddModelError("Error", Resource.The_key_to_reset_your_password_has_expired);
                }
                else
                    ModelState.AddModelError("Error", Resource.The_key_to_reset_your_password_was_not_found_Please_reset_again);
            }
            else
                ModelState.AddModelError("Error", Resource.An_unexpected_error_has_occurred_Please_try_again);

            ViewBag.MessageTitle = Resource.Password_recovery;
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Password_recovery;

            return View("~/Views/Shared/Message.cshtml");
        }

        [HttpGet]
        [AllowAnonymous]
        public RedirectToRouteResult UpdateUserEmail()
        {
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeNotClaimAttribute(ClaimTypes.Role, "Guest")]
        public ActionResult UpdateUserEmail(UpdateEmailViewModel model)
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.My_account;

            if (ModelState.IsValid)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                //Validate ipaddress
                bool adminLogExist = _administrationService.AdminLogExist(currentUser.IpAddress, "UpdateUserEmail");
                if (!adminLogExist)
                {
                    bool isUserPasswordValid = _userService.IsUserPasswordValid(User.Identity.Name, model.Password);

                    //Validate password
                    if (isUserPasswordValid)
                    {
                        bool userExistAccountOwner = _userService.UserExist(model.NewEmail);

                        //Validate if user already exist
                        if (!userExistAccountOwner)
                        {
                            _userService.UpdateUserEmail(currentUser.AccountID, model.NewEmail);

                            string rowKey = "updateemail-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                            AdminLogModel adminLog = new AdminLogModel(
                            rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID,
                            currentUser.AccountEmail, currentUser.IpAddress, "UpdateUserEmail");

                            //Insert admin log
                            _administrationService.InsertAdminLog(adminLog);

                            LogModel accountLog = new LogModel(
                                rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                                currentUser.IpAddress, Resource.Update_email_complete);

                            //Insert account log
                            _accountService.InsertAccountLog(accountLog);

                            var authenticationManager = Request.GetOwinContext().Authentication;

                            //Update username
                            ClaimsIdentity claims = (ClaimsIdentity)User.Identity;
                            claims.RemoveClaim(claims.FindFirst(ClaimTypes.NameIdentifier));
                            claims.RemoveClaim(claims.FindFirst(ClaimTypes.Name));
                            claims.AddClaims(new List<Claim>
                                {
                                    new Claim(ClaimTypes.NameIdentifier, model.NewEmail.ToLower().Trim()),
                                    new Claim(ClaimTypes.Name, model.NewEmail.ToLower().Trim())
                                });

                            ClaimsIdentity newClaimsIdentity = new ClaimsIdentity(claims.Claims, DefaultAuthenticationTypes.ApplicationCookie);
                            authenticationManager.SignIn(newClaimsIdentity);

                            //Update last active
                            _userService.UpdateUserLastActive(currentUser.AccountID);

                            TempData["AccountSettingsSuccess"] = Resource.Your_emailaddress_has_been_updated;

                            return RedirectToAction("Settings", "Account", new { Tab = 2 });
                        }
                        else
                            ModelState.AddModelError("AccountSettingsError", Resource.Email_is_already_in_use);
                    }
                    else
                        ModelState.AddModelError("AccountSettingsError", Resource.Incorrect_password);

                }
                else
                    ModelState.AddModelError("AccountSettingsError", Resource.You_have_already_updated_your_emailaddress_try_again_later);
            }
            else
                ModelState.AddModelError("AccountSettingsError", Resource.An_unexpected_error_has_occurred_Please_try_again);

            UsageDTOModel accountUsage = _accountService.GetAccountUsageFromCacheOrDB(User.Identity.Name);

            SettingsViewModel accountViewModelSettings = new SettingsViewModel()
            {
                Tab = 2,
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                LogCount = accountUsage.LogCount
            };

            return View("~/Views/Account/Settings.cshtml", accountViewModelSettings);
        }

        [HttpGet]
        [AllowAnonymous]
        public RedirectToRouteResult UpdateGuestEmail()
        {
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim(ClaimTypes.Role, "Guest")]
        public ActionResult UpdateGuestEmail(UpdateEmailViewModel model)
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.My_account;

            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            if (ModelState.IsValid)
            {
                //Validate ipaddress
                bool adminLogExist = _administrationService.AdminLogExist(currentUser.IpAddress, "UpdateGuestEmail");
                if (!adminLogExist)
                {
                    bool isPasswordValidGuest = _userService.IsGuestPasswordValid(User.Identity.Name, model.Password);

                    //Validate password
                    if (isPasswordValidGuest)
                    {
                        bool userExistGuest = _userService.GuestExist(model.NewEmail);

                        //Validate if user already exist
                        if (!userExistGuest)
                        {
                            _userService.UpdateGuestEmail(currentUser.AccountID, model.NewEmail);

                            string rowKey = "updateemail-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                            AdminLogModel adminLog = new AdminLogModel(
                            rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID,
                            currentUser.AccountEmail, currentUser.IpAddress, "UpdateGuestEmail");

                            //Insert admin log
                            _administrationService.InsertAdminLog(adminLog);

                            LogModel accountLog = new LogModel(
                                rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                                currentUser.IpAddress, Resource.Update_email_complete);

                            //Insert account log
                            _accountService.InsertAccountLog(accountLog);

                            var authenticationManager = Request.GetOwinContext().Authentication;

                            //Update username
                            ClaimsIdentity claims = (ClaimsIdentity)User.Identity;
                            claims.RemoveClaim(claims.FindFirst(ClaimTypes.NameIdentifier));
                            claims.RemoveClaim(claims.FindFirst(ClaimTypes.Name));
                            claims.AddClaims(new List<Claim>
                                {
                                    new Claim(ClaimTypes.NameIdentifier, model.NewEmail.ToLower().Trim()),
                                    new Claim(ClaimTypes.Name, model.NewEmail.ToLower().Trim())
                                });

                            ClaimsIdentity newClaimsIdentity = new ClaimsIdentity(claims.Claims, DefaultAuthenticationTypes.ApplicationCookie);
                            authenticationManager.SignIn(newClaimsIdentity);

                            //Update last active
                            _userService.UpdateGuestLastActive(currentUser.AccountID);

                            TempData["AccountSettingsSuccess"] = Resource.Your_emailaddress_has_been_updated;

                            return RedirectToAction("Settings", "Account", new { Tab = 2 });
                        }
                        else
                            ModelState.AddModelError("AccountSettingsError", Resource.Email_is_already_in_use);
                    }
                    else
                        ModelState.AddModelError("AccountSettingsError", Resource.Incorrect_password);

                }
                else
                    ModelState.AddModelError("AccountSettingsError", Resource.You_have_already_updated_your_emailaddress_try_again_later);
            }
            else
                ModelState.AddModelError("AccountSettingsError", Resource.An_unexpected_error_has_occurred_Please_try_again);

            UsageDTOModel accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);

            AccountLinkPermissionBoolDTOModel accountLinkPermissions = _accountService.GetAccountLinkPermissionsBool(currentUser.AccountOwnerID, currentUser.AccountID);

            SettingsViewModel accountViewModelSettings = new SettingsViewModel()
            {
                Tab = 1,
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                LogCount = accountUsage.LogCount,
                FileRead = accountLinkPermissions.FileRead,
                FileWrite = accountLinkPermissions.FileWrite,
                FileEdit = accountLinkPermissions.FileEdit,
                LinkRead = accountLinkPermissions.LinkRead,
                LinkWrite = accountLinkPermissions.LinkWrite,
                LinkEdit = accountLinkPermissions.LinkEdit,
                UserEmail = currentUser.AccountOwnerEmail
            };

            return View("~/Views/Account/Settings.cshtml", accountViewModelSettings);
        }

        [HttpGet]
        [AllowAnonymous]
        public RedirectToRouteResult UpdateUserPassword()
        {
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeNotClaimAttribute(ClaimTypes.Role, "Guest")]
        public ActionResult UpdateUserPassword(UpdatePasswordViewModel model)
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.My_account;

            if (ModelState.IsValid)
            {
                //Validate ipaddress
                bool adminLogExist = _administrationService.AdminLogExistAuthenticated(User.Identity.Name, "UpdateUserPassword");
                if (!adminLogExist)
                {
                    bool isUserPasswordValid = _userService.IsUserPasswordValid(User.Identity.Name, model.CurrentPassword);

                    //Validate password
                    if (isUserPasswordValid)
                    {
                        ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                        //Update password
                        _userService.UpdateUserPassword(currentUser.AccountID, model.NewPassword);

                        string rowKey = "updatepassword-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                        AdminLogModel adminLog = new AdminLogModel(
                        rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID,
                        currentUser.AccountEmail, currentUser.IpAddress, "UpdateUserPassword");

                        //Insert admin log
                        _administrationService.InsertAdminLog(adminLog);

                        LogModel accountLog = new LogModel(
                            rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                            currentUser.IpAddress, Resource.Update_password);

                        //Insert account log
                        _accountService.InsertAccountLog(accountLog);

                        TempData["AccountSettingsSuccess"] = Resource.Your_password_has_been_updated;

                        return RedirectToAction("Settings", "Account", new { Tab = 1 });
                    }
                    else
                        ModelState.AddModelError("AccountSettingsError", Resource.Incorrect_password);
                }
                else
                    ModelState.AddModelError("AccountSettingsError", Resource.You_have_already_updated_your_password_try_again_later);
            }
            else
                ModelState.AddModelError("AccountSettingsError", Resource.An_unexpected_error_has_occurred_Please_try_again);

            UsageDTOModel accountUsage = _accountService.GetAccountUsageFromCacheOrDB(User.Identity.Name);

            SettingsViewModel accountViewModelSettings = new SettingsViewModel()
            {
                Tab = 1,
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                LogCount = accountUsage.LogCount
            };

            return View("~/Views/Account/Settings.cshtml", accountViewModelSettings);
        }

        [HttpGet]
        [AllowAnonymous]
        public RedirectToRouteResult UpdateGuestPassword()
        {
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim(ClaimTypes.Role, "Guest")]
        public ActionResult UpdateGuestPassword(UpdatePasswordViewModel model)
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.My_account;

            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            if (ModelState.IsValid)
            {
                //Validate ipaddress
                bool adminLogExist = _administrationService.AdminLogExistAuthenticated(User.Identity.Name, "UpdateGuestPassword");
                if (!adminLogExist)
                {
                    bool isPasswordValidGuest = _userService.IsGuestPasswordValid(User.Identity.Name, model.CurrentPassword);

                    //Validate password
                    if (isPasswordValidGuest)
                    {
                        //Update password
                        _userService.UpdateGuestPassword(currentUser.AccountID, model.NewPassword);

                        string rowKey = "updatepassword-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                        AdminLogModel adminLog = new AdminLogModel(
                        rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID,
                        currentUser.AccountEmail, currentUser.IpAddress, "UpdateGuestPassword");

                        //Insert admin log
                        _administrationService.InsertAdminLog(adminLog);

                        TempData["AccountSettingsSuccess"] = Resource.Your_password_has_been_updated;

                        return RedirectToAction("Settings", "Account", new { Tab = 1 });
                    }
                    else
                        ModelState.AddModelError("AccountSettingsError", Resource.Incorrect_password);
                }
                else
                    ModelState.AddModelError("AccountSettingsError", Resource.You_have_already_updated_your_password_try_again_later);
            }
            else
                ModelState.AddModelError("AccountSettingsError", Resource.An_unexpected_error_has_occurred_Please_try_again);

            UsageDTOModel accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);

            AccountLinkPermissionBoolDTOModel accountLinkPermissions = _accountService.GetAccountLinkPermissionsBool(currentUser.AccountOwnerID, currentUser.AccountID);

            SettingsViewModel accountViewModelSettings = new SettingsViewModel()
            {
                Tab = 1,
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                LogCount = accountUsage.LogCount,
                FileRead = accountLinkPermissions.FileRead,
                FileWrite = accountLinkPermissions.FileWrite,
                FileEdit = accountLinkPermissions.FileEdit,
                LinkRead = accountLinkPermissions.LinkRead,
                LinkWrite = accountLinkPermissions.LinkWrite,
                LinkEdit = accountLinkPermissions.LinkEdit,
                UserEmail = currentUser.AccountOwnerEmail
            };

            return View("~/Views/Account/Settings.cshtml", accountViewModelSettings);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public JsonResult ResetUser(ResetAccountViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                //Validate ipaddress
                bool adminLogExist = _administrationService.AdminLogExistAuthenticated(User.Identity.Name, "ResetUser");
                if (!adminLogExist)
                {
                    bool isUserPasswordValid = _userService.IsUserPasswordValid(User.Identity.Name, model.Password);

                    //Validate password
                    if (isUserPasswordValid)
                    {
                        ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                        //Delete azure links
                        _accountService.DeleteAllLinks(currentUser.AccountEmail);

                        //Delete azure files
                        _accountService.DeleteAllFiles(currentUser.AccountEmail);

                        //Delete azure logs
                        _accountService.DeleteAllLogs(currentUser.AccountEmail);

                        //Delete acccount links
                        _accountService.DeleteAllAccountLinksUser(currentUser.AccountID);

                        string rowKey = "resetaccount-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                        AdminLogModel adminLog = new AdminLogModel(
                        rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID,
                        currentUser.AccountEmail, currentUser.IpAddress, "ResetUser");

                        //Insert admin log
                        _administrationService.InsertAdminLog(adminLog);

                        LogModel accountLog = new LogModel(
                            rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                            currentUser.IpAddress, Resource.Reset_account_complete);

                        //Insert account log
                        _accountService.InsertAccountLog(accountLog);

                        //Remove account usage cache
                        DataCache cache = new DataCache("default");
                        object cacheAccountUsage = cache.Get("cacheAccountUsage");
                        if (cacheAccountUsage != null)
                            cache.Remove("cacheAccountUsage");

                        message = Resource.Your_account_has_been_restored;
                        success = true;
                    }
                    else
                        message = Resource.Incorrect_password;
                }
                else
                    message = Resource.You_have_already_reset_your_account_try_again_later;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success
            });
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeNotClaimAttribute(ClaimTypes.Role, "Guest")]
        public JsonResult DeleteUser(DeleteAccountViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                bool IsDeleted = _userService.IsUserDeleted(User.Identity.Name);

                if (!IsDeleted)
                {
                    bool isUserPasswordValid = _userService.IsUserPasswordValid(User.Identity.Name, model.Password);

                    //Validate password
                    if (isUserPasswordValid)
                    {
                        ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                        //Set delete flag
                        _userService.DeleteUser(currentUser.AccountID);

                        //Delete all account links
                        _accountService.DeleteAllAccountLinksUser(currentUser.AccountID);

                        string rowKey = "deleteaccount-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                        AdminLogModel adminLog = new AdminLogModel(
                            rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID,
                            currentUser.AccountEmail, currentUser.IpAddress, "DeleteUser");

                        //Insert admin log
                        _administrationService.InsertAdminLog(adminLog);

                        LogModel accountLog = new LogModel(
                            rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                            currentUser.IpAddress, Resource.Account_deletion_complete);

                        //Insert account log
                        _accountService.InsertAccountLog(accountLog);

                        //Sign out
                        AuthenticationManager.SignOut();

                        TempData["IndexMessage"] = @"<i class=""fa fa-sign-out popup-icon""></i>" + Resource.You_have_been_logged_out_because_your_account_has_been_deleted;

                        success = true;
                    }
                    else
                        message = Resource.Incorrect_password;
                }
                else
                    message = Resource.Your_account_has_already_been_deleted_Log_out_to_execute;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success
            });
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim(ClaimTypes.Role, "Guest")]
        public JsonResult DeleteGuest(DeleteAccountViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                bool IsDeleted = _userService.IsGuestDeleted(User.Identity.Name);

                if (!IsDeleted)
                {
                    bool isPasswordValidGuest = _userService.IsGuestPasswordValid(User.Identity.Name, model.Password);

                    //Validate password
                    if (isPasswordValidGuest)
                    {
                        ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                        //Set delete flag
                        _userService.DeleteGuest(currentUser.AccountID);

                        //Delete all account links
                        _accountService.DeleteAllAccountLinksGuest(currentUser.AccountID);

                        //Sign out
                        AuthenticationManager.SignOut();

                        TempData["IndexMessage"] = @"<i class=""fa fa-sign-out popup-icon""></i>" + Resource.You_have_been_logged_out_because_your_account_has_been_deleted;

                        success = true;
                    }
                    else
                        message = Resource.Incorrect_password;
                }
                else
                    message = Resource.Your_account_has_already_been_deleted_Log_out_to_execute;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExternalAuthentication(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalAuthenticationCallback", "User", new { returnUrl = returnUrl }));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteAccountAuthenticationProvider(DeleteAccountAuthenticationProviderModel model)
        {
            bool success = false;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                if (_userService.IsUserPasswordValid(currentUser.AccountEmail, model.Password))
                {
                    SignInUserDTOModel signInModel = _userService.GetSignInUser(currentUser.AccountEmail);

                    if (!signInModel.IsDeleted)
                    {
                        _userService.DeleteAccountAuthenticationProvider(currentUser.AccountID, model.AuthenticationProviderID);

                        message = Resource.Third_party_authentication_has_been_removed;

                        string rowKey = "deleteaccountauthenticationprovider-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                        LogModel accountLog = new LogModel(
                            rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, Resource.User,
                            currentUser.IpAddress, Resource.Deletion_of_third_party_authentication);

                        //Insert account log
                        _accountService.InsertAccountLog(accountLog);

                        success = true;
                    }
                    else
                        message = Resource.The_user_account_is_disabled_please +
                            " <a class='sign-in-account-disabled-link' href='/Contact/Index''>" +
                            Resource.Contact2 + "</a> " + Resource.Support2;
                }
                else
                    message = Resource.Incorrect_password;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public JsonResult InsertAccountAuthenticationProvider(InsertAccountAuthenticationProviderViewModel model)
        {
            bool success = false;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                if (_userService.IsUserPasswordValid(model.Email, model.Password))
                {
                    SignInUserDTOModel signInModel = _userService.GetSignInUser(model.Email);

                    if (!signInModel.IsDeleted)
                    {
                        int authenticationProviderID = _userService.GetExternalAuthenticationID(model.ProviderName);

                        if (authenticationProviderID > 0)
                        {
                            if (!_userService.AccountAuthenticationProviderExist(signInModel.UserID, authenticationProviderID))
                            {
                                //Insert accountauthenticationprovider
                                _userService.InsertAccountAuthenticationProvider(signInModel.UserID, authenticationProviderID, model.ProviderKey);

                                string ipAddress = IpAddressExtensions.GetIpAddress();

                                string rowKey = "insertaccountauthenticationprovider-" + signInModel.UserID + "-" + GuidExtensions.DateAndGuid();

                                LogModel accountLogAAP = new LogModel(
                                    rowKey, signInModel.UserID, model.Email.ToLower().Trim(), signInModel.UserID, model.Email.ToLower().Trim(), Resource.User,
                                    ipAddress, Resource.Deletion_of_third_party_authentication);

                                //Insert account log AAP
                                _accountService.InsertAccountLog(accountLogAAP);

                                List<Claim> claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, model.Email.ToLower().Trim()),
                                new Claim(ClaimTypes.Name, model.Email.ToLower().Trim()),
                                new Claim(ClaimTypes.Sid, signInModel.UserID.ToString()),
                                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity")
                            };

                                if (signInModel.UserRoles != null && signInModel.UserRoles.Count() > 0)
                                {
                                    foreach (var item in signInModel.UserRoles)
                                    {
                                        Claim claimsTemp = new Claim(ClaimTypes.Role, item);
                                        claims.Add(claimsTemp);
                                    }
                                }

                                ClaimsIdentity identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                                AuthenticationManager.SignIn(identity);

                                string rowKeyLog = "signin-" + signInModel.UserID + "-" + GuidExtensions.DateAndGuid();

                                LogModel accountLogSI = new LogModel(
                                    rowKeyLog, signInModel.UserID, model.Email.ToLower().Trim(), signInModel.UserID, model.Email.ToLower().Trim(), Resource.User,
                                    ipAddress, Resource.Login_by_user);

                                //Insert account log SI
                                _accountService.InsertAccountLog(accountLogSI);

                                string rowKeyUsage = "signin-" + signInModel.UserID + "-" + GuidExtensions.DateAndGuid();

                                UsageModel usage = new UsageModel(rowKeyUsage, model.Email.ToLower(), "SignIn");

                                //Insert usage
                                _accountService.InsertUsage(usage);

                                success = true;

                            }
                            else
                                message = Resource.Your_user_account_has_already_been_linked_to + " " + model.ProviderName + ".";
                        }
                        else
                            message = Resource.Could_not_find_authentication_provider_Please_try_again;
                    }
                    else
                        message = Resource.Your_user_account_is_inactive_and_can_not_be_linked_to + " " + model.ProviderName + ". " + Resource.Please +
                            " <a class='sign-in-account-disabled-link' href='/Contact/Index''>" + Resource.Contact2 + "</a> " + Resource.Support2;
                }
                else
                    message = Resource.Incorrect_username_or_password;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success,
                returnURL = model.ReturnURL
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExternalAuthenticationCallback(string returnUrl)
        {
            var providerName = "";
            var providerKey = "";
            var email = "";
            var firstName = "";
            var lastName = "";
            var externalIdentity = HttpContext.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
            if (externalIdentity.Result == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut();
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return RedirectToAction("Index", "Home");
            }
            var nameIdentifier = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameIdentifier != null)
            {
                providerName = nameIdentifier.Issuer;
                providerKey = nameIdentifier.Value;
            }
            if (providerName == "Facebook")
            {
                var facebookToken = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == "FacebookAccessToken");
                if (facebookToken != null)
                {
                    var fb = new FacebookClient(facebookToken.Value);
                    dynamic myInfo = fb.Get("/me?fields=email,first_name,last_name");
                    email = myInfo.email;
                    firstName = myInfo.first_name;
                    lastName = myInfo.last_name;
                }
            }
            else
            {
                var emailClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var firstnameClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
                var lastnameClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
                if (emailClaim != null)
                {
                    email = emailClaim.Value;
                }
                if (firstnameClaim != null && lastnameClaim != null)
                {
                    firstName = firstnameClaim.Value;
                    lastName = lastnameClaim.Value;
                }
            }

            string decodedUrl = "";
            if (!string.IsNullOrEmpty(returnUrl))
                decodedUrl = Server.UrlDecode(returnUrl);

            SignInUserDTOModel signInModel = _userService.GetSignInUser(email);

            if (signInModel != null && signInModel.UserID > 0)
            {
                //Account exist
                if (!signInModel.IsDeleted)
                {
                    int authenticationProviderID = _userService.GetExternalAuthenticationID(providerName);
                    if (authenticationProviderID > 0)
                    {
                        bool isUserAssociatedWithExternalAuthentication = _userService.AccountAuthenticationProviderExist(signInModel.UserID, authenticationProviderID);

                        if (isUserAssociatedWithExternalAuthentication)
                        {
                            var isAccountAuthenticationProviderKeyValid = _userService.IsAccountAuthenticationProviderKeyValid(signInModel.UserID, authenticationProviderID, providerKey);

                            if (isAccountAuthenticationProviderKeyValid)
                            {
                                //Account is linked with provider and key is correct - Sign in
                                SignInUser(signInModel, email);

                                if (Url.IsLocalUrl(decodedUrl))
                                {
                                    return Redirect(decodedUrl);
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Account");
                                }
                            }
                            else
                            {
                                //Account is linked with provider but key is not correct - Redirect to message view
                                ModelState.AddModelError("Error", Resource.Sign_in_failed + " " + Resource.Incorrect_key);

                                ViewBag.Title = ProductNameWithDomain + " - " + Resource.Sign_in_with + " " + providerName;
                                ViewBag.MessageTitle = Resource.Sign_in_with + " " + providerName;

                                return View("~/Views/Shared/Message.cshtml");
                            }
                        }
                        else
                        {
                            //Account is not yet linked with provider - Redirect to associate view

                            InsertAccountAuthenticationProviderViewModel viewModel = new InsertAccountAuthenticationProviderViewModel()
                            {
                                Email = email,
                                ProviderKey = providerKey,
                                ProviderName = providerName,
                                ReturnURL = returnUrl
                            };

                            return View("~/Views/User/InsertAccountAuthenticationProvider.cshtml", viewModel);
                        }
                    }
                    else
                    {
                        //Could not find authentication provider - redirect to message view
                        ModelState.AddModelError("Error", Resource.Could_not_find_authentication_provider_Please_try_again);

                        ViewBag.Title = ProductNameWithDomain + " - " + Resource.Sign_in_with + " " + providerName;
                        ViewBag.MessageTitle = Resource.Sign_in_with + " " + providerName;

                        return View("~/Views/Shared/Message.cshtml");
                    }
                }
                else
                {
                    //Account is deleted - redirect to message view
                    ModelState.AddModelError("Error", Resource.Sign_in_failed + " " + Resource.The_user_account_is_disabled_please +
                            " <a class='sign-in-account-disabled-link' href='/Contact/Index''>" +
                            Resource.Contact2 + "</a> " + Resource.Support2);

                    ViewBag.Title = ProductNameWithDomain + " - " + Resource.Sign_in_with + " " + providerName;
                    ViewBag.MessageTitle = Resource.Sign_in_with + " " + providerName;

                    return View("~/Views/Shared/Message.cshtml");
                }
            }
            else
            {
                int authenticationProviderID = _userService.GetExternalAuthenticationID(providerName);
                if (authenticationProviderID > 0)
                {
                    //Account does not exist - Register new user
                    var password = GuidExtensions.DateAndGuid();
                    SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();
                    string passwordHash = crypto.Compute(password);

                    string emailToLowerTrim = email.ToLower().Trim();
                    string ipAddress = IpAddressExtensions.GetIpAddress();

                    UserModel newUser = new UserModel()
                    {
                        Email = emailToLowerTrim,
                        PasswordHash = passwordHash,
                        PasswordSalt = crypto.Salt,
                        FirstName = firstName,
                        LastName = lastName,
                        DateRegistered = TimeZoneExtensions.GetCurrentDate(),
                        IsDeleted = false,
                        AgreeUserAgreement = true,
                        DefaultIpAddress = ipAddress,
                        DefaultEmailAddress = emailToLowerTrim,
                        DateLastActive = TimeZoneExtensions.GetCurrentDate()
                    };

                    //Insert user
                    int userID = _userService.InsertUser(newUser);

                    RoleModel role = _userService.GetRole("AccountOwner");
                    if (role != null)
                        _userService.InsertUserRole(role, userID);

                    //Send confirmation email with password och agree
                    _userService.SendEmailRegisterExternalAuthenticationSuccess(emailToLowerTrim, password, providerName);

                    _userService.InsertAccountAuthenticationProvider(userID, authenticationProviderID, providerKey);

                    string rowKey = "registeroauth-" + userID + "-" + GuidExtensions.DateAndGuid();

                    LogModel accountLog = new LogModel(
                        rowKey, userID, newUser.Email, userID, newUser.Email, Resource.User,
                        ipAddress, Resource.Register_account_complete);

                    //Insert account log
                    _accountService.InsertAccountLog(accountLog);

                    SignInUserDTOModel SignInNewUser = new SignInUserDTOModel()
                    {
                        IsDeleted = false,
                        UserID = userID,
                        UserRoles = new List<string> { role != null ? role.Name : "" }
                    };

                    //Sign in new user
                    SignInUser(SignInNewUser, email);

                    if (Url.IsLocalUrl(decodedUrl))
                    {
                        return Redirect(decodedUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Account");
                    }
                }
                else
                {
                    //Could not find authentication provider - redirect to message view
                    ModelState.AddModelError("Error", Resource.Could_not_find_authentication_provider_Please_try_again);

                    ViewBag.Title = ProductNameWithDomain + " - " + Resource.Sign_in_with + " " + providerName;
                    ViewBag.MessageTitle = Resource.Sign_in_with + " " + providerName;

                    return View("~/Views/Shared/Message.cshtml");
                }
            }
        }

        private const string XsrfKey = "XsrfId";
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                context.RequestContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private void SignInUser(SignInUserDTOModel signInModel, string email)
        {
            string emailToLowerTrim = email.ToLower().Trim();

            List<Claim> claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, emailToLowerTrim),
                                new Claim(ClaimTypes.Name, emailToLowerTrim),
                                new Claim(ClaimTypes.Sid, signInModel.UserID.ToString()),
                                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity")
                            };

            if (signInModel.UserRoles != null && signInModel.UserRoles.Count() > 0)
            {
                foreach (var item in signInModel.UserRoles)
                {
                    Claim claimsTemp = new Claim(ClaimTypes.Role, item);
                    claims.Add(claimsTemp);
                }
            }

            ClaimsIdentity identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(identity);

            string ipAddress = IpAddressExtensions.GetIpAddress();
            string rowKeyLog = "signin-" + signInModel.UserID + "-" + GuidExtensions.DateAndGuid();

            LogModel accountLog = new LogModel(
                rowKeyLog, signInModel.UserID, emailToLowerTrim, signInModel.UserID, emailToLowerTrim, Resource.User,
                ipAddress, Resource.Login_by_user);

            //Insert account log
            _accountService.InsertAccountLog(accountLog);

            string rowKeyUsage = "signin-" + signInModel.UserID + "-" + GuidExtensions.DateAndGuid();

            UsageModel usage = new UsageModel(rowKeyUsage, emailToLowerTrim, "SignIn");

            //Insert usage
            _accountService.InsertUsage(usage);
        }
    }
}
