using Fildela.Web.Helpers;
using Fildela.Web.Models;
using Microsoft.ApplicationServer.Caching;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using Fildela.Web.Models.AccountModels;
using Fildela.Business.Domains.Administration;
using Fildela.Business.Domains.Account;
using Fildela.Business.Domains.User;
using Fildela.Business.Domains.Account.Models;
using Fildela.Business.Domains.User.Models;
using System.Configuration;
using Fildela.Web.Models.UserModels;
using Fildela.Resources;

namespace Fildela.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];

        private readonly IAdministrationService _administrationService;
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        public AccountController(IAccountService accountService, IAdministrationService administrationService, IUserService userService)
        {
            _accountService = accountService;
            _administrationService = administrationService;
            _userService = userService;
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [HttpGet]
        public ActionResult AccessDenied()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Access_denied;

            AccountLinkPermissionBoolDTOModel accountLinkPermissions = new AccountLinkPermissionBoolDTOModel();
            UsageDTOModel accountUsage = new UsageDTOModel();

            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            if (!User.IsInRole("AccountOwner"))
            {
                accountLinkPermissions = _accountService.GetAccountLinkPermissionsBool(currentUser.AccountOwnerID, currentUser.AccountID);

                bool ActivePermission = accountLinkPermissions.GetType().GetProperties()
                 .Where(p => p.GetValue(accountLinkPermissions) is bool)
                 .Any(p => (bool)p.GetValue(accountLinkPermissions) == true);

                if (!ActivePermission)
                {
                    AuthenticationManager.SignOut();

                    TempData["IndexMessage"] = @"<i class=""fa fa-sign-out popup-icon""></i>" + Resource.You_have_been_logged_out_because_you_no_longer_have_any_rights;

                    return RedirectToAction("Index", "Home");
                }
            }

            accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);

            AccountViewModel accountViewModelSettings = new AccountViewModel()
            {
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

            return View(accountViewModelSettings);
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (User.IsInRole("AccountOwner"))
                return RedirectToAction("Files");
            else if (User.IsInRole("Guest"))
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                AccountLinkPermissionBoolDTOModel accountLinkPermissions = _accountService.GetAccountLinkPermissionsBool(currentUser.AccountOwnerID, currentUser.AccountID);

                if (accountLinkPermissions.FileRead || accountLinkPermissions.FileWrite || accountLinkPermissions.FileEdit)
                    return RedirectToAction("Files");
                else if (accountLinkPermissions.LinkRead || accountLinkPermissions.LinkWrite || accountLinkPermissions.LinkEdit)
                    return RedirectToAction("Links");
                else
                {
                    ViewBag.Title = ProductNameWithDomain + " - " + Resource.Access_denied;

                    AuthenticationManager.SignOut();

                    TempData["IndexMessage"] = @"<i class=""fa fa-sign-out popup-icon""></i>" + Resource.You_have_been_logged_out_because_you_no_longer_have_any_rights;

                    return RedirectToAction("Index", "Home");
                }
            }
            else if (User.IsInRole("Admin") || User.IsInRole("Support") || User.IsInRole("Publisher"))
                return RedirectToAction("Index", "Administration");
            else
                return RedirectToAction("AccessDenied");
        }

        [HttpGet]
        [AuthorizePermissionAttribute("FileRead", "FileWrite", "FileEdit")]
        public ActionResult Files()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.My_account;

            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            UsageDTOModel accountUsage = new UsageDTOModel();
            AccountLinkPermissionBoolDTOModel accountLinkPermissions = new AccountLinkPermissionBoolDTOModel();

            if (!User.IsInRole("AccountOwner"))
                accountLinkPermissions = _accountService.GetAccountLinkPermissionsBool(currentUser.AccountOwnerID, currentUser.AccountID);

            List<FileModel> files = _accountService.GetFiles(currentUser.AccountOwnerEmail).ToList();

            accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);

            long storedBytes = files.Sum(b => b.FileSizeBytes);

            //Update cache if file count from Azure storage isnt equal to cache
            if (accountUsage.FileCount != files.Count() || accountUsage.StoredBytes != storedBytes)
            {
                //Update cache
                accountUsage.FileCount = files.Count;
                accountUsage.StoredBytes = storedBytes;

                DataCache cache = new DataCache("default");
                object cacheAccountUsage = cache.Get("cacheAccountUsage");

                cache.Remove("cacheAccountUsage");
            }

            FilesViewModel accountFilesViewModel = new FilesViewModel()
            {
                Files = files,
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
                StoredBytes = accountUsage.StoredBytes,
                AllowedFileCount = accountUsage.AllowedFileCount,
                AllowedStoredBytes = accountUsage.AllowedStoredBytes,
                UserEmail = currentUser.AccountOwnerEmail
            };

            return View(accountFilesViewModel);
        }

        [HttpGet]
        [AjaxOnly]
        [AuthorizePermissionAttribute("FileRead", "FileWrite", "FileEdit")]
        public PartialViewResult UpdateFilesContent()
        {
            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            UsageDTOModel accountUsage = new UsageDTOModel();
            AccountLinkPermissionBoolDTOModel accountLinkPermissions = new AccountLinkPermissionBoolDTOModel();

            if (!User.IsInRole("AccountOwner"))
                accountLinkPermissions = _accountService.GetAccountLinkPermissionsBool(currentUser.AccountOwnerID, currentUser.AccountID);

            List<FileModel> files = _accountService.GetFiles(currentUser.AccountOwnerEmail).ToList();

            accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);

            long storedBytes = files.Sum(b => b.FileSizeBytes);

            //Update cache if file count from Azure storage isnt equal to cache
            if (accountUsage.FileCount != files.Count() || accountUsage.StoredBytes != storedBytes)
            {
                //Update cache
                accountUsage.FileCount = files.Count;
                accountUsage.StoredBytes = storedBytes;

                DataCache cache = new DataCache("default");
                object cacheAccountUsage = cache.Get("cacheAccountUsage");

                cache.Remove("cacheAccountUsage");
            }

            FilesViewModel accountFilesViewModel = new FilesViewModel()
            {
                Files = files,
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                LogCount = accountUsage.LogCount,
                AllowedFileCount = accountUsage.AllowedFileCount,
                AllowedStoredBytes = accountUsage.AllowedStoredBytes,
                StoredBytes = accountUsage.StoredBytes,
                FileRead = accountLinkPermissions.FileRead,
                FileWrite = accountLinkPermissions.FileWrite,
                FileEdit = accountLinkPermissions.FileEdit,
                LinkRead = accountLinkPermissions.LinkRead,
                LinkWrite = accountLinkPermissions.LinkWrite,
                LinkEdit = accountLinkPermissions.LinkEdit,
                UserEmail = currentUser.AccountOwnerEmail
            };

            return PartialView("~/Views/Account/Partials/_FilesContentPartial.cshtml", accountFilesViewModel);
        }

        [HttpGet]
        [AuthorizePermissionAttribute("LinkRead", "LinkWrite", "LinkEdit")]
        public ActionResult Links()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.My_account;

            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            UsageDTOModel accountUsage = new UsageDTOModel();
            AccountLinkPermissionBoolDTOModel accountLinkPermissions = new AccountLinkPermissionBoolDTOModel();

            if (!User.IsInRole("AccountOwner"))
                accountLinkPermissions = _accountService.GetAccountLinkPermissionsBool(currentUser.AccountOwnerID, currentUser.AccountID);

            List<LinkModel> links = _accountService.GetLinks(currentUser.AccountOwnerEmail).ToList();

            accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);

            //Update cache if link count from Azure storage isnt equal to cache
            if (accountUsage.LinkCount != links.Count())
            {
                //Update cache
                accountUsage.LinkCount = links.Count;

                DataCache cache = new DataCache("default");
                object cacheAccountUsage = cache.Get("cacheAccountUsage");

                cache.Remove("cacheAccountUsage");
            }

            LinksViewModel accountLinksViewModel = new LinksViewModel()
            {
                Links = links,
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                LogCount = accountUsage.LogCount,
                AllowedLinkCount = accountUsage.AllowedLinkCount,
                FileRead = accountLinkPermissions.FileRead,
                FileWrite = accountLinkPermissions.FileWrite,
                FileEdit = accountLinkPermissions.FileEdit,
                LinkRead = accountLinkPermissions.LinkRead,
                LinkWrite = accountLinkPermissions.LinkWrite,
                LinkEdit = accountLinkPermissions.LinkEdit,
                UserEmail = currentUser.AccountOwnerEmail
            };

            return View(accountLinksViewModel);
        }

        [HttpGet]
        [AjaxOnly]
        [AuthorizePermissionAttribute("LinkRead", "LinkWrite", "LinkEdit")]
        public PartialViewResult UpdateLinksContent()
        {
            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            UsageDTOModel accountUsage = new UsageDTOModel();
            AccountLinkPermissionBoolDTOModel accountLinkPermissions = new AccountLinkPermissionBoolDTOModel();

            if (!User.IsInRole("AccountOwner"))
                accountLinkPermissions = _accountService.GetAccountLinkPermissionsBool(currentUser.AccountOwnerID, currentUser.AccountID);

            List<LinkModel> links = _accountService.GetLinks(currentUser.AccountOwnerEmail).ToList();

            accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);

            //Update cache if link count from Azure storage isnt equal to cache
            if (accountUsage.LinkCount != links.Count())
            {
                //Update cache
                accountUsage.LinkCount = links.Count;

                DataCache cache = new DataCache("default");
                object cacheAccountUsage = cache.Get("cacheAccountUsage");

                cache.Remove("cacheAccountUsage");
            }

            LinksViewModel accountLinksViewModel = new LinksViewModel()
            {
                Links = links,
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                AllowedLinkCount = accountUsage.AllowedLinkCount,
                LogCount = accountUsage.LogCount,
                FileRead = accountLinkPermissions.FileRead,
                FileWrite = accountLinkPermissions.FileWrite,
                FileEdit = accountLinkPermissions.FileEdit,
                LinkRead = accountLinkPermissions.LinkRead,
                LinkWrite = accountLinkPermissions.LinkWrite,
                LinkEdit = accountLinkPermissions.LinkEdit,
                UserEmail = currentUser.AccountOwnerEmail
            };

            return PartialView("~/Views/Account/Partials/_LinksContentPartial.cshtml", accountLinksViewModel);
        }

        [HttpGet]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public ViewResult GuestAccounts()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.My_account;

            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            UsageDTOModel accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);
            List<GuestDTOModel> guestAccounts = _accountService.GetGuestsLinkedWithUser(currentUser.AccountOwnerID).ToList();
            List<AccountLinkVerificationModel> pendingGuestAccounts = _accountService.GetPendingGuestsLinkedWithUser(currentUser.AccountOwnerEmail).ToList();
            int totalAccounts = guestAccounts.Count() + pendingGuestAccounts.Count();

            //Update cache if guestaccount count from DB isnt equal to cache
            if (accountUsage.GuestAccountCount != totalAccounts)
            {
                //Update cache
                accountUsage.GuestAccountCount = totalAccounts;

                DataCache cache = new DataCache("default");
                object cacheAccountUsage = cache.Get("cacheAccountUsage");

                cache.Remove("cacheAccountUsage");
            }

            GuestAccountsViewModel accountGuestAccountsViewModel = new GuestAccountsViewModel()
            {
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                LogCount = accountUsage.LogCount,
                AllowedGuestAccountCount = accountUsage.AllowedGuestAccountCount,
                GuestAccounts = guestAccounts,
                PendingGuestAccounts = pendingGuestAccounts
            };

            return View(accountGuestAccountsViewModel);
        }

        [HttpGet]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public PartialViewResult UpdateGuestAccountsContent()
        {
            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            UsageDTOModel accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);
            List<GuestDTOModel> guestAccounts = _accountService.GetGuestsLinkedWithUser(currentUser.AccountOwnerID).ToList();
            List<AccountLinkVerificationModel> pendingGuestAccounts = _accountService.GetPendingGuestsLinkedWithUser(currentUser.AccountOwnerEmail).ToList();
            int totalAccounts = guestAccounts.Count() + pendingGuestAccounts.Count();

            //Update cache if guestaccount count from DB isnt equal to cache
            if (accountUsage.GuestAccountCount != totalAccounts)
            {
                //Update cache
                accountUsage.GuestAccountCount = totalAccounts;

                DataCache cache = new DataCache("default");
                object cacheAccountUsage = cache.Get("cacheAccountUsage");

                cache.Remove("cacheAccountUsage");
            }

            GuestAccountsViewModel accountGuestAccountsViewModel = new GuestAccountsViewModel()
            {
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                LogCount = accountUsage.LogCount,
                AllowedGuestAccountCount = accountUsage.AllowedGuestAccountCount,
                GuestAccounts = guestAccounts,
                PendingGuestAccounts = pendingGuestAccounts,
                UserEmail = currentUser.AccountOwnerEmail
            };

            return PartialView("~/Views/Account/Partials/_GuestAccountsContentPartial.cshtml", accountGuestAccountsViewModel);
        }

        [HttpGet]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public ViewResult Logs()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.My_account;

            UsageDTOModel accountUsage = _accountService.GetAccountUsageFromCacheOrDB(User.Identity.Name);
            List<LogModel> logs = _accountService.GetLogs(User.Identity.Name).ToList();

            //Update cache if log count from Azure storage isnt equal to cache
            if (accountUsage.LogCount != logs.Count)
            {
                accountUsage.LogCount = logs.Count;

                DataCache cache = new DataCache("default");
                object cacheAccountUsage = cache.Get("cacheAccountUsage");

                cache.Remove("cacheAccountUsage");
            }

            LogsViewModel accountLogsViewModel = new LogsViewModel()
            {
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                LogCount = accountUsage.LogCount,
                Logs = logs
            };

            return View(accountLogsViewModel);
        }

        [HttpGet]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public PartialViewResult UpdateLogsContent()
        {
            UsageDTOModel accountUsage = _accountService.GetAccountUsageFromCacheOrDB(User.Identity.Name);
            List<LogModel> logs = _accountService.GetLogs(User.Identity.Name).ToList();

            //Update cache if log count from Azure storage isnt equal to cache
            if (accountUsage.LogCount != logs.Count)
            {
                accountUsage.LogCount = logs.Count;

                DataCache cache = new DataCache("default");
                object cacheAccountUsage = cache.Get("cacheAccountUsage");

                cache.Remove("cacheAccountUsage");
            }

            LogsViewModel accountLogsViewModel = new LogsViewModel()
            {
                FileCount = accountUsage.FileCount,
                GuestAccountCount = accountUsage.GuestAccountCount,
                LinkCount = accountUsage.LinkCount,
                LogCount = accountUsage.LogCount,
                Logs = logs
            };

            return PartialView("~/Views/Account/Partials/_LogsContentPartial.cshtml", accountLogsViewModel);
        }

        [HttpGet]
        public ViewResult Settings([DefaultValue(1)]int Tab)
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.My_account;

            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            UsageDTOModel accountUsage = new UsageDTOModel();
            AccountLinkPermissionBoolDTOModel accountLinkPermissions = new AccountLinkPermissionBoolDTOModel();
            List<Models.UserModels.AuthenticationProviderModel> userAuthenticationProviders = new List<Models.UserModels.AuthenticationProviderModel>();

            if (!User.IsInRole("AccountOwner"))
            {
                accountLinkPermissions = _accountService.GetAccountLinkPermissionsBool(currentUser.AccountOwnerID, currentUser.AccountID);
            }
            else
            {
                IEnumerable<Business.Domains.User.Models.AuthenticationProviderModel> providers = _userService.GetAuthenticationProviders().ToList();
                IEnumerable<AccountAuthenticationProviderModel> accountProviders = _userService.GetAccountAuthenticationProviders(currentUser.AccountID).ToList();

                foreach (var item in providers)
                {
                    Models.UserModels.AuthenticationProviderModel userAuthenticationProvider = new Models.UserModels.AuthenticationProviderModel()
                    {
                        AuthenticationProviderID = item.AuthenticationProviderID,
                        Name = item.Name,
                        IsLinked = accountProviders.Any(m => m.AccountID == currentUser.AccountID && m.AuthenticationProviderID == item.AuthenticationProviderID),
                        IconClass = item.IconClass,
                        IconColor = item.IconColor
                    };

                    userAuthenticationProviders.Add(userAuthenticationProvider);
                }
            }

            accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);

            SettingsViewModel accountSettingsViewModel = new SettingsViewModel()
            {
                Tab = Tab,
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
                UserEmail = currentUser.AccountOwnerEmail,
                UserAuthenticationProviders = userAuthenticationProviders
            };

            return View(accountSettingsViewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public PartialViewResult UpdateSettingsAuthenticationContent()
        {
            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            UsageDTOModel accountUsage = new UsageDTOModel();
            AccountLinkPermissionBoolDTOModel accountLinkPermissions = new AccountLinkPermissionBoolDTOModel();
            List<Models.UserModels.AuthenticationProviderModel> userAuthenticationProviders = new List<Models.UserModels.AuthenticationProviderModel>();

            if (!User.IsInRole("AccountOwner"))
            {
                accountLinkPermissions = _accountService.GetAccountLinkPermissionsBool(currentUser.AccountOwnerID, currentUser.AccountID);
            }
            else
            {
                IEnumerable<Business.Domains.User.Models.AuthenticationProviderModel> providers = _userService.GetAuthenticationProviders().ToList();
                IEnumerable<AccountAuthenticationProviderModel> accountProviders = _userService.GetAccountAuthenticationProviders(currentUser.AccountID).ToList();

                foreach (var item in providers)
                {
                    Models.UserModels.AuthenticationProviderModel userAuthenticationProvider = new Models.UserModels.AuthenticationProviderModel()
                    {
                        AuthenticationProviderID = item.AuthenticationProviderID,
                        Name = item.Name,
                        IsLinked = accountProviders.Any(m => m.AccountID == currentUser.AccountID && m.AuthenticationProviderID == item.AuthenticationProviderID),
                        IconClass = item.IconClass,
                        IconColor = item.IconColor
                    };

                    userAuthenticationProviders.Add(userAuthenticationProvider);
                }
            }

            accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);

            SettingsViewModel accountSettingsViewModel = new SettingsViewModel()
            {
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
                UserEmail = currentUser.AccountOwnerEmail,
                UserAuthenticationProviders = userAuthenticationProviders
            };

            return PartialView("~/Views/Account/Partials/_SettingsAuthenticationContentPartial.cshtml", accountSettingsViewModel);
        }

        [HttpGet]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public ViewResult Usage()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.My_account;

            UsageAndStatisticsDTOModel accountUsageAndStatistics = _accountService.GetAccountUsageAndStatistics(User.Identity.Name);

            //Statistics Series
            List<AccountStatisticSeriesViewModel> accountStatisticSeriesListViewModel = new List<AccountStatisticSeriesViewModel>();
            if (accountUsageAndStatistics != null && accountUsageAndStatistics.AccountStatisticsDTOModel != null)
            {
                foreach (var item in accountUsageAndStatistics.AccountStatisticsDTOModel.Series)
                {
                    AccountStatisticSeriesViewModel accountStatisticSeriesViewModel = new AccountStatisticSeriesViewModel()
                    {
                        Data = item.Data,
                        Name = item.Name
                    };

                    accountStatisticSeriesListViewModel.Add(accountStatisticSeriesViewModel);
                }
            }

            //Statistics
            StatisticsViewModel accountStatisticsViewModel = new StatisticsViewModel()
            {
                Series = accountStatisticSeriesListViewModel,
                xAxis = accountUsageAndStatistics.AccountStatisticsDTOModel != null && accountUsageAndStatistics.AccountStatisticsDTOModel.XAxis != null ? accountUsageAndStatistics.AccountStatisticsDTOModel.XAxis : null
            };

            //Usage
            UsageViewModel accountUsageViewModel = new UsageViewModel()
            {
                FileCount = accountUsageAndStatistics.UsageDTOModel.FileCount,
                LinkCount = accountUsageAndStatistics.UsageDTOModel.LinkCount,
                LogCount = accountUsageAndStatistics.UsageDTOModel.LogCount,
                GuestAccountCount = accountUsageAndStatistics.UsageDTOModel.GuestAccountCount,
                StoredBytes = accountUsageAndStatistics.UsageDTOModel.StoredBytes,
                AllowedFileCount = accountUsageAndStatistics.UsageDTOModel.AllowedFileCount,
                AllowedLinkCount = accountUsageAndStatistics.UsageDTOModel.AllowedLinkCount,
                AllowedGuestAccountCount = accountUsageAndStatistics.UsageDTOModel.AllowedGuestAccountCount,
                AllowedStoredBytes = accountUsageAndStatistics.UsageDTOModel.AllowedStoredBytes,
                StatisticsViewModel = accountStatisticsViewModel
            };

            return View(accountUsageViewModel);
        }

        [HttpGet]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public ContentResult GetPermissions(string q)
        {
            JArray jPermissions = new JArray();

            List<PermissionModel> permissionDTO = _accountService.GetPermissions().ToList();

            foreach (var item in permissionDTO)
            {
                if (String.IsNullOrEmpty(q) || item.Name.ToLower().Contains(q.ToLower()))
                {
                    jPermissions.Add(new JObject(
                        new JProperty("id", item.PermissionID),
                        new JProperty("text", ResourceHelpers.GetString<Resource>(item.Name))
                    ));
                }
            }

            return Content(jPermissions.ToString(), "application/json");
        }

        [HttpGet]
        [AuthorizePermissionAttribute("LinkWrite")]
        public ContentResult GetFiles(string q)
        {
            JArray jPermissions = new JArray();

            ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

            List<FileModel> files = _accountService.GetFiles(currentUser.AccountOwnerEmail).ToList();

            int index = 0;
            foreach (var item in files)
            {
                if (String.IsNullOrEmpty(q) || item.FileName.ToLower().Contains(q.ToLower()))
                {
                    jPermissions.Add(new JObject(
                        new JProperty("id", index),
                        new JProperty("rowkey", item.RowKey),
                        new JProperty("text", item.FileName)
                    ));

                    index++;
                }
            }

            return Content(jPermissions.ToString(), "application/json");
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizePermissionAttribute("LinkWrite")]
        public JsonResult InsertLink(InsertLinkViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                DateTime currentTime = TimeZoneExtensions.GetCurrentDate().AddMinutes(-15);
                double clientHourDiff = TimeZoneExtensions.GetDateDiffClient(model.CurrentTime);

                model.DateStart = model.DateStart.AddHours(clientHourDiff);
                model.DateExpires = model.DateExpires.AddHours(clientHourDiff);

                if (model.DateStart > currentTime && model.DateStart < currentTime.AddYears(2) &&
                    model.DateExpires > currentTime && model.DateExpires < currentTime.AddYears(2) &&
                    model.DateStart < model.DateExpires)
                {
                    ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                    UsageDTOModel accountUsage = _accountService.GetAccountUsageLinks(currentUser.AccountOwnerEmail);

                    //Validate file size storage
                    if (accountUsage.LinkCount < accountUsage.AllowedLinkCount)
                    {
                        //Get selected json rowkey for file and convert it to a string
                        if (!String.IsNullOrEmpty(model.File))
                        {
                            InsertLinkFileModel jsonFile = JsonConvert.DeserializeObject<InsertLinkFileModel>(model.File);

                            if (!String.IsNullOrEmpty(jsonFile.RowKey))
                            {
                                FileModel file = _accountService.GetFileFromRowKey(jsonFile.RowKey, currentUser.AccountOwnerEmail);

                                if (file != null)
                                {
                                    string linkRowKey = currentUser.AccountID + Fildela.Web.Helpers.GuidExtensions.DateAndGuid().ToUpper();
                                    string downloadLink = this.Url.Action("DownloadLink", "Account", new { RowKey = linkRowKey + "/" }, this.Request.Url.Scheme);

                                    LinkModel link = new LinkModel(linkRowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail,
                                        file.FileName, file.FileExtension, jsonFile.RowKey, file.BlobIconURL, downloadLink, model.DateStart, model.DateExpires);

                                    //Insert link entity
                                    _accountService.InsertLink(link);

                                    string accountLogRowKey = "insertlink-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                                    LogModel accountLog = new LogModel(
                                        accountLogRowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                                        currentUser.IpAddress, Resource.Link_created2);

                                    //Insert account log
                                    _accountService.InsertAccountLog(accountLog);

                                    message = Resource.Link_created;
                                    success = true;
                                }
                                else
                                    message = Resource.The_file_could_not_be_found_Please_try_again;
                            }
                            else message = Resource.An_unexpected_error_has_occurred_Please_try_again;
                        }
                        else
                            message = Resource.An_unexpected_error_has_occurred_Please_try_again;
                    }
                    else
                        message = Resource.You_have_exceeded_the_allowed_number_of_links;
                }
                else
                    message = Resource.Please_enter_correct_dates;
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
        public ActionResult DownloadLink(DownloadLinkViewModel model)
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Download_of_file;
            ViewBag.MessageTitle = Resource.Download_of_file;

            if (ModelState.IsValid)
            {
                LinkModel link = _accountService.GetActiveLinkFromRowKey(model.RowKey);

                if (link != null)
                {
                    DateTime currentTime = TimeZoneExtensions.GetCurrentDate();
                    double DateBlobSASURLExpires = (link.DateBlobSASURLExpires - currentTime).TotalDays;

                    if (DateBlobSASURLExpires > 0 && !String.IsNullOrEmpty(link.BlobSASURI))
                    {
                        if ((currentTime - link.DateLastDownload.AddMinutes(15)).TotalDays > 0)
                        {
                            string rowKeyUsage = "download-" + link.UserID + "-" + GuidExtensions.DateAndGuid();

                            UsageModel usage = new UsageModel(rowKeyUsage, link.UserEmail, "Download");

                            //Insert usage
                            _accountService.InsertUsage(usage);

                            link.DateLastDownload = currentTime;
                            _accountService.UpdateLink(link);
                        }

                        return Redirect(link.BlobSASURI);
                    }
                    else
                    {
                        if ((currentTime - link.DateLastDownload.AddMinutes(15)).TotalDays > 0)
                        {
                            string rowKeyUsage = "download-" + link.UserID + "-" + GuidExtensions.DateAndGuid();

                            UsageModel usage = new UsageModel(rowKeyUsage, link.UserEmail, "Download");

                            //Insert usage
                            _accountService.InsertUsage(usage);

                            link.DateLastDownload = currentTime;
                        }

                        link.DateBlobSASURLExpires = currentTime.AddHours(1);

                        string newBloBSASURL = _accountService.GetDownloadSASURIForLink(link.BlobName, link.FileName, "." + link.FileExtension, 1);

                        link.BlobSASURI = newBloBSASURL;

                        _accountService.UpdateLink(link);

                        return Redirect(newBloBSASURL);
                    }
                }
                else
                {
                    string message = Resource.The_file_could_not_be_downloaded + "<p style='margin-bottom:20px; font-size:16px;'>" + Resource.This_may_be_because_the_file_could_not_be_found_or_the_expiration_date_has_passed + "</p>";

                    ModelState.AddModelError("Error", message);

                    return View("~/Views/Shared/Message.cshtml");
                }
            }
            else
            {
                ModelState.AddModelError("Error", Resource.An_unexpected_error_has_occurred_Please_try_again);

                return View("~/Views/Shared/Message.cshtml");
            }
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public JsonResult InsertAccountLink(InsertAccountLinkViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                if (currentUser.AccountEmail != model.Email.ToLower().Trim())
                {
                    DateTime currentTime = TimeZoneExtensions.GetCurrentDate().AddMinutes(-15);
                    double clientHourDiff = TimeZoneExtensions.GetDateDiffClient(model.CurrentTime);

                    model.DateStart = model.DateStart.AddHours(clientHourDiff);
                    model.DateExpires = model.DateExpires.AddHours(clientHourDiff);

                    if (model.DateStart > currentTime && model.DateStart < currentTime.AddYears(2) &&
                        model.DateExpires > currentTime && model.DateExpires < currentTime.AddYears(2) &&
                        model.DateStart < model.DateExpires)
                    {
                        //Validate if user registerverification already exist and is active
                        bool accountLinkVerificationExist = _accountService.AccountLinkVerificationExist(currentUser.AccountEmail, model.Email);

                        if (!accountLinkVerificationExist)
                        {
                            bool isGuestAlreadyLinked = _accountService.IsGuestLinkedWithUser(model.Email, currentUser.AccountEmail);

                            if (!isGuestAlreadyLinked)
                            {
                                UsageDTOModel accountUsage = _accountService.GetAccountUsageGuestAccounts(currentUser.AccountEmail);

                                if (accountUsage.GuestAccountCount < accountUsage.AllowedGuestAccountCount)
                                {
                                    bool guestExist = _userService.GuestExist(model.Email);

                                    string password = Guid.NewGuid().ToString();
                                    string key = GuidExtensions.DateAndGuid();
                                    string accountLinkVerificationRowKey = currentUser.AccountID + "-" + key;

                                    AccountLinkVerificationModel accountLinkVerification = new AccountLinkVerificationModel(
                                        accountLinkVerificationRowKey, model.Email.ToLower().Trim(), key, string.Empty, string.Empty, password, true, guestExist,
                                        currentUser.AccountID, currentUser.AccountEmail, model.Permissions, model.DateStart, model.DateExpires);

                                    //Insert accountlink verification
                                    _accountService.InsertRegisterVerification(accountLinkVerification);

                                    //Send account link verification email
                                    _accountService.SendEmailAccountLinkVerification(accountLinkVerification);

                                    string accountLogrowKey = "accountlinkverification-" + currentUser.AccountOwnerID + "-" + key;

                                    LogModel accountLog = new LogModel(
                                        accountLogrowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                                        currentUser.IpAddress, Resource.Linking_of_guest_account_started);

                                    //Insert account log
                                    _accountService.InsertAccountLog(accountLog);

                                    message = Resource.An_activation_email_has_been_sent_to_the_specified_the_email_address;
                                    success = true;
                                }
                                else
                                    message = Resource.You_have_exceeded_the_allowed_number_of_guest_accounts;
                            }
                            else
                                message = Resource.The_specified_email_address_is_already_linked_with_your_account;
                        }
                        else
                            message = Resource.The_given_email_address_already_has_an_active_request;
                    }
                    else
                        message = Resource.Please_enter_correct_dates;
                }
                else
                    message = Resource.You_can_not_link_an_guestaccount_with_the_same_email_address;
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
        public ViewResult AccountLinkVerification(AccountLinkVerificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Get register verification
                AccountLinkVerificationModel accountLinkVerification = _accountService.GetAccountLinkVerification(model.AccountOwnerEmail, model.GuestEmail, model.Key);
                if (accountLinkVerification != null)
                {
                    //Validate expirytime on account link verification
                    DateTime currentTime = TimeZoneExtensions.GetCurrentDate();
                    if ((accountLinkVerification.DateExpiresVerification - currentTime).TotalDays > 0 && (accountLinkVerification.DateExpiresAccountLink - currentTime).TotalDays > 0)
                    {
                        int guestID = 0;
                        string ipAddress = IpAddressExtensions.GetIpAddress();
                        List<int> permissionIDs = new List<int>();

                        //Get json selected permissions and convert it to list of ints
                        if (!String.IsNullOrEmpty(accountLinkVerification.Permissions))
                        {
                            List<AccountLinkVerificationPermissionJSONModel> accountPermissions = JsonConvert.DeserializeObject<List<AccountLinkVerificationPermissionJSONModel>>(accountLinkVerification.Permissions);
                            if (accountPermissions != null && accountPermissions.Count > 0)
                            {
                                foreach (var item in accountPermissions)
                                {
                                    permissionIDs.Add(item.ID);
                                }
                            }
                        }

                        //Guest already exist - create/update accountlink
                        if (accountLinkVerification.GuestExist)
                        {
                            guestID = _userService.GetGuestID(accountLinkVerification.GuestEmail);

                            //Insert new or update existing account link
                            _accountService.InsertAccountLink(accountLinkVerification.UserID, guestID, permissionIDs, accountLinkVerification.DateCreated, accountLinkVerification.DateStartAccountLink, accountLinkVerification.DateExpiresAccountLink);

                            //Restore account if its deleted
                            _userService.RestoreGuest(guestID);

                            //Send success email
                            _accountService.SendEmailAccountLinkSuccess(accountLinkVerification);

                            ModelState.AddModelError("Success", Resource.Existing_guest_account_has_been_linked_with + " " + accountLinkVerification.UserEmail + ".");
                        }
                        //Guest doesn't exist - create new account and accountlink
                        else
                        {
                            //Validate if guest already exist
                            bool userExistGuest = _userService.GuestExist(accountLinkVerification.GuestEmail);
                            if (!userExistGuest)
                            {
                                SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();
                                string passwordHash = crypto.Compute(accountLinkVerification.Password);

                                GuestModel newUser = new GuestModel()
                                {
                                    Email = accountLinkVerification.GuestEmail,
                                    PasswordHash = passwordHash,
                                    PasswordSalt = crypto.Salt,
                                    FirstName = accountLinkVerification.FirstName,
                                    LastName = accountLinkVerification.LastName,
                                    DateRegistered = accountLinkVerification.DateCreated,
                                    IsDeleted = accountLinkVerification.IsDeleted,
                                    AgreeUserAgreement = accountLinkVerification.AgreeUserAgreement,
                                    DefaultEmailAddress = accountLinkVerification.GuestEmail,
                                    DefaultIpAddress = ipAddress,
                                    DateLastActive = accountLinkVerification.DateCreated
                                };

                                //Insert user
                                guestID = _userService.InsertGuest(newUser);

                                if (guestID > 0)
                                {
                                    //Insert new or update existing account link
                                    _accountService.InsertAccountLink(accountLinkVerification.UserID, guestID, permissionIDs, accountLinkVerification.DateCreated, accountLinkVerification.DateStartAccountLink, accountLinkVerification.DateExpiresAccountLink);

                                    //Send success email
                                    _accountService.SendEmailAccountLinkSuccess(accountLinkVerification);

                                    ModelState.AddModelError("Success", Resource.Guest_account_has_been_created_and_linked_with + " " + accountLinkVerification.UserEmail + ".");
                                }
                                else
                                    ModelState.AddModelError("Error", Resource.An_unexpected_error_has_occurred_Please_try_again);
                            }
                            else
                                ModelState.AddModelError("Error", Resource.Email_is_already_in_use);
                        }

                        //Delete account link verification
                        _accountService.DeleteAccountLinkVerification(accountLinkVerification.UserEmail, accountLinkVerification.GuestEmail);

                        string rowKey = "accountlinkverification-" + ipAddress.Replace(".", "") + "-" + GuidExtensions.DateAndGuid();

                        LogModel accountLog = new LogModel(
                            rowKey, accountLinkVerification.UserID, accountLinkVerification.UserEmail,
                            guestID, accountLinkVerification.UserEmail, Resource.User, ipAddress, Resource.Linking_of_guest_account_complete);

                        //Insert account log
                        _accountService.InsertAccountLog(accountLog);

                    }
                    else
                        ModelState.AddModelError("Error", Resource.The_key_to_activate_your_guest_account_has_expired_please_contact_the_account_owner);
                }
                else
                    ModelState.AddModelError("Error", Resource.The_key_to_activate_and_or_link_your_guest_account_was_not_found_please_contact_the_account_owner);
            }
            else
                ModelState.AddModelError("Error", Resource.An_unexpected_error_has_occurred_Please_try_again);

            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Linking_of_guest_account;
            ViewBag.MessageTitle = Resource.Linking_of_guest_account;

            return View("~/Views/Shared/Message.cshtml");
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizePermissionAttribute("FileWrite")]
        public JsonResult GetUploadSASURI(GetUploadSASURIViewModel model)
        {
            string message = string.Empty;
            bool success = false;
            string blobName = string.Empty;
            string blobSASURI = string.Empty;

            if (ModelState.IsValid)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                UsageDTOModel accountUsage = _accountService.GetAccountUsageFiles(currentUser.AccountOwnerEmail);

                //Validate file size storage
                if (model.FileSize + accountUsage.StoredBytes <= accountUsage.AllowedStoredBytes)
                {
                    //Validate file count storage
                    if (accountUsage.FileCount < accountUsage.AllowedFileCount)
                    {
                        blobName = currentUser.AccountID + "-" + Fildela.Web.Helpers.GuidExtensions.DateAndGuid();
                        blobSASURI = _accountService.GetUploadSASURI(blobName);

                        success = true;
                    }
                    else
                        message = Resource.You_have_exceeded_the_allowed_number_of_files;
                }
                else
                    message = Resource.You_have_exceeded_the_allowed_stored_bytes;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new { success = success, blobSASURI = blobSASURI, blobName = blobName, message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizePermissionAttribute("FileWrite")]
        public JsonResult InsertFile(InsertFileViewModel model)
        {
            string message = string.Empty;
            string storedBytesToString = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                bool blobExist = _accountService.BlobExist(model.BlobName);

                //Validate if blob was uploaded successfully
                if (blobExist)
                {
                    //Validate if anyone already owns the blob
                    bool isFileOwnedByAnyone = _accountService.IsFileOwnedByAnyone(model.BlobName);
                    if (!isFileOwnedByAnyone)
                    {
                        FileDTOModel fileDTOModel = _accountService.GetBlobAttributes(model.BlobName);
                        UsageDTOModel accountUsage = _accountService.GetAccountUsageFiles(currentUser.AccountOwnerEmail);

                        //Validate file count storage
                        if (accountUsage.FileCount < accountUsage.AllowedFileCount)
                        {
                            //Validate file size storage
                            if (fileDTOModel.Size + accountUsage.StoredBytes <= accountUsage.AllowedStoredBytes)
                            {
                                //Insert file meta data
                                FileModel file = new FileModel(fileDTOModel.Name, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail,
                                    Path.GetFileNameWithoutExtension(model.FileName), fileDTOModel.Size, Path.GetExtension(model.FileName).Replace(".", ""));

                                _accountService.InsertFile(file);

                                string rowKeyLog = "insertfile-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                                LogModel accountLog = new LogModel(
                                    rowKeyLog, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID,
                                    currentUser.AccountEmail, currentUser.UserType, currentUser.IpAddress, Resource.Upload_by_file);

                                //Insert account log
                                _accountService.InsertAccountLog(accountLog);

                                string rowKeyUsage = "upload-" + currentUser.AccountOwnerID + "-" + GuidExtensions.DateAndGuid();

                                UsageModel usage = new UsageModel(rowKeyUsage, currentUser.AccountOwnerEmail, "Upload");

                                //Insert usage
                                _accountService.InsertUsage(usage);

                                //Send back stored bytes to files model
                                long storedBytes = accountUsage.StoredBytes + fileDTOModel.Size;
                                storedBytesToString = UnitConverterExtensions.SubtractAndConvertByteToASuitableUnit(accountUsage.AllowedStoredBytes, storedBytes);

                                message = Resource.File_uploaded;
                                success = true;
                            }
                            else
                            {
                                if (!isFileOwnedByAnyone)
                                    _accountService.DeleteBlob(model.BlobName);

                                message = message = Resource.You_have_exceeded_the_allowed_stored_bytes;
                            }
                        }
                        else
                        {
                            if (!isFileOwnedByAnyone)
                                _accountService.DeleteBlob(model.BlobName);

                            message = Resource.You_have_exceeded_the_allowed_number_of_files;
                        }
                    }
                    else
                        message = Resource.You_have_no_rights_for_that_file;
                }
                else
                    message = Resource.An_unexpected_error_has_occurred_Please_try_again;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new { success = success, message = message, storedbytes = storedBytesToString }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxOnly]
        [AllowAnonymous]
        [AuthorizePermissionAttribute("FileRead")]
        public String DownloadFile(DownloadFileViewModel model)
        {
            string SASURI = String.Empty;

            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                FileModel file = _accountService.GetFile(model.BlobName, currentUser.AccountOwnerEmail);

                if (file != null)
                {
                    DateTime currentTime = TimeZoneExtensions.GetCurrentDate();
                    double DateBlobSASURLExpires = (file.DateBlobSASURLExpires - currentTime).TotalDays;

                    if (DateBlobSASURLExpires > 0 && !String.IsNullOrEmpty(file.BlobSASURI))
                    {
                        SASURI = file.BlobSASURI;
                    }
                    else
                    {
                        file.DateBlobSASURLExpires = currentTime.AddHours(1);

                        string newBloBSASURL = _accountService.GetDownloadSASURIForFile(model.BlobName, file.FileName, "." + file.FileExtension, 1);

                        file.BlobSASURI = newBloBSASURL;

                        SASURI = newBloBSASURL;
                    }

                    if ((currentTime - file.DateLastDownload.AddMinutes(15)).TotalDays > 0)
                    {
                        string rowKeyLog = "downloadfile-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                        LogModel accountLog = new LogModel(
                            rowKeyLog, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                            currentUser.IpAddress, Resource.Download_of_file);

                        //Insert account log
                        _accountService.InsertAccountLog(accountLog);

                        string rowKeyUsage = "download-" + currentUser.AccountOwnerID + "-" + GuidExtensions.DateAndGuid();

                        UsageModel usage = new UsageModel(rowKeyUsage, currentUser.AccountOwnerEmail, "Download");

                        //Insert usage
                        _accountService.InsertUsage(usage);

                        file.DateLastDownload = currentTime;
                    }

                    _accountService.UpdateFile(file);
                }
            }

            return SASURI;
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizePermissionAttribute("FileEdit")]
        public JsonResult UpdateFileName(UpdateFileNameViewModel model)
        {
            bool success = false;

            if (ModelState.IsValid)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                FileModel file = _accountService.GetFileFromRowKey(model.RowKey, currentUser.AccountOwnerEmail);

                if (file != null)
                {
                    DateTime currentTime = TimeZoneExtensions.GetCurrentDate();

                    if ((currentTime - file.DateLastModified.AddMinutes(15)).TotalDays > 0)
                    {
                        string rowKey = "updatefile-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                        LogModel accountLog = new LogModel(
                            rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                            currentUser.IpAddress, Resource.Editing_by_file);

                        //Insert account log
                        _accountService.InsertAccountLog(accountLog);

                        file.DateLastModified = currentTime;
                    }

                    file.FileName = model.FileName;
                    file.BlobSASURI = string.Empty;

                    _accountService.UpdateFile(file);

                    success = true;
                }
            }

            return Json(success);
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizePermissionAttribute("FileEdit")]
        public JsonResult DeleteFiles(DeleteFilesViewModel model)
        {
            string message = string.Empty;
            bool success = false;
            string storedBytesToString = string.Empty;

            if (model.RowKeys != null && model.RowKeys.Count() > 0)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                int fileCount = _accountService.DeleteFiles(model.RowKeys, currentUser.AccountOwnerEmail);
                int linkCount = _accountService.DeleteLinksWithBlobNames(model.RowKeys, currentUser.AccountOwnerEmail);

                if (fileCount > 0 || linkCount > 0)
                {
                    if (fileCount > 0)
                    {
                        string accountLogMessage = string.Empty;

                        if (linkCount == 0)
                            accountLogMessage = model.RowKeys.Count() == 1 ? Resource.Deleting_of_file : Resource.Deleting_of_files;
                        else
                            accountLogMessage = model.RowKeys.Count() == 1 ? Resource.Deleting_of_file_and_link : Resource.Deleting_of_files_and_links;

                        string rowKey = "deletefile-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                        LogModel accountLog = new LogModel(
                            rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                            currentUser.IpAddress, accountLogMessage);

                        //Insert account log
                        _accountService.InsertAccountLog(accountLog);
                    }

                    message = fileCount == 1 ? "[" + fileCount + "] " + Resource.File : "[" + fileCount + "] " + Resource.Files2;

                    if (linkCount > 0)
                    {
                        message += " " + Resource.And + " [" + linkCount + "] ";
                        message += linkCount == 1 ? Resource.Link + " " : Resource.Links2 + " ";
                        message += Resource.Has_been_deleted;
                    }
                    else
                        message += " " + Resource.Has_been_deleted;

                    //Send back stored bytes to files model
                    UsageDTOModel accountUsage = _accountService.GetAccountUsageFromCacheOrDB(currentUser.AccountOwnerEmail);
                    List<FileModel> files = _accountService.GetFiles(currentUser.AccountOwnerEmail).ToList();

                    long storedBytes = files.Sum(b => b.FileSizeBytes);
                    storedBytesToString = UnitConverterExtensions.SubtractAndConvertByteToASuitableUnit(accountUsage.AllowedStoredBytes, storedBytes);

                    success = true;
                }
                else
                {
                    message = Resource.The_selected_files_could_not_be_found_Please_try_again;

                    success = false;
                }
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success,
                storedbytes = storedBytesToString,
            });
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizePermissionAttribute("LinkEdit")]
        public JsonResult DeleteLinks(DeleteLinksViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (model.RowKeys != null && model.RowKeys.Count() > 0)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                int linkCount = _accountService.DeleteLinks(model.RowKeys, currentUser.AccountOwnerEmail);

                if (linkCount > 0)
                {
                    string accountLogMessage = model.RowKeys.Count() == 1 ? Resource.Deleting_of_link : Resource.Deleting_of_links;
                    string rowKey = "deletelink-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                    LogModel accountLog = new LogModel(
                        rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                        currentUser.IpAddress, accountLogMessage);

                    //Insert account log
                    _accountService.InsertAccountLog(accountLog);

                    message = linkCount == 1 ? "[" + linkCount + "] " + Resource.Link + " " + Resource.Has_been_deleted : "[" + linkCount + "] " + Resource.Links2 + " " + Resource.Has_been_deleted;

                    success = true;
                }
                else
                {
                    message = Resource.The_selected_links_could_not_be_found_Please_try_again;

                    success = false;
                }
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
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public JsonResult DeletePendingAccountLink(DeletePendingAccountLinkViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (model.GuestEmail != null && model.GuestEmail.Count() > 0)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                success = _accountService.DeleteAccountLinkVerification(currentUser.AccountEmail, model.GuestEmail);

                if (success)
                {
                    string rowKey = "deleteaccountlink-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                    LogModel accountLog = new LogModel(
                        rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                        currentUser.IpAddress, Resource.Deleting_of_guest_account);

                    //Insert account log
                    _accountService.InsertAccountLog(accountLog);

                    message = "[1] " + Resource.Guest_account + " " + Resource.Has_been_deleted;
                }
                else
                    message = Resource.The_selected_guest_accounts_could_not_be_found_Please_try_again;
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
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public JsonResult DeleteAccountLinks(DeleteAccountLinksViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (model.GuestIDs != null && model.GuestIDs.Count() > 0)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                int guestAccountCount = _accountService.DeleteAccountLinks(model.GuestIDs, currentUser.AccountID);

                if (guestAccountCount > 0)
                {
                    string accountLogMessage = model.GuestIDs.Count() == 1 ? Resource.Deleting_of_guest_account : Resource.Deleting_of_guest_accounts;
                    string rowKey = "deleteaccountlink-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                    LogModel accountLog = new LogModel(
                        rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                        currentUser.IpAddress, accountLogMessage);

                    //Insert account log
                    _accountService.InsertAccountLog(accountLog);

                    message = guestAccountCount == 1 ? "[" + guestAccountCount + "] " + Resource.Guest_account + " " + Resource.Has_been_deleted : "[" + guestAccountCount + "] " + Resource.Guest_accounts2 + " " + Resource.Has_been_deleted;

                    success = true;
                }
                else
                {
                    message = Resource.The_selected_guest_accounts_could_not_be_found_Please_try_again;

                    success = false;
                }
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
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim(ClaimTypes.Role, "AccountOwner")]
        public JsonResult DeleteLogs(DeleteLogsViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (model.RowKeys != null && model.RowKeys.Count > 0)
            {
                ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                int logCount = _accountService.DeleteLogs(model.RowKeys, currentUser.AccountEmail);

                if (logCount > 0)
                {
                    string accountLogMessage = model.RowKeys.Count() == 1 ? Resource.Deleting_of_log : Resource.Deleting_of_logs;
                    string rowKey = "deletelog-" + currentUser.AccountID + "-" + GuidExtensions.DateAndGuid();

                    LogModel accountLog = new LogModel(
                        rowKey, currentUser.AccountOwnerID, currentUser.AccountOwnerEmail, currentUser.AccountID, currentUser.AccountEmail, currentUser.UserType,
                        currentUser.IpAddress, accountLogMessage);

                    //Insert account log
                    _accountService.InsertAccountLog(accountLog);

                    message = logCount == 1 ? "[" + logCount + "] " + Resource.Log2 + " " + Resource.Has_been_deleted : "[" + logCount + "] " + Resource.Logs2 + " " + Resource.Has_been_deleted;

                    success = true;
                }
                else
                {
                    message = Resource.The_selected_logs_could_not_be_found_Please_try_again;

                    success = false;
                }
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success
            });
        }
    }
}
