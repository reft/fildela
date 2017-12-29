using Fildela.Data.CustomModels.Account;
using Fildela.Data.CustomModels.User;
using Fildela.Data.Database.DataLayer;
using Fildela.Data.Database.Models;
using Fildela.Data.Helpers;
using Fildela.Data.Storage.Models;
using Fildela.Data.Storage.Services;
using Microsoft.ApplicationServer.Caching;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;

namespace Fildela.Data.Repositories.Account
{
    public class AccountRepository : RepositoryBase, IAccountRepository
    {
        private readonly string AccountAllowedFileCount = ConfigurationManager.AppSettings["AccountAllowedFileCount"];
        private readonly string AccountAllowedGuestCount = ConfigurationManager.AppSettings["AccountAllowedGuestCount"];
        private readonly string AccountAllowedLinkCount = ConfigurationManager.AppSettings["AccountAllowedLinkCount"];
        private readonly string AccountAllowedStoredBytes = ConfigurationManager.AppSettings["AccountAllowedStoredBytes"];

        public AccountRepository(DataLayer db, CloudStorageServices storage) : base(db, storage) { }

        public void DeleteAllFiles(string userEmail)
        {
            CloudTable fileTable = Storage.GetCloudFileTable();

            TableQuery<File> fileQuery = new TableQuery<File>();
            List<File> fileEntities = (from e in fileTable.ExecuteQuery(fileQuery)
                                       where e.PartitionKey == "files" && e.UserEmail == userEmail.ToLower().Trim()
                                       select e).ToList();

            //BatchOperation only takes 100 entities at a time
            int deleteCount = 0;
            while (deleteCount < fileEntities.Count())
            {
                List<File> fileEntities100 = fileEntities.Skip(deleteCount).Take(100).ToList();

                TableBatchOperation batchOperation = new TableBatchOperation();

                CloudBlobContainer blobContainer = Storage.GetCloudBlobFileContainer();

                foreach (File item in fileEntities100)
                {
                    batchOperation.Delete(item);

                    //Delete blob entity
                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(item.RowKey);
                    if (blob.Exists())
                    {
                        blob.Delete();
                    }
                }

                //Delete table entities
                if (batchOperation.Count > 0)
                    fileTable.ExecuteBatch(batchOperation);

                deleteCount += 100;
            }
        }

        public void DeleteAllLinks(string userEmail)
        {
            CloudTable linkTable = Storage.GetCloudLinkTable();

            TableQuery<Link> linkQuery = new TableQuery<Link>();
            List<Link> linkEntities = (from e in linkTable.ExecuteQuery(linkQuery)
                                       where e.PartitionKey == "links" && e.UserEmail == userEmail.ToLower().Trim()
                                       select e).ToList();

            //BatchOperation only takes 100 entities at a time
            int deleteCount = 0;
            while (deleteCount < linkEntities.Count())
            {
                List<Link> linkEntities100 = linkEntities.Skip(deleteCount).Take(100).ToList();

                TableBatchOperation batchOperation = new TableBatchOperation();

                foreach (Link item in linkEntities100)
                {
                    batchOperation.Delete(item);
                }

                //Delete table entities
                if (batchOperation.Count > 0)
                    linkTable.ExecuteBatch(batchOperation);

                deleteCount += 100;
            }
        }

        public void DeleteAllLogs(string userEmail)
        {
            CloudTable logTable = Storage.GetCloudLogTable();

            TableQuery<Log> logQuery = new TableQuery<Log>();
            List<Log> logEntities = (from e in logTable.ExecuteQuery(logQuery)
                                     where e.PartitionKey == "logs" && e.AccountOwnerEmail == userEmail.ToLower().Trim()
                                     select e).ToList();

            //BatchOperation only takes 100 entities at a time
            int deleteCount = 0;
            while (deleteCount < logEntities.Count())
            {
                TableBatchOperation batchOperation = new TableBatchOperation();

                List<Log> logEntities100 = logEntities.Skip(deleteCount).Take(100).ToList();

                foreach (Log item in logEntities100)
                {
                    batchOperation.Delete(item);
                }

                //Delete table entities
                if (batchOperation.Count > 0)
                    logTable.ExecuteBatch(batchOperation);

                deleteCount += 100;
            }
        }


        public UsageDTO GetAccountUsageFromCacheOrDB(string userEmail)
        {
            DataCache cache = new DataCache("default");
            object cacheAccountUsage = cache.Get("cacheAccountUsage");

            UsageDTO accountUsage = new UsageDTO();

            if (cacheAccountUsage == null)
            {
                DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

                accountUsage = GetAllowedAccountUsage(userEmail.ToLower().Trim());

                //Get active guestaccounts
                int guestAccounts = GetGuestCountLinkedWithUser(userEmail.ToLower().Trim());

                //Get pending guestaccounts
                CloudTable accountLinkVerificationTable = Storage.GetCloudAccountLinkVerificationTable();
                TableQuery<AccountLinkVerification> accountLinkVerificationQuery = accountLinkVerificationTable.CreateQuery<AccountLinkVerification>();
                int guestAccountsInvites = (from alvc in accountLinkVerificationTable.ExecuteQuery(accountLinkVerificationQuery)
                                            where alvc.PartitionKey == "accountlinkverifications" &&
                                            alvc.AccountOwnerEmail == userEmail &&
                                            (alvc.DateExpiresVerification - currentTime).TotalDays > 0
                                            select alvc).Count();

                accountUsage.GuestAccountCount = guestAccounts + guestAccountsInvites;

                CloudTable fileTable = Storage.GetCloudFileTable();
                TableQuery<File> fileQuery = new TableQuery<File>();
                List<File> files = (from e in fileTable.ExecuteQuery(fileQuery)
                                    where e.PartitionKey == "files" && e.UserEmail == userEmail.ToLower().Trim()
                                    select e).ToList();

                accountUsage.FileCount = files.Count();
                accountUsage.StoredBytes = files.Sum(b => b.FileSizeBytes);

                CloudTable linkTable = Storage.GetCloudLinkTable();
                TableQuery<Link> linkQuery = new TableQuery<Link>();
                accountUsage.LinkCount = (from e in linkTable.ExecuteQuery(linkQuery)
                                          where e.PartitionKey == "links" && e.UserEmail == userEmail.ToLower().Trim() &&
                                          (e.DateExpires - currentTime).TotalDays > 0
                                          select e).Count();

                CloudTable logTable = Storage.GetCloudLogTable();
                TableQuery<Log> logQuery = new TableQuery<Log>();
                accountUsage.LogCount = (from e in logTable.ExecuteQuery(logQuery)
                                         where e.PartitionKey == "logs" && e.AccountOwnerEmail == userEmail.ToLower().Trim()
                                         select e).Count();

                cache.Remove("cacheAccountUsage");
                cache.Add("cacheAccountUsage", accountUsage);
            }
            else
                accountUsage = (UsageDTO)cacheAccountUsage;

            return accountUsage;
        }

        public UsageAndStatisticsDTO GetAccountUsageAndStatistics(string userEmail)
        {
            UsageAndStatisticsDTO accountUsageAndStatistics = new UsageAndStatisticsDTO() { AccountStatisticsDTO = new StatisticsDTO() };

            UsageDTO accountUsage = new UsageDTO();

            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            accountUsage = GetAllowedAccountUsage(userEmail.ToLower().Trim());

            //Get active guestaccounts
            int guestAccounts = GetGuestCountLinkedWithUser(userEmail.ToLower().Trim());

            //Get pending guestaccounts
            CloudTable accountLinkVerificationTable = Storage.GetCloudAccountLinkVerificationTable();
            TableQuery<AccountLinkVerification> accountLinkVerificationQuery = accountLinkVerificationTable.CreateQuery<AccountLinkVerification>();
            int guestAccountsInvites = (from alvc in accountLinkVerificationTable.ExecuteQuery(accountLinkVerificationQuery)
                                        where alvc.PartitionKey == "accountlinkverifications" &&
                                        alvc.AccountOwnerEmail == userEmail &&
                                        (alvc.DateExpiresVerification - currentTime).TotalDays > 0
                                        select alvc).Count();

            accountUsage.GuestAccountCount = guestAccounts + guestAccountsInvites;

            //Get files
            CloudTable fileTable = Storage.GetCloudFileTable();
            TableQuery<File> fileQuery = new TableQuery<File>();
            List<File> files = (from e in fileTable.ExecuteQuery(fileQuery)
                                where e.PartitionKey == "files" && e.UserEmail == userEmail.ToLower().Trim()
                                select e).ToList();

            accountUsage.FileCount = files.Count();
            accountUsage.StoredBytes = files.Sum(b => b.FileSizeBytes);

            //Get links
            CloudTable linkTable = Storage.GetCloudLinkTable();
            TableQuery<Link> linkQuery = new TableQuery<Link>();
            accountUsage.LinkCount = (from e in linkTable.ExecuteQuery(linkQuery)
                                      where e.PartitionKey == "links" && e.UserEmail == userEmail.ToLower().Trim() &&
                                      (e.DateExpires - currentTime).TotalDays > 0
                                      select e).Count();

            //Get logs
            CloudTable logTable = Storage.GetCloudLogTable();
            TableQuery<Log> logQuery = new TableQuery<Log>();
            accountUsage.LogCount = (from e in logTable.ExecuteQuery(logQuery)
                                     where e.PartitionKey == "logs" && e.AccountOwnerEmail == userEmail.ToLower().Trim()
                                     select e).Count();

            //Get data for stat istics
            CloudTable usageTable = Storage.GetCloudUsageTable();
            TableQuery<Usage> usageQuery = new TableQuery<Usage>();
            List<Usage> allUsage = (from u in usageTable.ExecuteQuery(usageQuery)
                                    where u.PartitionKey == "usage" &&
                                    u.UserEmail == userEmail.ToLower().Trim() &&
                                    (u.DateExpires - currentTime).TotalDays > 0
                                    select u).ToList();

            List<int> xAxis = new List<int>();

            List<StatisticSeriesDTO> series = new List<StatisticSeriesDTO>();

            StatisticSeriesDTO loginSerie = new StatisticSeriesDTO() { Name = "SignIns", Data = new List<int>() };
            StatisticSeriesDTO uploadSerie = new StatisticSeriesDTO() { Name = "Uploads", Data = new List<int>() };
            StatisticSeriesDTO downloadSerie = new StatisticSeriesDTO() { Name = "Downloads", Data = new List<int>() };

            for (int i = -29; i < 1; i++)
            {
                int day = currentTime.AddDays(i).Day;
                int month = currentTime.AddDays(i).Month;

                //Days
                xAxis.Add(day);

                //Logins
                int logins = allUsage.Where(m => m.Type == "SignIn" && m.DateTriggered.Day == day && m.DateTriggered.Month == month).Count();
                loginSerie.Data.Add(logins);

                //Uploads
                int uploads = allUsage.Where(m => m.Type == "Upload" && m.DateTriggered.Day == day && m.DateTriggered.Month == month).Count();
                uploadSerie.Data.Add(uploads);

                //Downloads
                int downloads = allUsage.Where(m => m.Type == "Download" && m.DateTriggered.Day == day && m.DateTriggered.Month == month).Count();
                downloadSerie.Data.Add(downloads);
            }

            series.Add(loginSerie);
            series.Add(uploadSerie);
            series.Add(downloadSerie);

            accountUsageAndStatistics.UsageDTO = accountUsage;
            accountUsageAndStatistics.AccountStatisticsDTO.xAxis = xAxis;
            accountUsageAndStatistics.AccountStatisticsDTO.Series = series;

            return accountUsageAndStatistics;
        }

        public UsageDTO GetAccountUsageFiles(string userEmail)
        {
            UsageDTO accountUsage = new UsageDTO();

            accountUsage = GetAllowedAccountUsage(userEmail.ToLower().Trim());

            CloudTable fileTable = Storage.GetCloudFileTable();
            TableQuery<File> fileQuery = new TableQuery<File>();

            List<File> files = (from e in fileTable.ExecuteQuery(fileQuery)
                                where e.PartitionKey == "files" &&
                                e.UserEmail == userEmail.ToLower().Trim()
                                select e).ToList();

            int fileCount = 0;
            long fileBytes = 0;
            foreach (var item in files)
            {
                fileCount += 1;
                fileBytes += item.FileSizeBytes;
            }

            accountUsage.FileCount = fileCount;
            accountUsage.StoredBytes = fileBytes;

            return accountUsage;
        }

        public UsageDTO GetAccountUsageLinks(string userEmail)
        {
            UsageDTO accountUsage = new UsageDTO();

            accountUsage = GetAllowedAccountUsage(userEmail.ToLower().Trim());

            CloudTable linkTable = Storage.GetCloudLinkTable();
            TableQuery<Link> linkQuery = new TableQuery<Link>();

            List<Link> links = (from e in linkTable.ExecuteQuery(linkQuery)
                                where e.PartitionKey == "links" &&
                                e.UserEmail == userEmail.ToLower().Trim()
                                select e).ToList();

            accountUsage.LinkCount = links.Count();

            return accountUsage;
        }

        public UsageDTO GetAccountUsageGuestAccounts(string userEmail)
        {
            //Get allowed guestaccounts count
            UsageDTO accountUsage = GetAllowedAccountUsage(userEmail.ToLower().Trim());

            //Get active guestaccounts
            int guestAccounts = GetGuestCountLinkedWithUser(userEmail.ToLower().Trim());

            //Get active guestaccount invites pending
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();
            CloudTable accountLinkVerificationTable = Storage.GetCloudAccountLinkVerificationTable();
            TableQuery<AccountLinkVerification> accountLinkVerificationQuery = accountLinkVerificationTable.CreateQuery<AccountLinkVerification>();
            int guestAccountsInvites = (from alvc in accountLinkVerificationTable.ExecuteQuery(accountLinkVerificationQuery)
                                        where alvc.PartitionKey == "accountlinkverifications" &&
                                        alvc.AccountOwnerEmail == userEmail &&
                                        (alvc.DateExpiresVerification - currentTime).TotalDays > 0
                                        select alvc).Count();

            accountUsage.GuestAccountCount = guestAccounts + guestAccountsInvites;

            return accountUsage;
        }

        public string GetUploadSASURI(string blobname)
        {
            CloudBlobContainer blobContainer = Storage.GetCloudBlobFileContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobname);

            string sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                Permissions = SharedAccessBlobPermissions.Write
            });

            string blobSASURI = string.Format(CultureInfo.InvariantCulture, "{0}{1}", blobContainer.Uri, sas);

            return blobSASURI;
        }

        public string GetDownloadSASURIForFile(string blobName, string fileName, string fileExtension, int ExpiredInHours)
        {
            CloudBlobContainer blobContainer = Storage.GetCloudBlobFileContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);

            string sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(ExpiredInHours),
                Permissions = SharedAccessBlobPermissions.Read
            }, new SharedAccessBlobHeaders()
            {
                ContentDisposition = "Attachment; filename=" + fileName + fileExtension
            });

            return (string.Format(CultureInfo.InvariantCulture, "{0}{1}", blob.Uri, sas));
        }

        public string GetDownloadSASURIForLink(string blobName, string fileName, string fileExtension, int ExpiredInHours)
        {
            CloudBlobContainer blobContainer = Storage.GetCloudBlobFileContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);

            string sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(ExpiredInHours),
                Permissions = SharedAccessBlobPermissions.Read
            }, new SharedAccessBlobHeaders()
            {
                ContentDisposition = "Attachment; filename=" + fileName + fileExtension
            });

            string blobSASURI = (string.Format(CultureInfo.InvariantCulture, "{0}{1}", blob.Uri, sas));

            return blobSASURI;
        }

        public FileDTO GetBlobAttributes(string blobName)
        {
            FileDTO accountBlobFileModel = new FileDTO();

            //Get blob file
            CloudBlobContainer blobFileContainer = Storage.GetCloudBlobFileContainer();
            CloudBlockBlob blob = blobFileContainer.GetBlockBlobReference(blobName);

            if (blob.Exists())
            {
                blob.FetchAttributes();

                accountBlobFileModel = new FileDTO()
                {
                    Name = blob.Name,
                    Size = blob.Properties.Length
                };
            }

            return accountBlobFileModel;
        }

        public void InsertFile(File file)
        {
            //Get blob icon
            CloudBlobContainer blobIconContainer = Storage.GetCloudBlobIconContainer();
            string blobIconURL = blobIconContainer.Uri + "/" + file.FileExtension + ".png";

            //Validate if blob icon exist
            CloudBlockBlob blobIcon = blobIconContainer.GetBlockBlobReference(file.FileExtension + ".png");
            bool blobIcanValidateResult = blobIcon.Exists();

            //Set blob icon associated with file to blank if no icon with that extension was found in blob storage
            if (!blobIcanValidateResult)
                blobIconURL = blobIconContainer.Uri + "/" + "blank.png";

            //Set blob icon URL
            file.BlobIconURL = blobIconURL;

            CloudTable uploadDirectlyTable = Storage.GetCloudFileTable();

            TableOperation insertOperation = TableOperation.InsertOrReplace(file);
            uploadDirectlyTable.Execute(insertOperation);
        }

        public void InsertUsage(Usage usage)
        {
            CloudTable usageTable = Storage.GetCloudUsageTable();

            TableOperation insertOperation = TableOperation.InsertOrReplace(usage);
            usageTable.Execute(insertOperation);
        }

        public void InsertLink(Link link)
        {
            CloudTable linkTable = Storage.GetCloudLinkTable();

            TableOperation insertOperation = TableOperation.InsertOrReplace(link);
            linkTable.Execute(insertOperation);
        }

        public void DeleteBlob(string blobName)
        {
            CloudBlobContainer blobContainer = Storage.GetCloudBlobFileContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);
            if (blob.Exists())
            {
                blob.Delete();
            }
        }

        public bool IsFileOwnedByAnyone(string blobName)
        {
            CloudTable fileTable = Storage.GetCloudFileTable();
            TableQuery<File> tableQuery = new TableQuery<File>();
            bool isFileOwnedByAnyone = (from ifoba in fileTable.ExecuteQuery(tableQuery)
                                        where ifoba.PartitionKey == "files" && ifoba.RowKey == blobName
                                        select ifoba).Any();

            return isFileOwnedByAnyone;
        }

        public void InsertRegisterVerification(AccountLinkVerification accountLinkVerificationEntity)
        {
            CloudTable accountLinkVerificationTable = Storage.GetCloudAccountLinkVerificationTable();

            TableOperation insertOperation = TableOperation.InsertOrReplace(accountLinkVerificationEntity);
            accountLinkVerificationTable.Execute(insertOperation);
        }

        public bool AccountLinkVerificationExist(string userEmail, string guestEmail)
        {
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            CloudTable accountLinkVerificationTable = Storage.GetCloudAccountLinkVerificationTable();
            TableQuery<AccountLinkVerification> accountLinkVerificationQuery = accountLinkVerificationTable.CreateQuery<AccountLinkVerification>();

            bool accountLinkVerificationExist = (from alve in accountLinkVerificationTable.ExecuteQuery(accountLinkVerificationQuery)
                                                 where alve.PartitionKey == "accountlinkverifications" &&
                                                 alve.AccountOwnerEmail == userEmail.ToLower().Trim() &&
                                                 alve.GuestEmail == guestEmail.ToLower().Trim() &&
                                                 (alve.DateExpiresVerification - currentTime).TotalDays > 0
                                                 select alve).SingleOrDefault() != null;

            return accountLinkVerificationExist;
        }

        public AccountLinkVerification GetAccountLinkVerification(string userEmail, string guestEmail, string key)
        {
            CloudTable accountLinkVerificationTable = Storage.GetCloudAccountLinkVerificationTable();
            TableQuery<AccountLinkVerification> accountLinkVerificationQuery = accountLinkVerificationTable.CreateQuery<AccountLinkVerification>();

            AccountLinkVerification accountLinkVerificationEntity = (from r in accountLinkVerificationTable.ExecuteQuery(accountLinkVerificationQuery)
                                                                     where r.PartitionKey == "accountlinkverifications" &&
                                                                     r.AccountOwnerEmail == userEmail &&
                                                                     r.GuestEmail == guestEmail.ToLower().Trim() &&
                                                                     r.Key == key.Trim()
                                                                     select r).SingleOrDefault();

            return accountLinkVerificationEntity;
        }

        public bool DeleteAccountLinkVerification(string userEmail, string guestEmail)
        {
            bool success = false;

            CloudTable accountLinkVerificationTable = Storage.GetCloudAccountLinkVerificationTable();
            TableQuery<AccountLinkVerification> accountLinkVerificationQuery = accountLinkVerificationTable.CreateQuery<AccountLinkVerification>();

            List<AccountLinkVerification> accountLinkVerificationEntities = (from r in accountLinkVerificationTable.ExecuteQuery(accountLinkVerificationQuery)
                                                                             where r.PartitionKey == "accountlinkverifications" &&
                                                                             r.AccountOwnerEmail == userEmail.ToLower().Trim() &&
                                                                             r.GuestEmail == guestEmail.ToLower().Trim()
                                                                             select r).ToList();

            if (accountLinkVerificationEntities != null && accountLinkVerificationEntities.Count() > 0)
            {
                int deleteCount = 0;

                while (deleteCount < accountLinkVerificationEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<AccountLinkVerification> accountLinkVerificationEntities100 = accountLinkVerificationEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (AccountLinkVerification item in accountLinkVerificationEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                        accountLinkVerificationTable.ExecuteBatch(batchOperation);

                    deleteCount += 100;
                }

                success = true;
            }

            return success;
        }

        public IEnumerable<Log> GetLogs(string userEmail)
        {
            //Get accountlogs that is not older than 30 days
            DateTime ExpireDate = DataTimeZoneExtensions.GetCurrentDate();
            DateTime startDate = DataTimeZoneExtensions.GetCurrentDate().AddDays(-30);

            CloudTable logTable = Storage.GetCloudLogTable();

            TableQuery<Log> logQuery = new TableQuery<Log>();
            List<Log> logs = (from e in logTable.ExecuteQuery(logQuery)
                              where e.PartitionKey == "logs" &&
                              e.AccountOwnerEmail == userEmail.ToLower().Trim() &&
                              e.DateTriggered > startDate && e.DateTriggered < ExpireDate
                              orderby e.DateTriggered descending, e.TriggeredByEmail ascending
                              select e).ToList();

            return logs;
        }

        public void InsertAccountLog(Log accountLog)
        {
            CloudTable logTable = Storage.GetCloudLogTable();

            //Insert new accountlog
            TableOperation insertOperation = TableOperation.InsertOrReplace(accountLog);
            logTable.Execute(insertOperation);
        }

        public IEnumerable<GuestDTO> GetGuestsLinkedWithUser(int userID)
        {
            List<GuestDTO> guestDTOs = new List<GuestDTO>();

            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();
            List<AccountLink> accountLinks = (from ao in DB.AccountLink.Include("Guest").Include("AccountLinkPermissions.Permission.PermissionType")
                                              where ao.UserID == userID &&
                                              SqlFunctions.DateDiff("second", currentTime, ao.DateExpires) > 0
                                              select ao).ToList();

            if (accountLinks != null && accountLinks.Count > 0)
            {
                foreach (var item in accountLinks)
                {
                    if (item.Guest != null)
                    {
                        List<String> permissionNames = new List<string>();
                        if (item.AccountLinkPermissions != null && item.AccountLinkPermissions.Count > 0)
                        {
                            foreach (var accountLinkPermission in item.AccountLinkPermissions)
                            {
                                if (accountLinkPermission.Permission != null && accountLinkPermission.Permission.PermissionType != null)
                                {
                                    permissionNames.Add(accountLinkPermission.Permission.PermissionType.Name + accountLinkPermission.Permission.Name);
                                }
                            }
                        }

                        GuestDTO guestDTOTemp = new GuestDTO()
                        {
                            Email = item.Guest.Email,
                            DateRegistered = item.DateCreated,
                            DateExpires = item.DateExpires,
                            DateStart = item.DateStart,
                            UserID = item.Guest.UserID,
                            PermissionNames = permissionNames
                        };

                        guestDTOs.Add(guestDTOTemp);
                    }
                }

                guestDTOs = guestDTOs.OrderBy(m => m.Email).ThenByDescending(n => n.DateStart).ToList();
            }

            return guestDTOs;
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return DB.Permission.Include("PermissionType");
        }

        public bool IsGuestLinkedWithUser(string guestEmail, string userEmail)
        {
            bool isGuestLinkedWithUser = false;

            var guest = (from u in DB.Account
                         where u.Email == guestEmail.ToLower().Trim() &&
                         !u.IsDeleted &&
                         u is Guest
                         select u).SingleOrDefault();

            var user = (from u in DB.Account
                        where u.Email == userEmail.ToLower().Trim() &&
                        !u.IsDeleted
                        && u is Database.Models.User
                        select u).SingleOrDefault();

            if (guest != null && user != null)
            {
                DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

                isGuestLinkedWithUser = (from ac in DB.AccountLink
                                         where ac.GuestID == guest.UserID &&
                                         ac.UserID == user.UserID &&
                                         SqlFunctions.DateDiff("second", currentTime, ac.DateExpires) > 0 &&
                                         SqlFunctions.DateDiff("second", currentTime, ac.DateStart) < 0
                                         select ac).Any();
            }

            return isGuestLinkedWithUser;
        }

        public bool IsGuestLinkExpired(int guestID, int userID)
        {
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();
            bool guestLinkExpired = (from ac in DB.AccountLink
                                     where ac.GuestID == guestID &&
                                     ac.UserID == userID &&
                                     SqlFunctions.DateDiff("second", currentTime, ac.DateExpires) < 0
                                     select ac).Any();

            return guestLinkExpired;
        }

        public void UpdateAccountLink(int userID, int guestID, IEnumerable<int> permissionIDs, DateTime dateStart, DateTime dateExpires)
        {
            AccountLink accountLink = (from ac in DB.AccountLink
                                       where ac.GuestID == guestID && ac.UserID == userID
                                       select ac).SingleOrDefault();

            if (accountLink != null)
            {
                List<AccountLinkPermission> currentAccountLinkPermission = (from a in DB.AccountLinkPermission
                                                                            where a.UserID == userID &&
                                                                            a.GuestID == guestID
                                                                            select a).ToList();

                //Delete all current permissions
                if (currentAccountLinkPermission != null && currentAccountLinkPermission.Count > 0)
                {
                    foreach (var currentPermissions in currentAccountLinkPermission)
                    {
                        DB.AccountLinkPermission.Remove(currentPermissions);
                    }

                    DB.SaveChanges();
                }

                //Update start and expiry date
                accountLink.DateStart = dateStart;
                accountLink.DateExpires = dateExpires;

                //Insert new permissions
                if (permissionIDs != null && permissionIDs.Count() > 0)
                {
                    foreach (var item in permissionIDs)
                    {
                        Permission permission = (from p in DB.Permission
                                                 where p.PermissionID == item
                                                 select p).SingleOrDefault();

                        if (permission != null)
                        {
                            AccountLinkPermission accountLinkPermission = new AccountLinkPermission(userID, guestID, permission.PermissionID);

                            DB.AccountLinkPermission.Add(accountLinkPermission);
                        }
                    }
                }

                DB.SaveChanges();
            }
        }

        public void InsertAccountLink(int userID, int guestID, IEnumerable<int> permissionIDs, DateTime dateCreated, DateTime dateStart, DateTime dateExpires)
        {
            var user = (from u in DB.Account
                        where u.UserID == userID && u is Database.Models.User
                        select u).SingleOrDefault();

            var guest = (from u in DB.Account
                         where u.UserID == guestID && u is Guest
                         select u).SingleOrDefault();

            if (user != null && guest != null)
            {
                AccountLink accountLink = (from al in DB.AccountLink
                                           where al.UserID == user.UserID &&
                                           al.GuestID == guestID
                                           select al).SingleOrDefault();

                if (accountLink != null)
                {
                    //Update existing accountlink
                    UpdateAccountLink(user.UserID, guest.UserID, permissionIDs, dateStart, dateExpires);
                }
                else
                {
                    //Insert new accountlink
                    AccountLink newAccountLink = new AccountLink(user.UserID, guest.UserID, dateCreated, dateStart, dateExpires);

                    DB.AccountLink.Add(newAccountLink);
                    DB.SaveChanges();

                    //Insert accountlinkpermissions
                    if (permissionIDs != null && permissionIDs.Count() > 0)
                    {
                        foreach (var item in permissionIDs)
                        {
                            Permission permission = (from p in DB.Permission
                                                     where p.PermissionID == item
                                                     select p).SingleOrDefault();

                            if (permission != null)
                            {
                                AccountLinkPermission accountLinkPermission = new AccountLinkPermission(user.UserID, guest.UserID, permission.PermissionID);

                                DB.AccountLinkPermission.Add(accountLinkPermission);
                            }
                        }

                        DB.SaveChanges();
                    }
                }
            }
        }

        public void DeleteAllAccountLinksGuest(int guestID)
        {
            List<AccountLink> accountLinks = (from ao in DB.AccountLink.Include("AccountLinkPermissions")
                                              where ao.GuestID == guestID
                                              select ao).ToList();

            if (accountLinks != null && accountLinks.Count > 0)
            {
                foreach (var item in accountLinks)
                {
                    //Delete account link and permissions
                    DB.AccountLink.Remove(item);
                }

                DB.SaveChanges();
            }
        }

        public void DeleteAllAccountLinksUser(int userID)
        {
            //Delete active guestaccounts
            List<AccountLink> accountLinks = (from ao in DB.AccountLink.Include("AccountLinkPermissions")
                                              where ao.UserID == userID
                                              select ao).ToList();

            if (accountLinks != null && accountLinks.Count > 0)
            {
                foreach (var item in accountLinks)
                {
                    //Delete account link and permissions
                    DB.AccountLink.Remove(item);
                }

                DB.SaveChanges();
            }

            //Delete pending guestaccounts
            CloudTable accountLinkVerificationTable = Storage.GetCloudAccountLinkVerificationTable();
            TableQuery<AccountLinkVerification> accountLinkVerificationQuery = accountLinkVerificationTable.CreateQuery<AccountLinkVerification>();

            List<AccountLinkVerification> accountLinkVerificationEntities = (from r in accountLinkVerificationTable.ExecuteQuery(accountLinkVerificationQuery)
                                                                             where r.PartitionKey == "accountlinkverifications" &&
                                                                             r.AccountOwnerID == userID
                                                                             select r).ToList();

            if (accountLinkVerificationEntities != null && accountLinkVerificationEntities.Count() > 0)
            {
                int deleteCount = 0;

                while (deleteCount < accountLinkVerificationEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<AccountLinkVerification> accountLinkVerificationEntities100 = accountLinkVerificationEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (AccountLinkVerification item in accountLinkVerificationEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                        accountLinkVerificationTable.ExecuteBatch(batchOperation);

                    deleteCount += 100;
                }
            }
        }

        public IEnumerable<AccountLinkPermission> GetAccountLinkPermissions(int userID, int guestID)
        {
            List<AccountLinkPermission> accountLinkpermissionsDTO = new List<AccountLinkPermission>();

            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            AccountLink accountLinks = (from ao in DB.AccountLink
                                        where ao.GuestID == guestID &&
                                        ao.UserID == userID &&
                                        SqlFunctions.DateDiff("second", currentTime, ao.DateExpires) > 0 &&
                                        SqlFunctions.DateDiff("second", currentTime, ao.DateStart) < 0
                                        select ao).SingleOrDefault();

            if (accountLinks != null)
            {
                List<AccountLinkPermission> accountLinkpermission = (from p in DB.AccountLinkPermission.Include("Permission.PermissionType")
                                                                     where p.UserID == userID &&
                                                                     p.GuestID == guestID
                                                                     select p).ToList();

                if (accountLinkpermission != null && accountLinkpermission.Count > 0)
                {
                    foreach (var item in accountLinkpermission)
                    {
                        Permission permission = new Permission()
                        {
                            Name = item.Permission.Name,
                            PermissionTypeID = item.Permission.PermissionTypeID,
                            PermissionType = item.Permission.PermissionType,
                            PermissionID = item.Permission.PermissionID
                        };

                        AccountLinkPermission accountLinkPermissionDTOtemp = new AccountLinkPermission()
                        {
                            UserID = item.UserID,
                            GuestID = item.GuestID,
                            PermissionID = item.PermissionID,
                            Permission = permission
                        };

                        accountLinkpermissionsDTO.Add(accountLinkPermissionDTOtemp);
                    }
                }
            }

            return accountLinkpermissionsDTO;
        }

        public AccountLinkPermissionBoolDTO GetAccountLinkPermissionsBool(int userID, int guestID)
        {
            AccountLinkPermissionBoolDTO accountLinkPermissionBool = new AccountLinkPermissionBoolDTO();

            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            AccountLink accountLink = (from a in DB.AccountLink
                                       where a.UserID == userID &&
                                       a.GuestID == guestID &&
                                       SqlFunctions.DateDiff("second", currentTime, a.DateExpires) > 0 &&
                                       SqlFunctions.DateDiff("second", currentTime, a.DateStart) < 0
                                       select a).SingleOrDefault();

            if (accountLink != null)
            {
                List<AccountLinkPermission> accountLinkpermission = (from p in DB.AccountLinkPermission.Include("Permission")
                                                                     where p.UserID == userID &&
                                                                     p.GuestID == guestID
                                                                     select p).ToList();

                bool fileRead = accountLinkpermission.Where(m => m.Permission.Name == "Read" && m.Permission.PermissionTypeID == 1).Any();
                bool fileWrite = accountLinkpermission.Where(m => m.Permission.Name == "Write" && m.Permission.PermissionTypeID == 1).Any();
                bool fileEdit = accountLinkpermission.Where(m => m.Permission.Name == "Edit" && m.Permission.PermissionTypeID == 1).Any();

                bool linkRead = accountLinkpermission.Where(m => m.Permission.Name == "Read" && m.Permission.PermissionTypeID == 2).Any();
                bool linkWrite = accountLinkpermission.Where(m => m.Permission.Name == "Write" && m.Permission.PermissionTypeID == 2).Any();
                bool linkEdit = accountLinkpermission.Where(m => m.Permission.Name == "Edit" && m.Permission.PermissionTypeID == 2).Any();

                accountLinkPermissionBool = new AccountLinkPermissionBoolDTO()
                {
                    FileEdit = fileEdit,
                    FileRead = fileRead,
                    FileWrite = fileWrite,
                    LinkEdit = linkEdit,
                    LinkRead = linkRead,
                    LinkWrite = linkWrite
                };
            }

            return accountLinkPermissionBool;
        }

        public bool BlobExist(string blobName)
        {
            CloudBlobContainer blobContainer = Storage.GetCloudBlobFileContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);

            return blob.Exists();
        }

        public IEnumerable<File> GetFiles(string userEmail)
        {
            CloudTable fileTable = Storage.GetCloudFileTable();
            TableQuery<File> fileQuery = fileTable.CreateQuery<File>();
            List<File> files = (from f in fileTable.ExecuteQuery(fileQuery)
                                where f.PartitionKey == "files" &&
                                f.UserEmail == userEmail
                                orderby f.FileName ascending, f.DateCreated descending
                                select f).ToList();

            return files;
        }

        public IEnumerable<Link> GetLinks(string userEmail)
        {
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            CloudTable linkTable = Storage.GetCloudLinkTable();
            TableQuery<Link> linkQuery = linkTable.CreateQuery<Link>();

            List<Link> links = (from l in linkTable.ExecuteQuery(linkQuery)
                                where l.PartitionKey == "links" &&
                                l.UserEmail == userEmail
                                && (l.DateExpires - currentTime).TotalDays > 0
                                orderby l.FileName ascending, l.DateCreated descending
                                select l).ToList();

            if (links != null)
                links.Select(c => { c.BlobSASURI = null; return c; }).ToList();

            return links;
        }

        public File GetFile(string blobName, string userEmail)
        {
            CloudTable fileTable = Storage.GetCloudFileTable();
            TableQuery<File> fileQuery = fileTable.CreateQuery<File>();

            File file = (from f in fileTable.ExecuteQuery(fileQuery)
                         where f.PartitionKey == "files" &&
                         f.RowKey == blobName &&
                         f.UserEmail == userEmail
                         select f).SingleOrDefault();

            return file;
        }

        public File GetFileFromRowKey(string rowKey, string userEmail)
        {
            CloudTable fileTable = Storage.GetCloudFileTable();
            TableQuery<File> fileQuery = fileTable.CreateQuery<File>();

            File file = (from f in fileTable.ExecuteQuery(fileQuery)
                         where f.PartitionKey == "files" &&
                         f.RowKey == rowKey &&
                         f.UserEmail == userEmail
                         select f).SingleOrDefault();

            return file;
        }

        public Link GetLinkFromRowKey(string rowKey)
        {
            CloudTable linkTable = Storage.GetCloudLinkTable();

            TableQuery<Link> linkQuery = new TableQuery<Link>();
            Link linkEntity = (from l in linkTable.ExecuteQuery(linkQuery)
                               where l.PartitionKey == "links" &&
                               l.RowKey == rowKey
                               select l).SingleOrDefault();

            return linkEntity;
        }

        public Link GetActiveLinkFromRowKey(string rowKey)
        {
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            CloudTable linkTable = Storage.GetCloudLinkTable();

            TableQuery<Link> linkQuery = new TableQuery<Link>();
            Link linkEntity = (from l in linkTable.ExecuteQuery(linkQuery)
                               where l.PartitionKey == "links" &&
                               l.RowKey == rowKey &&
                               (l.DateExpires - currentTime).TotalDays > 0 &&
                               (l.DateStart - currentTime).TotalDays < 0
                               select l).SingleOrDefault();

            return linkEntity;
        }

        public void UpdateFile(File file)
        {
            CloudTable fileTable = Storage.GetCloudFileTable();
            TableQuery<File> fileQuery = fileTable.CreateQuery<File>();

            File oldFile = (from f in fileTable.ExecuteQuery(fileQuery)
                            where f.PartitionKey == "files" &&
                            f.RowKey == file.RowKey
                            select f).SingleOrDefault();

            if (oldFile != null)
            {
                TableOperation replaceOperation = TableOperation.Replace(file);
                fileTable.Execute(replaceOperation);
            }
        }

        public void UpdateLink(Link link)
        {
            CloudTable linkTable = Storage.GetCloudLinkTable();
            TableQuery<Link> linkQuery = linkTable.CreateQuery<Link>();

            Link oldLink = (from f in linkTable.ExecuteQuery(linkQuery)
                            where f.PartitionKey == "links" &&
                            f.RowKey == link.RowKey
                            select f).SingleOrDefault();

            if (link != null)
            {
                TableOperation replaceOperation = TableOperation.Replace(link);
                linkTable.Execute(replaceOperation);
            }
        }

        public int DeleteFiles(IEnumerable<string> rowKeys, string userEmail)
        {
            CloudTable fileTable = Storage.GetCloudFileTable();

            TableQuery<File> fileQuery = new TableQuery<File>();
            List<File> fileEntities = (from e in fileTable.ExecuteQuery(fileQuery)
                                       where e.PartitionKey == "files" &&
                                       e.UserEmail == userEmail.ToLower().Trim() &&
                                       rowKeys.Contains(e.RowKey)
                                       select e).ToList();

            if (fileEntities != null && fileEntities.Count > 0)
            {
                CloudBlobContainer blobContainer = Storage.GetCloudBlobFileContainer();

                int deleteCount = 0;

                while (deleteCount < fileEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<File> fileEntities100 = fileEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (File item in fileEntities100)
                    {
                        batchOperation.Delete(item);

                        //Delete blob entity
                        CloudBlockBlob blob = blobContainer.GetBlockBlobReference(item.RowKey);
                        if (blob.Exists())
                        {
                            blob.Delete();
                        }
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                        fileTable.ExecuteBatch(batchOperation);

                    deleteCount += 100;
                }
            }

            return fileEntities.Count;
        }

        public int DeleteLinks(IEnumerable<string> rowKeys, string userEmail)
        {
            CloudTable linkTable = Storage.GetCloudLinkTable();

            TableQuery<Link> linkQuery = new TableQuery<Link>();
            List<Link> linkEntities = (from e in linkTable.ExecuteQuery(linkQuery)
                                       where e.PartitionKey == "links" &&
                                       e.UserEmail == userEmail.ToLower().Trim() &&
                                       rowKeys.Contains(e.RowKey)
                                       select e).ToList();

            if (linkEntities != null && linkEntities.Count > 0)
            {
                int deleteCount = 0;

                while (deleteCount < linkEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<Link> linkEntities100 = linkEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (Link item in linkEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                        linkTable.ExecuteBatch(batchOperation);

                    deleteCount += 100;
                }
            }

            return linkEntities.Count;
        }

        public int DeleteLinksWithBlobNames(IEnumerable<string> blobNames, string userEmail)
        {
            CloudTable linkTable = Storage.GetCloudLinkTable();

            TableQuery<Link> linkQuery = new TableQuery<Link>();
            List<Link> linkEntities = (from e in linkTable.ExecuteQuery(linkQuery)
                                       where e.PartitionKey == "links" &&
                                       e.UserEmail == userEmail.ToLower().Trim() &&
                                       blobNames.Contains(e.BlobName)
                                       select e).ToList();

            if (linkEntities != null && linkEntities.Count > 0)
            {
                int deleteCount = 0;

                while (deleteCount < linkEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<Link> linkEntities100 = linkEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (Link item in linkEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                        linkTable.ExecuteBatch(batchOperation);

                    deleteCount += 100;
                }
            }

            return linkEntities.Count;
        }

        public int DeleteAccountLinks(IEnumerable<int> guestIDs, int userID)
        {
            List<AccountLink> accountLinks = (from a in DB.AccountLink.Include("AccountLinkPermissions")
                                              where a.UserID == userID &&
                                              guestIDs.Contains(a.GuestID)
                                              select a).ToList();

            if (accountLinks != null && accountLinks.Count > 0)
            {
                foreach (var item in accountLinks)
                {
                    //Delete account link
                    DB.AccountLink.Remove(item);
                }

                DB.SaveChanges();
            }

            return accountLinks.Count;
        }

        public int DeleteLogs(IEnumerable<string> rowKeys, string userEmail)
        {
            CloudTable logTable = Storage.GetCloudLogTable();

            TableQuery<Log> logQuery = new TableQuery<Log>();
            List<Log> logEntities = (from e in logTable.ExecuteQuery(logQuery)
                                     where e.PartitionKey == "logs" &&
                                     e.AccountOwnerEmail == userEmail.ToLower().Trim() &&
                                     rowKeys.Contains(e.RowKey)
                                     select e).ToList();

            if (logEntities != null && logEntities.Count > 0)
            {
                int deleteCount = 0;

                while (deleteCount < logEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<Log> logEntities100 = logEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (Log item in logEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                        logTable.ExecuteBatch(batchOperation);

                    deleteCount += 100;
                }
            }

            return logEntities.Count;
        }

        public IEnumerable<AccountLinkVerification> GetPendingGuestsLinkedWithUser(string userEmail)
        {
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            CloudTable accountLinkVerificationTable = Storage.GetCloudAccountLinkVerificationTable();
            TableQuery<AccountLinkVerification> accountLinkVerificationQuery = new TableQuery<AccountLinkVerification>();
            List<AccountLinkVerification> pendingGuestAccounts = (from e in accountLinkVerificationTable.ExecuteQuery(accountLinkVerificationQuery)
                                                                  where e.AccountOwnerEmail == userEmail &&
                                                                  (e.DateExpiresVerification - currentTime).TotalDays > 0
                                                                  orderby e.GuestEmail
                                                                  select e).ToList();

            return pendingGuestAccounts;
        }

        public UsageDTO GetAllowedAccountUsage(string userEmail)
        {
            UsageDTO accountUsageDTO = new UsageDTO();

            var user = (from u in DB.Account
                        where u.Email == userEmail.ToLower().Trim() &&
                        u is Database.Models.User
                        select u).SingleOrDefault();

            if (user != null)
            {
                AccountUsagePremium accountUsagePremium = (from u in DB.AccountUsagePremium
                                                           where u.UserID == user.UserID
                                                           select u).SingleOrDefault();

                //Get premium
                if (accountUsagePremium != null)
                {
                    accountUsageDTO.AllowedFileCount = accountUsagePremium.AllowedFileCount;
                    accountUsageDTO.AllowedGuestAccountCount = accountUsagePremium.AllowedGuestAccountCount;
                    accountUsageDTO.AllowedLinkCount = accountUsagePremium.AllowedLinkCount;
                    accountUsageDTO.AllowedStoredBytes = accountUsagePremium.AllowedStoredBytes;
                }
                //Get free
                else
                {
                    accountUsageDTO.AllowedFileCount = Convert.ToInt32(AccountAllowedFileCount);
                    accountUsageDTO.AllowedGuestAccountCount = Convert.ToInt32(AccountAllowedGuestCount);
                    accountUsageDTO.AllowedLinkCount = Convert.ToInt32(AccountAllowedLinkCount);
                    accountUsageDTO.AllowedStoredBytes = Convert.ToInt64(AccountAllowedStoredBytes);
                }
            }

            return accountUsageDTO;
        }

        public int GetGuestCountLinkedWithUser(string userEmail)
        {
            var user = (from u in DB.Account
                        where u.Email == userEmail.ToLower().Trim() &&
                        u is Database.Models.User
                        select u).SingleOrDefault();

            if (user != null)
            {
                DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

                int accountLinks = (from ao in DB.AccountLink
                                    where ao.UserID == user.UserID &&
                                    SqlFunctions.DateDiff("second", currentTime, ao.DateExpires) > 0 &&
                                    SqlFunctions.DateDiff("second", currentTime, ao.DateStart) < 0
                                    select ao).Count();

                return accountLinks;
            }
            else
                return 0;
        }
    }
}
