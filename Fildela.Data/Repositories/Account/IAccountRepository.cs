using Fildela.Data.CustomModels.Account;
using Fildela.Data.CustomModels.User;
using Fildela.Data.Database.Models;
using Fildela.Data.Storage.Models;
using System;
using System.Collections.Generic;

namespace Fildela.Data.Repositories.Account
{
    public interface IAccountRepository
    {
        void DeleteAllFiles(string userEmail);

        void DeleteAllLinks(string userEmail);

        void DeleteAllLogs(string userEmail);

        UsageDTO GetAccountUsageFromCacheOrDB(string userEmail);

        UsageAndStatisticsDTO GetAccountUsageAndStatistics(string userEmail);

        UsageDTO GetAccountUsageFiles(string userEmail);

        UsageDTO GetAccountUsageLinks(string userEmail);

        UsageDTO GetAccountUsageGuestAccounts(string userEmail);

        string GetUploadSASURI(string blobname);

        string GetDownloadSASURIForFile(string blobName, string fileName, string fileExtension, int ExpiredInHours);

        string GetDownloadSASURIForLink(string blobName, string fileName, string fileExtension, int ExpiredInHours);

        FileDTO GetBlobAttributes(string blobName);

        void InsertFile(File file);

        void InsertLink(Link link);

        void InsertUsage(Usage usage);

        void DeleteBlob(string blobName);

        bool IsFileOwnedByAnyone(string blobName);

        void InsertRegisterVerification(AccountLinkVerification accountLinkVerificationEntity);

        bool AccountLinkVerificationExist(string userEmail, string guestEmail);

        AccountLinkVerification GetAccountLinkVerification(string userEmail, string guestEmail, string key);

        bool DeleteAccountLinkVerification(string userEmail, string guestEmail);

        IEnumerable<Log> GetLogs(string userEmail);

        void InsertAccountLog(Log accountLog);

        IEnumerable<GuestDTO> GetGuestsLinkedWithUser(int userID);

        IEnumerable<Permission> GetPermissions();

        bool IsGuestLinkedWithUser(string guestEmail, string userEmail);

        bool IsGuestLinkExpired(int guestID, int userID);

        void UpdateAccountLink(int userID, int guestID, IEnumerable<int> permissionIDs, DateTime dateStart, DateTime dateExpires);

        void InsertAccountLink(int userID, int guestID, IEnumerable<int> permissionIDs, DateTime dateCreated, DateTime dateStart, DateTime dateExpires);

        void DeleteAllAccountLinksGuest(int guestID);

        void DeleteAllAccountLinksUser(int userID);

        IEnumerable<AccountLinkPermission> GetAccountLinkPermissions(int userID, int guestID);

        AccountLinkPermissionBoolDTO GetAccountLinkPermissionsBool(int userID, int guestID);

        bool BlobExist(string blobName);

        IEnumerable<File> GetFiles(string userEmail);

        IEnumerable<Link> GetLinks(string userEmail);

        File GetFile(string blobName, string userEmail);

        File GetFileFromRowKey(string rowKey, string userEmail);

        Link GetLinkFromRowKey(string rowKey);

        Link GetActiveLinkFromRowKey(string rowKey);

        void UpdateFile(File file);

        void UpdateLink(Link link);

        int DeleteFiles(IEnumerable<String> rowKeys, string userEmail);

        int DeleteLinks(IEnumerable<String> rowKeys, string userEmail);

        int DeleteLinksWithBlobNames(IEnumerable<String> blobNames, string userEmail);

        int DeleteAccountLinks(IEnumerable<int> guestIDs, int userID);

        int DeleteLogs(IEnumerable<String> rowKeys, string userEmail);

        IEnumerable<AccountLinkVerification> GetPendingGuestsLinkedWithUser(string userEmail);

        UsageDTO GetAllowedAccountUsage(string userEmail);

        int GetGuestCountLinkedWithUser(string userEmail);
    }
}
