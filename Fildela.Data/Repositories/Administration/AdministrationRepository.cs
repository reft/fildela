using Fildela.Data.CustomModels.Administration;
using Fildela.Data.CustomModels.News;
using Fildela.Data.CustomModels.User;
using Fildela.Data.Database.DataLayer;
using Fildela.Data.Database.Models;
using Fildela.Data.Helpers;
using Fildela.Data.Storage.Models;
using Fildela.Data.Storage.Services;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Fildela.Data.Repositories.Administration
{
    public class AdministrationRepository : RepositoryBase, IAdministrationRepository
    {
        public AdministrationRepository(DataLayer db, CloudStorageServices storage) : base(db, storage) { }

        public void InsertAdminLog(AdminLog adminLog)
        {
            CloudTable adminLogTable = Storage.GetCloudAdminLogTable();

            TableOperation insertOperation = TableOperation.InsertOrReplace(adminLog);
            adminLogTable.Execute(insertOperation);
        }

        public bool AdminLogExist(string ipAddress, string type)
        {
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            CloudTable adminLogTable = Storage.GetCloudAdminLogTable();
            TableQuery<AdminLog> adminLogQuery = adminLogTable.CreateQuery<AdminLog>();

            bool adminLogExist = (from a in adminLogTable.ExecuteQuery(adminLogQuery)
                                  where a.PartitionKey == "adminlogs" &&
                                  a.IpAddress == ipAddress.Trim() &&
                                  a.Type == type &&
                                  (a.DateExpires - currentTime).TotalDays > 0
                                  select a).SingleOrDefault() != null;

            return adminLogExist;
        }

        public int AdminLogExistCount(string ipAddress, string type)
        {
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            CloudTable adminLogTable = Storage.GetCloudAdminLogTable();
            TableQuery<AdminLog> adminLogQuery = adminLogTable.CreateQuery<AdminLog>();

            int adminLogExistCount = (from a in adminLogTable.ExecuteQuery(adminLogQuery)
                                      where a.PartitionKey == "adminlogs" &&
                                      a.IpAddress == ipAddress.Trim() &&
                                      a.Type == type &&
                                      (a.DateExpires - currentTime).TotalDays > 0
                                      select a).Count();

            return adminLogExistCount;
        }

        public bool AdminLogExistAuthenticated(string email, string type)
        {
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            CloudTable adminLogTable = Storage.GetCloudAdminLogTable();
            TableQuery<AdminLog> adminLogQuery = adminLogTable.CreateQuery<AdminLog>();

            bool adminLogExist = (from a in adminLogTable.ExecuteQuery(adminLogQuery)
                                  where a.PartitionKey == "adminlogs" &&
                                  a.TriggeredByEmail == email.ToLower().Trim() &&
                                  a.Type == type &&
                                  (a.DateExpires - currentTime).TotalDays > 0
                                  select a).SingleOrDefault() != null;

            return adminLogExist;
        }

        public AdministrationAccountsDTO GetUsersAdministration()
        {
            List<Database.Models.User> users = (from u in DB.User.Include("UserRoles.Role")
                                                             select u).ToList();

            List<Guest> guests = (from u in DB.Guest
                                  select u).ToList();

            List<AccountUsagePremium> premiumList = DB.AccountUsagePremium.ToList();

            int newsCount = (from n in DB.News
                             select n).Count();

            int emailCount = 0;

            int userCount = users.Count();

            List<AccountDTO> accountsDTO = new List<AccountDTO>();

            foreach (var item in users)
            {
                //Get premium status
                DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();
                bool IsPremium = premiumList.Where(m => m.UserID == item.UserID && SqlFunctions.DateDiff("second", currentTime, m.DateExpires) > 0).Any();

                //Get roles and convert it to one string
                string userRolesToString = string.Empty;
                if (item.UserRoles != null && item.UserRoles.Count > 0)
                {
                    foreach (var role in item.UserRoles.Reverse())
                    {
                        if (String.IsNullOrEmpty(userRolesToString))
                            userRolesToString = role.Role.Name;
                        else
                            userRolesToString += ", " + role.Role.Name;
                    }
                }

                AccountDTO userDTOtemp = new AccountDTO()
                {
                    AccountID = item.UserID,
                    Email = item.Email,
                    UserRolesToString = userRolesToString,
                    IsPremium = IsPremium,
                    IsDeleted = item.IsDeleted,
                    DateRegistered = item.DateRegistered,
                    DateLastActive = item.DateLastActive
                };

                accountsDTO.Add(userDTOtemp);
            }

            foreach (var item in guests)
            {
                AccountDTO guestDTOtemp = new AccountDTO()
                {
                    AccountID = item.UserID,
                    Email = item.Email,
                    UserRolesToString = "Guest",
                    IsPremium = false,
                    IsDeleted = item.IsDeleted,
                    DateRegistered = item.DateRegistered,
                    DateLastActive = item.DateLastActive
                };

                accountsDTO.Add(guestDTOtemp);
            }

            AdministrationAccountsDTO administrationViewModel = new AdministrationAccountsDTO()
            {
                UserCount = userCount,
                NewsCount = newsCount,
                EmailCount = emailCount,
                Accounts = accountsDTO.OrderBy(m => m.Email)
            };

            return administrationViewModel;
        }

        public AdministrationNewsDTO GetNewsAdministration()
        {
            int userCount = (from uc in DB.Account
                             select uc).Count();

            int newsCount = (from nc in DB.News.Include("NewsCategories")
                             select nc).Count();

            int emailCount = 0;

            List<NewsDTO> newsDTO = new List<NewsDTO>();

            List<Fildela.Data.Database.Models.News> news = (from n in DB.News.Include("Category")
                                                            orderby n.DatePublished descending
                                                            select n).ToList();

            foreach (var item in news)
            {
                NewsDTO newsDTOtemp = new NewsDTO()
                {
                    TextBlobName = item.TextBlobName,
                    ImageBlobURL = item.ImageBlobURL,
                    DatePublished = item.DatePublished,
                    CategoryToString = item.Category.Name,
                    NewsID = item.NewsID,
                    PreviewText = item.PreviewText,
                    PublishedByFullName = item.PublishedByFullName,
                    PublishedByID = item.PublishedByID,
                    PublishedByEmail = item.PublishedByEmail,
                    Title = item.Title
                };

                newsDTO.Add(newsDTOtemp);
            }

            AdministrationNewsDTO administrationViewModel = new AdministrationNewsDTO()
            {
                UserCount = userCount,
                NewsCount = newsCount,
                EmailCount = emailCount,
                News = newsDTO
            };

            return administrationViewModel;
        }

        public AdministrationEmailsDTO GetEmailAdministration()
        {
            int userCount = (from u in DB.Account
                             select u).Count();

            int newsCount = (from n in DB.News
                             select n).Count();

            int emailCount = 0;

            AdministrationEmailsDTO administrationViewModel = new AdministrationEmailsDTO()
            {
                UserCount = userCount,
                NewsCount = newsCount,
                EmailCount = emailCount
            };

            return administrationViewModel;
        }

        public AdministrationLogsDTO GetLogsAdministration()
        {
            int userCount = (from u in DB.Account
                             select u).Count();

            int newsCount = (from n in DB.News
                             select n).Count();

            int emailCount = 0;

            AdministrationLogsDTO administrationViewModel = new AdministrationLogsDTO()
            {
                UserCount = userCount,
                NewsCount = newsCount,
                EmailCount = emailCount
            };

            return administrationViewModel;
        }

        public string GetUserFullName(int userID)
        {
            string userFullName = string.Empty;

            var user = (from u in DB.Account
                        where u.UserID == userID
                        select u).SingleOrDefault();

            if (user != null)
                userFullName = user.FirstName + " " + user.LastName;

            return userFullName;
        }
    }
}
