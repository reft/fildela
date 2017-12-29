using Fildela.Business.Domains.Administration.Models;
using Fildela.Business.Domains.News.Models;
using Fildela.Business.Domains.User.Models;
using Fildela.Data.CustomModels.Administration;
using Fildela.Data.CustomModels.News;
using Fildela.Data.CustomModels.User;
using Fildela.Data.Storage.Models;
using System.Linq;

namespace Fildela.Business.Domains.Administration
{
    public static class AdministrationExtensions
    {
        public static AdministrationEmailsDTO ToEntity(this AdministrationEmailsDTOModel model)
        {
            if (model == null) return null;

            return new AdministrationEmailsDTO()
            {
                EmailCount = model.EmailCount,
                NewsCount = model.NewsCount,
                UserCount = model.UserCount
            };
        }

        public static AdministrationEmailsDTOModel ToModel(this AdministrationEmailsDTO entity)
        {
            if (entity == null) return null;

            return new AdministrationEmailsDTOModel()
            {
                EmailCount = entity.EmailCount,
                NewsCount = entity.NewsCount,
                UserCount = entity.UserCount
            };
        }

        public static AdministrationLogsDTO ToEntity(this AdministrationLogsDTOModel model)
        {
            if (model == null) return null;

            return new AdministrationLogsDTO()
            {
                EmailCount = model.EmailCount,
                NewsCount = model.NewsCount,
                UserCount = model.UserCount
            };
        }

        public static AdministrationLogsDTOModel ToModel(this AdministrationLogsDTO entity)
        {
            if (entity == null) return null;

            return new AdministrationLogsDTOModel()
            {
                EmailCount = entity.EmailCount,
                NewsCount = entity.NewsCount,
                UserCount = entity.UserCount
            };
        }

        public static AdministrationNewsDTO ToEntity(this AdministrationNewsDTOModel model)
        {
            if (model == null) return null;

            return new AdministrationNewsDTO()
            {
                EmailCount = model.EmailCount,
                NewsCount = model.NewsCount,
                UserCount = model.UserCount
            };
        }

        public static AdministrationNewsDTOModel ToModel(this AdministrationNewsDTO entity)
        {
            if (entity == null) return null;

            return new AdministrationNewsDTOModel()
            {
                EmailCount = entity.EmailCount,
                NewsCount = entity.NewsCount,
                UserCount = entity.UserCount,
                News = entity.News != null ? entity.News.Select(o => o.ToModel()) : null
            };
        }

        public static NewsDTOModel ToModel(this NewsDTO entity)
        {
            if (entity == null) return null;

            return new NewsDTOModel()
            {
                NewsID = entity.NewsID,
                Title = entity.Title,
                CategoryToString = entity.CategoryToString,
                DatePublished = entity.DatePublished,
                PublishedByFullName = entity.PublishedByFullName,
                PublishedByEmail = entity.PublishedByEmail,
                PublishedByID = entity.PublishedByID
            };
        }

        public static UserDTOModel ToModel(this UserDTO entity)
        {
            if (entity == null) return null;

            return new UserDTOModel()
            {
                UserID = entity.UserID,
                Email = entity.Email,
                UserRolesToString = entity.UserRolesToString,
                IsPremium = entity.IsPremium,
                IsDeleted = entity.IsDeleted,
                DateRegistered = entity.DateRegistered,
                DateLastActive = entity.DateLastActive
            };
        }

        public static AccountDTOModel ToModel(this AccountDTO entity)
        {
            if (entity == null) return null;

            return new AccountDTOModel()
            {
                AccountID = entity.AccountID,
                Email = entity.Email,
                UserRolesToString = entity.UserRolesToString,
                IsPremium = entity.IsPremium,
                IsDeleted = entity.IsDeleted,
                DateRegistered = entity.DateRegistered,
                DateLastActive = entity.DateLastActive
            };
        }

        public static AdministrationAccountsDTO ToEntity(this AdministrationAccountsDTOModel model)
        {
            if (model == null) return null;

            return new AdministrationAccountsDTO()
            {
                EmailCount = model.EmailCount,
                NewsCount = model.NewsCount,
                UserCount = model.UserCount
            };
        }

        public static AdministrationAccountsDTOModel ToModel(this AdministrationAccountsDTO entity)
        {
            if (entity == null) return null;

            return new AdministrationAccountsDTOModel()
            {
                EmailCount = entity.EmailCount,
                NewsCount = entity.NewsCount,
                UserCount = entity.UserCount,
                Accounts = entity.Accounts != null ? entity.Accounts.Select(o => o.ToModel()) : null
            };
        }

        public static AdminJob ToEntity(this AdminJobModel model)
        {
            if (model == null) return null;

            return new AdminJob()
            {
                AffectedRows = model.AffectedRows,
                DateEnd = model.DateEnd,
                DateStart = model.DateStart,
                ErrorCount = model.ErrorCount,
                RowKey = model.RowKey,
                Type = model.Type,
                PartitionKey = model.PartitionKey,
                ETag = model.ETag
            };
        }

        public static AdminJobModel ToModel(this AdminJob entity)
        {
            if (entity == null) return null;

            return new AdminJobModel()
            {
                AffectedRows = entity.AffectedRows,
                DateEnd = entity.DateEnd,
                DateStart = entity.DateStart,
                ErrorCount = entity.ErrorCount,
                RowKey = entity.RowKey,
                Type = entity.Type,
                PartitionKey = entity.PartitionKey,
                ETag = entity.ETag
            };
        }

        public static AdminLog ToEntity(this AdminLogModel model)
        {
            if (model == null) return null;

            return new AdminLog()
            {
                AccountOwnerEmail = model.AccountOwnerEmail,
                AccountOwnerID = model.AccountOwnerID,
                DateExpires = model.DateExpires,
                DateTriggered = model.DateTriggered,
                IpAddress = model.IpAddress,
                RowKey = model.RowKey,
                TriggeredByEmail = model.TriggeredByEmail,
                TriggeredByID = model.TriggeredByID,
                Type = model.Type,
                TriggeredByRole = model.TriggeredByRole,
                PartitionKey = model.PartitionKey,
                ETag = model.ETag
            };
        }

        public static AdminLogModel ToModel(this AdminLog entity)
        {
            if (entity == null) return null;

            return new AdminLogModel()
            {
                AccountOwnerEmail = entity.AccountOwnerEmail,
                AccountOwnerID = entity.AccountOwnerID,
                DateExpires = entity.DateExpires,
                DateTriggered = entity.DateTriggered,
                IpAddress = entity.IpAddress,
                RowKey = entity.RowKey,
                TriggeredByEmail = entity.TriggeredByEmail,
                TriggeredByID = entity.TriggeredByID,
                Type = entity.Type,
                TriggeredByRole = entity.TriggeredByRole,
                PartitionKey = entity.PartitionKey,
                ETag = entity.ETag
            };
        }
    }
}
