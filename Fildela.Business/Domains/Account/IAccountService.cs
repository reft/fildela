using Fildela.Business.Domains.Account.Models;
using Fildela.Business.Domains.User.Models;
using System;
using System.Collections.Generic;

namespace Fildela.Business.Domains.Account
{
    public interface IAccountService
    {
        void DeleteAllFiles(string userEmail);

        void DeleteAllLinks(string userEmail);

        void DeleteAllLogs(string userEmail);

        UsageDTOModel GetAccountUsageFromCacheOrDB(string userEmail);

        UsageAndStatisticsDTOModel GetAccountUsageAndStatistics(string userEmail);

        UsageDTOModel GetAccountUsageFiles(string userEmail);

        UsageDTOModel GetAccountUsageLinks(string userEmail);

        UsageDTOModel GetAccountUsageGuestAccounts(string userEmail);

        string GetUploadSASURI(string blobname);

        string GetDownloadSASURIForFile(string blobName, string fileName, string fileExtension, int ExpiredInHours);

        string GetDownloadSASURIForLink(string blobName, string fileName, string fileExtension, int ExpiredInHours);

        FileDTOModel GetBlobAttributes(string blobName);

        void InsertFile(FileModel file);

        void InsertLink(LinkModel link);

        void InsertUsage(UsageModel usage);

        void DeleteBlob(string blobName);

        bool IsFileOwnedByAnyone(string blobName);

        void InsertRegisterVerification(AccountLinkVerificationModel accountLinkVerificationEntity);

        bool AccountLinkVerificationExist(string userEmail, string guestEmail);

        AccountLinkVerificationModel GetAccountLinkVerification(string userEmail, string guestEmail, string key);

        bool DeleteAccountLinkVerification(string userEmail, string guestEmail);

        IEnumerable<LogModel> GetLogs(string userEmail);

        void InsertAccountLog(LogModel accountLog);

        IEnumerable<GuestDTOModel> GetGuestsLinkedWithUser(int userID);

        IEnumerable<PermissionModel> GetPermissions();

        bool IsGuestLinkedWithUser(string guestEmail, string userEmail);

        bool IsGuestLinkExpired(int guestID, int userID);

        void UpdateAccountLink(int userID, int guestID, IEnumerable<int> permissionIDs, DateTime dateStart, DateTime dateExpires);

        void InsertAccountLink(int userID, int guestID, IEnumerable<int> permissionIDs, DateTime dateCreated, DateTime dateStart, DateTime dateExpires);

        void DeleteAllAccountLinksGuest(int guestID);

        void DeleteAllAccountLinksUser(int userID);

        IEnumerable<AccountLinkPermissionModel> GetAccountLinkPermissions(int userID, int guestID);

        AccountLinkPermissionBoolDTOModel GetAccountLinkPermissionsBool(int userID, int guestID);

        bool BlobExist(string blobName);

        IEnumerable<FileModel> GetFiles(string userEmail);

        IEnumerable<LinkModel> GetLinks(string userEmail);

        FileModel GetFile(string blobName, string userEmail);

        FileModel GetFileFromRowKey(string rowKey, string userEmail);

        LinkModel GetLinkFromRowKey(string rowKey);

        LinkModel GetActiveLinkFromRowKey(string rowKey);

        void UpdateFile(FileModel file);

        void UpdateLink(LinkModel link);

        int DeleteFiles(IEnumerable<String> rowKeys, string userEmail);

        int DeleteLinks(IEnumerable<String> rowKeys, string userEmail);

        int DeleteLinksWithBlobNames(IEnumerable<String> blobNames, string userEmail);

        int DeleteAccountLinks(IEnumerable<int> guestIDs, int userID);

        int DeleteLogs(IEnumerable<String> rowKeys, string userEmail);

        IEnumerable<AccountLinkVerificationModel> GetPendingGuestsLinkedWithUser(string userEmail);

        UsageDTOModel GetAllowedAccountUsage(string userEmail);

        int GetGuestCountLinkedWithUser(string userEmail);

        void SendEmailAccountLinkVerification(AccountLinkVerificationModel accountLinkVerification);

        void SendEmailAccountLinkSuccess(AccountLinkVerificationModel accountLinkVerification);
    }
}
