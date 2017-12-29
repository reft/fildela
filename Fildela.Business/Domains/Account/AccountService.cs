using Fildela.Data.Repositories.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fildela.Business.Domains.Account.Models;
using Fildela.Business.Domains.User;
using Newtonsoft.Json;
using Fildela.Business.Helpers;
using Fildela.Resources;
using System.Configuration;

namespace Fildela.Business.Domains.Account
{
    public class AccountService : IAccountService
    {
        private readonly string BaseAddress = ConfigurationManager.AppSettings["BaseAddress"];
        private readonly string UserAgreementAddress = ConfigurationManager.AppSettings["BaseAddress"] + ConfigurationManager.AppSettings["UserAgreementAddress"];
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];

        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void DeleteAllFiles(string userEmail)
        {
            _accountRepository.DeleteAllFiles(userEmail);
        }

        public void DeleteAllLinks(string userEmail)
        {
            _accountRepository.DeleteAllLinks(userEmail);
        }

        public void DeleteAllLogs(string userEmail)
        {
            _accountRepository.DeleteAllLogs(userEmail);
        }

        public UsageDTOModel GetAccountUsageFromCacheOrDB(string userEmail)
        {
            return _accountRepository.GetAccountUsageFromCacheOrDB(userEmail).ToModel();
        }

        public UsageAndStatisticsDTOModel GetAccountUsageAndStatistics(string userEmail)
        {
            return _accountRepository.GetAccountUsageAndStatistics(userEmail).ToModel();
        }

        public UsageDTOModel GetAccountUsageFiles(string userEmail)
        {
            return _accountRepository.GetAccountUsageFiles(userEmail).ToModel();
        }

        public UsageDTOModel GetAccountUsageLinks(string userEmail)
        {
            return _accountRepository.GetAccountUsageLinks(userEmail).ToModel();
        }

        public UsageDTOModel GetAccountUsageGuestAccounts(string userEmail)
        {
            return _accountRepository.GetAccountUsageGuestAccounts(userEmail).ToModel();
        }

        public string GetUploadSASURI(string blobname)
        {
            return _accountRepository.GetUploadSASURI(blobname);
        }

        public string GetDownloadSASURIForFile(string blobName, string fileName, string fileExtension, int ExpiredInHours)
        {
            return _accountRepository.GetDownloadSASURIForFile(blobName, fileName, fileExtension, ExpiredInHours);
        }

        public string GetDownloadSASURIForLink(string blobName, string fileName, string fileExtension, int ExpiredInHours)
        {
            return _accountRepository.GetDownloadSASURIForLink(blobName, fileName, fileExtension, ExpiredInHours);
        }

        public FileDTOModel GetBlobAttributes(string blobName)
        {
            return _accountRepository.GetBlobAttributes(blobName).ToModel();
        }

        public void InsertFile(FileModel file)
        {
            _accountRepository.InsertFile(file.ToEntity());
        }

        public void InsertLink(LinkModel link)
        {
            _accountRepository.InsertLink(link.ToEntity());
        }

        public void InsertUsage(UsageModel usage)
        {
            _accountRepository.InsertUsage(usage.ToEntity());
        }

        public void DeleteBlob(string blobName)
        {
            _accountRepository.DeleteBlob(blobName);
        }

        public bool IsFileOwnedByAnyone(string blobName)
        {
            return _accountRepository.IsFileOwnedByAnyone(blobName);
        }

        public void InsertRegisterVerification(AccountLinkVerificationModel accountLinkVerificationEntity)
        {
            _accountRepository.InsertRegisterVerification(accountLinkVerificationEntity.ToEntity());
        }

        public bool AccountLinkVerificationExist(string userEmail, string guestEmail)
        {
            return _accountRepository.AccountLinkVerificationExist(userEmail, guestEmail);
        }

        public AccountLinkVerificationModel GetAccountLinkVerification(string userEmail, string guestEmail, string key)
        {
            return _accountRepository.GetAccountLinkVerification(userEmail, guestEmail, key).ToModel();
        }

        public bool DeleteAccountLinkVerification(string userEmail, string guestEmail)
        {
            return _accountRepository.DeleteAccountLinkVerification(userEmail, guestEmail);
        }

        public IEnumerable<LogModel> GetLogs(string userEmail)
        {
            return _accountRepository.GetLogs(userEmail).Select(o => o.ToModel());
        }

        public void InsertAccountLog(LogModel accountLog)
        {
            _accountRepository.InsertAccountLog(accountLog.ToEntity());
        }

        public IEnumerable<User.Models.GuestDTOModel> GetGuestsLinkedWithUser(int userID)
        {
            return _accountRepository.GetGuestsLinkedWithUser(userID).Select(o => o.ToModel());
        }

        public IEnumerable<PermissionModel> GetPermissions()
        {
            return _accountRepository.GetPermissions().Select(o => o.ToModel());
        }

        public bool IsGuestLinkedWithUser(string guestEmail, string userEmail)
        {
            return _accountRepository.IsGuestLinkedWithUser(guestEmail, userEmail);
        }

        public bool IsGuestLinkExpired(int guestID, int userID)
        {
            return _accountRepository.IsGuestLinkExpired(guestID, userID);
        }

        public void UpdateAccountLink(int userID, int guestID, IEnumerable<int> permissionIDs, DateTime dateStart, DateTime dateExpires)
        {
            _accountRepository.UpdateAccountLink(userID, guestID, permissionIDs, dateStart, dateExpires);
        }

        public void InsertAccountLink(int userID, int guestID, IEnumerable<int> permissionIDs, DateTime dateCreated, DateTime dateStart, DateTime dateExpires)
        {
            _accountRepository.InsertAccountLink(userID, guestID, permissionIDs, dateCreated, dateStart, dateExpires);
        }

        public void DeleteAllAccountLinksGuest(int guestID)
        {
            _accountRepository.DeleteAllAccountLinksGuest(guestID);
        }

        public void DeleteAllAccountLinksUser(int userID)
        {
            _accountRepository.DeleteAllAccountLinksUser(userID);
        }

        public IEnumerable<AccountLinkPermissionModel> GetAccountLinkPermissions(int userID, int guestID)
        {
            return _accountRepository.GetAccountLinkPermissions(userID, guestID).Select(o => o.ToModel());
        }

        public AccountLinkPermissionBoolDTOModel GetAccountLinkPermissionsBool(int userID, int guestID)
        {
            return _accountRepository.GetAccountLinkPermissionsBool(userID, guestID).ToModel();
        }

        public bool BlobExist(string blobName)
        {
            return _accountRepository.BlobExist(blobName);
        }

        public IEnumerable<FileModel> GetFiles(string userEmail)
        {
            return _accountRepository.GetFiles(userEmail).Select(o => o.ToModel());
        }

        public IEnumerable<LinkModel> GetLinks(string userEmail)
        {
            return _accountRepository.GetLinks(userEmail).Select(o => o.ToModel());
        }

        public FileModel GetFile(string blobName, string userEmail)
        {
            return _accountRepository.GetFile(blobName, userEmail).ToModel();
        }

        public FileModel GetFileFromRowKey(string rowKey, string userEmail)
        {
            return _accountRepository.GetFileFromRowKey(rowKey, userEmail).ToModel();
        }

        public LinkModel GetLinkFromRowKey(string rowKey)
        {
            return _accountRepository.GetLinkFromRowKey(rowKey).ToModel();
        }

        public LinkModel GetActiveLinkFromRowKey(string rowKey)
        {
            return _accountRepository.GetActiveLinkFromRowKey(rowKey).ToModel();
        }

        public void UpdateFile(FileModel file)
        {
            _accountRepository.UpdateFile(file.ToEntity());
        }

        public void UpdateLink(LinkModel link)
        {
            _accountRepository.UpdateLink(link.ToEntity());
        }

        public int DeleteFiles(IEnumerable<string> rowKeys, string userEmail)
        {
            return _accountRepository.DeleteFiles(rowKeys, userEmail);
        }

        public int DeleteLinks(IEnumerable<string> rowKeys, string userEmail)
        {
            return _accountRepository.DeleteLinks(rowKeys, userEmail);
        }

        public int DeleteLinksWithBlobNames(IEnumerable<string> blobNames, string userEmail)
        {
            return _accountRepository.DeleteLinksWithBlobNames(blobNames, userEmail);
        }

        public int DeleteAccountLinks(IEnumerable<int> guestIDs, int userID)
        {
            return _accountRepository.DeleteAccountLinks(guestIDs, userID);
        }

        public int DeleteLogs(IEnumerable<string> rowKeys, string userEmail)
        {
            return _accountRepository.DeleteLogs(rowKeys, userEmail);
        }

        public IEnumerable<AccountLinkVerificationModel> GetPendingGuestsLinkedWithUser(string userEmail)
        {
            return _accountRepository.GetPendingGuestsLinkedWithUser(userEmail).Select(o => o.ToModel());
        }

        public UsageDTOModel GetAllowedAccountUsage(string userEmail)
        {
            return _accountRepository.GetAllowedAccountUsage(userEmail).ToModel();
        }

        public int GetGuestCountLinkedWithUser(string userEmail)
        {
            return _accountRepository.GetGuestCountLinkedWithUser(userEmail);
        }

        public void SendEmailAccountLinkVerification(AccountLinkVerificationModel accountLinkVerification)
        {
            StringBuilder sbEmailBody = new StringBuilder();

            if (!accountLinkVerification.GuestExist)
            {
                sbEmailBody.Append(Resource.Welcome_to + " " + ProductNameWithDomain + "!<br/><br/>");

                sbEmailBody.Append(Resource.You_have_been_assigned_a_guest_account_from + " <b>" + BusinessStringExtensions.FirstCharToUpper(accountLinkVerification.UserEmail) + ".</b><br/><br/>");
            }
            else
                sbEmailBody.Append(Resource.Your_guest_account_has_been_given_access_to + " <b>" + BusinessStringExtensions.FirstCharToUpper(accountLinkVerification.UserEmail) + "</b>'s " + Resource.Useraccount + "<br/><br/>");

            sbEmailBody.Append("<b>" + Resource.Rights + ":</b><br/>");

            if (!String.IsNullOrEmpty(accountLinkVerification.Permissions))
            {
                //Get json selected permissions and convert it to list of ints
                List<AccountLinkVerificationPermissionJSONModel> accountPermissions = JsonConvert.DeserializeObject<List<AccountLinkVerificationPermissionJSONModel>>(accountLinkVerification.Permissions);
                if (accountPermissions != null && accountPermissions.Count > 0)
                {
                    foreach (var item in accountPermissions)
                    {
                        sbEmailBody.Append(item.Text + "<br/>");
                    }

                    sbEmailBody.Append("<br/>");
                }
            }

            else
            {
                sbEmailBody.Append(Resource.You_have_not_yet_been_assigned_any_rights + "<br/><br/>");
            }

            sbEmailBody.Append("<b>" + Resource.Start_date + ":</b><br/>" + accountLinkVerification.DateStartAccountLink.ToString("yyyy-MM-dd HH:mm") + "<br/>");
            sbEmailBody.Append("<b>" + Resource.End_date + ":</b><br/>" + accountLinkVerification.DateExpiresAccountLink.ToString("yyyy-MM-dd HH:mm") + "<br/><br/>");

            if (!accountLinkVerification.GuestExist)
            {
                sbEmailBody.Append(Resource.Click_on_the_link_below_to_active_your_guestaccount + "<br/>");
            }
            else
            {
                sbEmailBody.Append(Resource.Click_on_the_link_below_to_active_your_access + "<br/>");
            }

            sbEmailBody.Append("<a href=\"" + BaseAddress + "/account/accountlinkverification?accountowneremail=" +
                accountLinkVerification.UserEmail.ToLower().Trim() +
                "&guestemail=" + accountLinkVerification.GuestEmail +
                "&key=" + accountLinkVerification.Key +
                "\" style=\"text-decoration:none;\">" + BaseAddress + "/account/accountlinkverification?accountowneremail=" +
                @accountLinkVerification.UserEmail.ToLower().Trim() +
                "&guestemail=" + @accountLinkVerification.GuestEmail +
                "&key=" + @accountLinkVerification.Key + "</a>");

            if (!accountLinkVerification.GuestExist)
            {
                sbEmailBody.Append("<br/><br/>" + Resource.After_you_have_clicked_on_the_link_and_activated_your_guest_account_an_email_will_be_sent_to_you_with_new_login_information);
                sbEmailBody.Append("<br/>" + Resource.When_you_active_your_useraccount_you_automatically_accept_our_useragreement + @" <a href=""" + UserAgreementAddress + @""" style=""text-decoration:none;"">" + Resource.User_agreement2 + "</a>.");
            }

            sbEmailBody.Append(@"<br/><br/><a href=""" + BaseAddress + @""" style=""text-decoration:none;"">" + ProductNameWithDomain + "</a>");

            string subject = string.Empty;
            if (!accountLinkVerification.GuestExist)
                subject = ProductNameWithDomain + " - " + Resource.Activate_and_link_your_guestaccount;
            else
                subject = ProductNameWithDomain + " - " + Resource.Link_your_guestaccount;

            BusinessSMTPExtensions.SendEmail(subject, accountLinkVerification.GuestEmail.ToLower().Trim(), sbEmailBody.ToString(), false);
        }

        public void SendEmailAccountLinkSuccess(AccountLinkVerificationModel accountLinkVerification)
        {
            StringBuilder sbEmailBody = new StringBuilder();

            if (!accountLinkVerification.GuestExist)
            {
                sbEmailBody.Append(Resource.Welcome_to + " " + ProductNameWithDomain + "!<br/><br/>");
                sbEmailBody.Append(Resource.Your_guest_account_has_been_activated_and_linked_with + " <b>" + BusinessStringExtensions.FirstCharToUpper(accountLinkVerification.UserEmail) + ".</b><br/><br/>");

                sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.Username2 + "</span><br/>" + BusinessStringExtensions.FirstCharToUpper(accountLinkVerification.GuestEmail) + "<br/>");
                sbEmailBody.Append("<span style=\"font-weight:bold;\">" + Resource.Password2 + "</span><br/>" + accountLinkVerification.Password + "<br/><br/>");

                sbEmailBody.Append(Resource.Click_on_the_link_below_to_sign_in_and_change_to_a_new_password + "<br/>");

                sbEmailBody.Append("<a href=\"" + BaseAddress + "/account/settings" + "\"" +
                " style=\"text-decoration:none;\">" + BaseAddress + "/account/settings" + "</a>");
            }
            else
            {
                sbEmailBody.Append(Resource.Your_guest_account_has_been_linked_with + " <b>" + BusinessStringExtensions.FirstCharToUpper(accountLinkVerification.UserEmail) + ".</b>");
            }

            sbEmailBody.Append("<br/><br/><a href=\"" + BaseAddress + "\"" +
    " style=\"text-decoration:none;\">" + "Fildela.se</a>");

            string subject = string.Empty;
            if (!accountLinkVerification.GuestExist)
                subject = ProductNameWithDomain + " - " + Resource.Guest_account_has_been_activated_and_linked;
            else
                subject = ProductNameWithDomain + " - " + Resource.Guest_account_has_been_linked;

            BusinessSMTPExtensions.SendEmail(subject, accountLinkVerification.GuestEmail, sbEmailBody.ToString(), false);
        }
    }
}
