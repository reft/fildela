using Fildela.Resources;
using Fildela.Business.Domains.Account.Models;
using Fildela.Business.Domains.User.Models;
using Fildela.Business.Helpers;
using Fildela.Data.Repositories.User;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Fildela.Business.Domains.User
{
    public class UserService : IUserService
    {
        private readonly string BaseAddress = ConfigurationManager.AppSettings["BaseAddress"];
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];
        private readonly string UserAgreementAddress = ConfigurationManager.AppSettings["BaseAddress"] + ConfigurationManager.AppSettings["UserAgreementAddress"];

        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool RegisterVerificationExist(string email)
        {
            return _userRepository.RegisterVerificationExist(email);
        }

        public void InsertRegisterVerification(RegisterVerificationModel registerVerification)
        {
            _userRepository.InsertRegisterVerification(registerVerification.ToEntity());
        }

        public RegisterVerificationModel GetRegisterVerification(string email, string key)
        {
            return _userRepository.GetRegisterVerification(email, key).ToModel();
        }

        public void DeleteRegisterVerification(string email)
        {
            _userRepository.DeleteRegisterVerification(email);
        }

        public bool ResetPasswordExist(string email, bool isUser)
        {
            return _userRepository.ResetPasswordExist(email, isUser);
        }

        public void InsertResetPassword(Models.ResetPasswordModel resetPassword)
        {
            _userRepository.InsertResetPassword(resetPassword.ToEntity());
        }

        public Models.ResetPasswordModel GetResetPassword(string rowKey, string key, bool isUser)
        {
            return _userRepository.GetResetPassword(rowKey, key, isUser).ToModel();
        }

        public void DeleteResetPassword(string rowKey, bool isUser)
        {
            _userRepository.DeleteResetPassword(rowKey, isUser);
        }

        public Models.RoleModel GetRole(string roleName)
        {
            return _userRepository.GetRole(roleName).ToModel();
        }

        public bool UserExist(string email)
        {
            return _userRepository.UserExist(email);
        }

        public int InsertUser(Models.UserModel user)
        {
            return _userRepository.InsertUser(user.ToEntity());
        }

        public void InsertUserRole(Models.RoleModel newRole, int userID)
        {
            _userRepository.InsertUserRole(userID, newRole.ToEntity());
        }

        public bool IsUserPasswordValid(string email, string password)
        {
            return _userRepository.IsUserPasswordValid(email, password);
        }

        public bool IsGuestDeleted(string email)
        {
            return _userRepository.IsGuestDeleted(email);
        }

        public bool IsUserDeleted(string email)
        {
            return _userRepository.IsUserDeleted(email);
        }

        public int GetUserID(string email)
        {
            return _userRepository.GetUserID(email);
        }

        public IEnumerable<string> GetUserRoles(int userID)
        {
            return _userRepository.GetUserRoles(userID);
        }

        public void UpdateUserLastActive(int userID)
        {
            _userRepository.UpdateUserLastActive(userID);
        }

        public bool IsGuestPasswordValid(string email, string password)
        {
            return _userRepository.IsGuestPasswordValid(email, password);
        }

        public IEnumerable<Models.UserModel> GetAccountLinksForGuest(string email)
        {
            return _userRepository.GetAccountLinksForGuest(email).Select(o => o.ToModel());
        }

        public int GetGuestID(string email)
        {
            return _userRepository.GetGuestID(email);
        }

        public bool IsGuestLinkedWithUser(int guestID, int userID)
        {
            return _userRepository.IsGuestLinkedWithUser(guestID, userID);
        }

        public void UpdateGuestLastActive(int userID)
        {
            _userRepository.UpdateGuestLastActive(userID);
        }

        public bool UserExistAndActive(string email)
        {
            return _userRepository.UserExistAndActive(email);
        }

        public string ResetPasswordUser(string email)
        {
            return _userRepository.ResetPasswordUser(email);
        }

        public bool GuestExistAndActive(string email)
        {
            return _userRepository.GuestExistAndActive(email);
        }

        public string ResetPasswordGuest(string email)
        {
            return _userRepository.ResetPasswordGuest(email);
        }

        public string GetUserEmail(int userID)
        {
            return _userRepository.GetUserEmail(userID);
        }

        public void SendEmailRegisterVerification(RegisterVerificationModel registerVerification)
        {
            StringBuilder sbEmailBody = new StringBuilder();

            sbEmailBody.Append(Resource.Now_you_are_just_one_click_away_from_becoming_a_member_of_Swedens_fastest_file_sharing_site + "<br/><br/>");
            sbEmailBody.Append(Resource.Click_on_the_link_below_to_active_your_useraccount + "<br/>");
            sbEmailBody.Append("<a href=\"" + BaseAddress + "/user/registerverification?email=" +
                registerVerification.RowKey.ToLower().Trim() + "&key=" + registerVerification.Key + "\"" +
                " style=\"text-decoration:none;\">" + BaseAddress + "/user/registerverification?email=" +
                @registerVerification.RowKey.ToLower().Trim() + "&key=" + @registerVerification.Key + "</a>");
            sbEmailBody.Append("<br/><br/><a href=\"" + BaseAddress + "\"" +
                " style=\"text-decoration:none;\">" + ProductNameWithDomain + "</a>");

            string subject = ProductNameWithDomain + " - " + Resource.Activate_your_useraccount;

            BusinessSMTPExtensions.SendEmail(subject, registerVerification.RowKey.ToLower().Trim(), sbEmailBody.ToString(), false);
        }

        public void SendEmailRegisterSuccess(string email)
        {
            StringBuilder sbEmailBody = new StringBuilder();
            sbEmailBody.Append(Resource.Welcome_to + " " + ProductNameWithDomain + "!<br/><br/>");
            sbEmailBody.Append(Resource.Your_user_account_has_been_activated + ".");

            sbEmailBody.Append("<br/><br/><a href=\"" + BaseAddress + "\"" +
    " style=\"text-decoration:none;\">" + ProductNameWithDomain + "</a>");

            string subject = ProductNameWithDomain + " - " + Resource.Your_user_account_has_been_activated;

            BusinessSMTPExtensions.SendEmail(subject, email.ToLower().Trim(), sbEmailBody.ToString(), false);
        }

        public void SendEmailResetPassword(ResetPasswordModel resetPassword)
        {
            StringBuilder sbEmailBody = new StringBuilder();

            sbEmailBody.Append(Resource.Someone_has_requested_a_password_reset + "<br/>");
            sbEmailBody.Append(Resource.If_it_is_not_you_who_has_requested_a_new_password_you_can_ignore_this_mail + "<br/><br/>");
            sbEmailBody.Append(Resource.Click_on_the_link_below_to_reset_your_password + "<br/>");

            string subject = string.Empty;

            if (resetPassword.IsUser)
            {
                subject = ProductNameWithDomain + " - " + Resource.Resetting_password_user;

                sbEmailBody.Append("<a href=\"" + BaseAddress + "/user/resetpasswordverificationuser?rowkey=" +
                    resetPassword.RowKey.ToLower().Trim() + "&key=" + resetPassword.Key + "\"" +
                    " style=\"text-decoration:none;\">" + BaseAddress + "/user/resetpasswordverificationuser?rowkey=" +
                    @resetPassword.RowKey.ToLower().Trim() + "&key=" + @resetPassword.Key + "</a>");
            }
            else
            {
                subject = ProductNameWithDomain + " - " + Resource.Resetting_password_guest;

                sbEmailBody.Append("<a href=\"" + BaseAddress + "/user/resetpasswordverificationguest?rowkey=" +
                    resetPassword.RowKey.ToLower().Trim() + "&key=" + resetPassword.Key + "\"" +
                    " style=\"text-decoration:none;\">" + BaseAddress + "/user/resetpasswordverificationguest?rowkey=" +
                    @resetPassword.RowKey.ToLower().Trim() + "&key=" + @resetPassword.Key + "</a>");
            }

            sbEmailBody.Append("<br/><br/>" + Resource.When_you_visit_this_site_your_current_password_will_expire_and_the_new_will_be_sent_to_you_via_email);

            sbEmailBody.Append("<br/><br/><a href=\"" + BaseAddress + "\"" +
                " style=\"text-decoration:none;\">" + ProductNameWithDomain + "</a>");

            BusinessSMTPExtensions.SendEmail(subject, resetPassword.Email.ToLower().Trim(), sbEmailBody.ToString(), false);
        }

        public void SendEmailResetPasswordNewPassword(string email, string newPassword, bool isUser)
        {
            StringBuilder sbEmailBody = new StringBuilder();

            sbEmailBody.Append(Resource.Here_is_your_new_password_along_with_the_user_type_and_user_name + "<br/><br/>");

            string subject = string.Empty;

            if (isUser)
            {
                subject = ProductNameWithDomain + " - " + Resource.New_password_user;

                sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.User_type2 + "</span><br/>" + Resource.User + "<br/><br/>");
            }
            else
            {
                subject = ProductNameWithDomain + " - " + Resource.New_password_guest;

                sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.User_type2 + "</span><br/>" + Resource.Guest + "<br/><br/>");
            }

            sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.Username2 + "</span><br/>" + email.ToLower().Trim() + "<br/>");
            sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.Password2 + "</span><br/>" + newPassword.Trim() + "<br/><br/>");
            sbEmailBody.Append(Resource.Click_on_the_link_below_to_sign_in_and_change_to_a_new_password + "<br/>");
            sbEmailBody.Append("<a href=\"" + BaseAddress + "/account/settings" + "\"" +
            " style=\"text-decoration:none;\">" + BaseAddress + "/account/settings" + "</a>");
            sbEmailBody.Append("<br/><br/><a href=\"" + BaseAddress + "\"" +
                " style=\"text-decoration:none;\">" + ProductNameWithDomain + "</a>");

            BusinessSMTPExtensions.SendEmail(subject, email.ToLower().Trim(), sbEmailBody.ToString(), false);
        }

        public void SendEmailContact(string senderName, string senderEmail, string categoryName, string message)
        {
            StringBuilder sbEmailBody = new StringBuilder();
            sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.Name + ":</span> " + BusinessStringExtensions.FirstCharToUpper(senderName) + "<br/>");
            sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.Email + ":</span> " + BusinessStringExtensions.FirstCharToUpper(senderEmail) + "<br/><br/>");

            sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.Message2 + "</span><br/>");
            sbEmailBody.Append(message);

            sbEmailBody.Append("<br/><br/><a href=\"" + BaseAddress + "\"" +
            " style=\"text-decoration:none;\">" + ProductNameWithDomain + "</a>");

            string subject = ProductNameWithDomain + " - " + Resource.Contact + " - " + ResourceHelpers.GetString<Resource>(categoryName);

            BusinessSMTPExtensions.SendEmail(subject, null, sbEmailBody.ToString(), false);
        }

        public SignInAccountLinksDTOModel GetSignInAccountLinks(string email)
        {
            return _userRepository.GetSignInAccountLinks(email).ToModel();
        }

        public SignInUserDTOModel GetSignInUser(string email)
        {
            return _userRepository.GetSignInUser(email).ToModel();
        }

        public SignInGuestDTOModel GetSignInGuest(string guestEmail, int userId)
        {
            return _userRepository.GetSignInGuest(guestEmail, userId).ToModel();
        }

        public bool GuestExist(string email)
        {
            return _userRepository.GuestExist(email);
        }

        public int InsertGuest(User.Models.GuestModel guest)
        {
            return _userRepository.InsertGuest(guest.ToEntity());
        }

        public void UpdateUserEmail(int userID, string newEmail)
        {
            _userRepository.UpdateUserEmail(userID, newEmail);
        }

        public void UpdateGuestEmail(int userID, string newEmail)
        {
            _userRepository.UpdateGuestEmail(userID, newEmail);
        }

        public void DeleteUser(int userID)
        {
            _userRepository.DeleteUser(userID);
        }

        public void DeleteGuest(int guestID)
        {
            _userRepository.DeleteGuest(guestID);
        }

        public void UpdateGuestPassword(int userID, string newPassword)
        {
            _userRepository.UpdateGuestPassword(userID, newPassword);
        }

        public void UpdateUserPassword(int userID, string newPassword)
        {
            _userRepository.UpdateUserPassword(userID, newPassword);
        }

        public void RestoreUser(int accountOwnerID)
        {
            _userRepository.RestoreUser(accountOwnerID);
        }

        public void RestoreGuest(int guestID)
        {
            _userRepository.RestoreGuest(guestID);
        }

        public bool AccountAuthenticationProviderExist(int accountID, int authenticationProviderID)
        {
            return _userRepository.AccountAuthenticationProviderExist(accountID, authenticationProviderID);
        }

        public int GetExternalAuthenticationID(string externalAuthenticationName)
        {
            return _userRepository.GetExternalAuthenticationID(externalAuthenticationName);
        }

        public void InsertAccountAuthenticationProvider(int accountID, int authenticationProviderID, string key)
        {
            _userRepository.InsertAccountAuthenticationProvider(accountID, authenticationProviderID, key);
        }

        public AccountAuthenticationProviderModel GetAccountAuthenticationProvider(string email, string authenticationProviderName)
        {
            return _userRepository.GetAccountAuthenticationProvider(email, authenticationProviderName).ToModel();
        }

        public bool IsAccountAuthenticationProviderKeyValid(int accountID, int authenticationProviderID, string key)
        {
            return _userRepository.IsAccountAuthenticationProviderKeyValid(accountID, authenticationProviderID, key);
        }

        public void SendEmailRegisterExternalAuthenticationSuccess(string email, string password, string provider)
        {
            StringBuilder sbEmailBody = new StringBuilder();
            sbEmailBody.Append(Resource.Welcome_to + " " + ProductNameWithDomain + "!<br/><br/>");
            sbEmailBody.Append(Resource.Your_user_account_has_been_created_and_activated + ".<br/>");

            sbEmailBody.Append(Resource.Because_you_have_registered_and_logged_in_with + " " + provider + " " + Resource.A_random_password_has_been_created_for_regular_sign_in + "<br/><br/>");

            sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.Username2 + "</span><br/>" + email.ToLower().Trim() + "<br/>");
            sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.Password2 + "</span><br/>" + password.Trim() + "<br/><br/>");
            sbEmailBody.Append(Resource.Click_on_the_link_below_to_sign_in_and_change_to_a_new_password + "<br/>");
            sbEmailBody.Append("<a href=\"" + BaseAddress + "/account/settings" + "\"" +
            " style=\"text-decoration:none;\">" + BaseAddress + "/account/settings" + "</a>");

            sbEmailBody.Append("<br/><br/>" + Resource.Upon_activation_and_use_of_the_account_with_external_login_provider_you_automatically_accept_our + @" <a href=""" + UserAgreementAddress + @""" style=""text-decoration:none;"">" + Resource.User_agreement2 + "</a>.");

            sbEmailBody.Append("<br/><br/><a href=\"" + BaseAddress + "\"" +
    " style=\"text-decoration:none;\">" + ProductNameWithDomain + "</a>");

            string subject = ProductNameWithDomain + " - " + Resource.Your_user_account_has_been_created_and_activated;

            BusinessSMTPExtensions.SendEmail(subject, email.ToLower().Trim(), sbEmailBody.ToString(), false);
        }

        public IEnumerable<AuthenticationProviderModel> GetAuthenticationProviders()
        {
            return _userRepository.GetAuthenticationProviders().Select(m => m.ToModel());
        }

        public IEnumerable<AccountAuthenticationProviderModel> GetAccountAuthenticationProviders(int accountID)
        {
            return _userRepository.GetAccountAuthenticationProviders(accountID).Select(m => m.ToModel());
        }

        public bool DeleteAccountAuthenticationProvider(int accountID, int authenticationProviderID)
        {
            return _userRepository.DeleteAccountAuthenticationProvider(accountID, authenticationProviderID);
        }
    }
}
