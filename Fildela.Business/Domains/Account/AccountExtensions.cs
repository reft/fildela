using Fildela.Business.Domains.Account.Models;
using Fildela.Data.CustomModels.Account;
using Fildela.Data.Database.Models;
using Fildela.Data.Storage.Models;
using System.Linq;

namespace Fildela.Business.Domains.Account
{
    public static class AccountExtensions
    {
        public static FileDTO ToEntity(this Models.FileDTOModel model)
        {
            if (model == null) return null;

            return new Data.CustomModels.Account.FileDTO()
            {
                Name = model.Name,
                Size = model.Size
            };
        }

        public static FileDTOModel ToModel(this Data.CustomModels.Account.FileDTO entity)
        {
            if (entity == null) return null;

            return new Models.FileDTOModel()
            {
                Name = entity.Name,
                Size = entity.Size
            };
        }

        public static AccountLinkPermissionBoolDTO ToEntity(this AccountLinkPermissionBoolDTOModel model)
        {
            if (model == null) return null;

            return new AccountLinkPermissionBoolDTO()
            {
                FileEdit = model.FileEdit,
                FileRead = model.FileRead,
                FileWrite = model.FileWrite,
                LinkEdit = model.LinkEdit,
                LinkRead = model.LinkRead,
                LinkWrite = model.LinkWrite
            };
        }

        public static AccountLinkPermissionBoolDTOModel ToModel(this AccountLinkPermissionBoolDTO entity)
        {
            if (entity == null) return null;

            return new AccountLinkPermissionBoolDTOModel()
            {
                FileEdit = entity.FileEdit,
                FileRead = entity.FileRead,
                FileWrite = entity.FileWrite,
                LinkEdit = entity.LinkEdit,
                LinkRead = entity.LinkRead,
                LinkWrite = entity.LinkWrite
            };
        }

        public static AccountLinkPermission ToEntity(this AccountLinkPermissionModel model)
        {
            if (model == null) return null;

            return new AccountLinkPermission()
            {
                GuestID = model.GuestID,
                PermissionID = model.PermissionID,
                UserID = model.UserID,
                Permission = model.Permission != null ? model.Permission.ToEntity() : null
            };
        }

        public static AccountLinkPermissionModel ToModel(this AccountLinkPermission entity)
        {
            if (entity == null) return null;

            return new AccountLinkPermissionModel()
            {
                GuestID = entity.GuestID,
                PermissionID = entity.PermissionID,
                UserID = entity.UserID,
                Permission = entity.Permission != null ? entity.Permission.ToModel() : null
            };
        }

        public static AccountLinkVerification ToEntity(this AccountLinkVerificationModel model)
        {
            if (model == null) return null;

            return new AccountLinkVerification()
            {
                AccountOwnerEmail = model.UserEmail,
                AccountOwnerID = model.UserID,
                AgreeUserAgreement = model.AgreeUserAgreement,
                DateCreated = model.DateCreated,
                DateExpiresAccountLink = model.DateExpiresAccountLink,
                DateExpiresVerification = model.DateExpiresVerification,
                DateStartAccountLink = model.DateStartAccountLink,
                FirstName = model.FirstName,
                GuestEmail = model.GuestEmail,
                Key = model.Key,
                LastName = model.LastName,
                Password = model.Password,
                IsDeleted = model.IsDeleted,
                GuestExist = model.GuestExist,
                Permissions = model.Permissions,
                RowKey = model.RowKey,
                PartitionKey = model.PartitionKey,
                ETag = model.ETag
            };
        }

        public static AccountLinkVerificationModel ToModel(this AccountLinkVerification entity)
        {
            if (entity == null) return null;

            return new AccountLinkVerificationModel()
            {
                UserEmail = entity.AccountOwnerEmail,
                UserID = entity.AccountOwnerID,
                AgreeUserAgreement = entity.AgreeUserAgreement,
                DateCreated = entity.DateCreated,
                DateExpiresAccountLink = entity.DateExpiresAccountLink,
                DateExpiresVerification = entity.DateExpiresVerification,
                DateStartAccountLink = entity.DateStartAccountLink,
                FirstName = entity.FirstName,
                GuestEmail = entity.GuestEmail,
                Key = entity.Key,
                LastName = entity.LastName,
                Password = entity.Password,
                IsDeleted = entity.IsDeleted,
                GuestExist = entity.GuestExist,
                Permissions = entity.Permissions,
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                ETag = entity.ETag
            };
        }

        public static UsageDTO ToEntity(this UsageDTOModel model)
        {
            if (model == null) return null;

            return new UsageDTO()
            {
                AllowedFileCount = model.AllowedFileCount,
                AllowedGuestAccountCount = model.AllowedGuestAccountCount,
                AllowedLinkCount = model.AllowedLinkCount,
                AllowedStoredBytes = model.AllowedStoredBytes,
                FileCount = model.FileCount,
                LinkCount = model.LinkCount,
                LogCount = model.LogCount,
                StoredBytes = model.StoredBytes,
                GuestAccountCount = model.GuestAccountCount
            };
        }

        public static UsageDTOModel ToModel(this UsageDTO entity)
        {
            if (entity == null) return null;

            return new UsageDTOModel()
            {
                AllowedFileCount = entity.AllowedFileCount,
                AllowedGuestAccountCount = entity.AllowedGuestAccountCount,
                AllowedLinkCount = entity.AllowedLinkCount,
                AllowedStoredBytes = entity.AllowedStoredBytes,
                FileCount = entity.FileCount,
                LinkCount = entity.LinkCount,
                LogCount = entity.LogCount,
                StoredBytes = entity.StoredBytes,
                GuestAccountCount = entity.GuestAccountCount
            };
        }

        public static AccountUsagePremium ToEntity(this AccountUsagePremiumModel model)
        {
            if (model == null) return null;

            return new AccountUsagePremium()
            {
                AllowedFileCount = model.AllowedFileCount,
                AllowedGuestAccountCount = model.AllowedGuestAccountCount,
                AllowedLinkCount = model.AllowedLinkCount,
                AllowedStoredBytes = model.AllowedStoredBytes,
                UserID = model.UserID,
                DateExpires = model.DateExpires
            };
        }

        public static AccountUsagePremiumModel ToModel(this AccountUsagePremium entity)
        {
            if (entity == null) return null;

            return new AccountUsagePremiumModel()
            {
                AllowedFileCount = entity.AllowedFileCount,
                AllowedGuestAccountCount = entity.AllowedGuestAccountCount,
                AllowedLinkCount = entity.AllowedLinkCount,
                AllowedStoredBytes = entity.AllowedStoredBytes,
                UserID = entity.UserID,
                DateExpires = entity.DateExpires
            };
        }

        public static File ToEntity(this FileModel model)
        {
            if (model == null) return null;

            return new File()
            {
                UserEmail = model.UserEmail,
                UserID = model.UserID,
                BlobIconURL = model.BlobIconURL,
                BlobSASURI = model.BlobSASURI,
                DateBlobSASURLExpires = model.DateBlobSASURLExpires,
                DateCreated = model.DateCreated,
                DateLastDownload = model.DateLastDownload,
                DateLastModified = model.DateLastModified,
                DateLastShared = model.DateLastShared,
                FileExtension = model.FileExtension,
                FileName = model.FileName,
                FileSizeBytes = model.FileSizeBytes,
                UploadedByEmail = model.UploadedByEmail,
                UploadedByID = model.UploadedByID,
                RowKey = model.RowKey,
                PartitionKey = model.PartitionKey,
                ETag = model.ETag
            };
        }

        public static FileModel ToModel(this File entity)
        {
            if (entity == null) return null;

            return new FileModel()
            {
                UserEmail = entity.UserEmail,
                UserID = entity.UserID,
                BlobIconURL = entity.BlobIconURL,
                BlobSASURI = entity.BlobSASURI,
                DateBlobSASURLExpires = entity.DateBlobSASURLExpires,
                DateCreated = entity.DateCreated,
                DateLastDownload = entity.DateLastDownload,
                DateLastModified = entity.DateLastModified,
                DateLastShared = entity.DateLastShared,
                FileExtension = entity.FileExtension,
                FileName = entity.FileName,
                FileSizeBytes = entity.FileSizeBytes,
                UploadedByEmail = entity.UploadedByEmail,
                UploadedByID = entity.UploadedByID,
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                ETag = entity.ETag
            };
        }

        public static Link ToEntity(this LinkModel model)
        {
            if (model == null) return null;

            return new Link()
            {
                UserEmail = model.UserEmail,
                UserID = model.UserID,
                BlobIconURL = model.BlobIconURL,
                BlobName = model.BlobName,
                BlobSASURI = model.BlobSASURI,
                CreatedByEmail = model.CreatedByEmail,
                CreatedByID = model.CreatedByID,
                DateBlobSASURLExpires = model.DateBlobSASURLExpires,
                DateCreated = model.DateCreated,
                DateExpires = model.DateExpires,
                DateStart = model.DateStart,
                DateLastDownload = model.DateLastDownload,
                DownloadURL = model.DownloadURL,
                FileExtension = model.FileExtension,
                FileName = model.FileName,
                RowKey = model.RowKey,
                PartitionKey = model.PartitionKey,
                ETag = model.ETag
            };
        }

        public static LinkModel ToModel(this Link entity)
        {
            if (entity == null) return null;

            return new LinkModel()
            {
                UserEmail = entity.UserEmail,
                UserID = entity.UserID,
                BlobIconURL = entity.BlobIconURL,
                BlobName = entity.BlobName,
                BlobSASURI = entity.BlobSASURI,
                CreatedByEmail = entity.CreatedByEmail,
                CreatedByID = entity.CreatedByID,
                DateBlobSASURLExpires = entity.DateBlobSASURLExpires,
                DateCreated = entity.DateCreated,
                DateExpires = entity.DateExpires,
                DateStart = entity.DateStart,
                DateLastDownload = entity.DateLastDownload,
                DownloadURL = entity.DownloadURL,
                FileExtension = entity.FileExtension,
                FileName = entity.FileName,
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                ETag = entity.ETag
            };
        }

        public static Log ToEntity(this LogModel model)
        {
            if (model == null) return null;

            return new Log()
            {
                AccountOwnerEmail = model.AccountOwnerEmail,
                AccountOwnerID = model.AccountOwnerID,
                DateExpires = model.DateExpires,
                DateTriggered = model.DateTriggered,
                IpAddress = model.IpAddress,
                IsDeleted = model.IsDeleted,
                IsRead = model.IsRead,
                TriggeredByEmail = model.TriggeredByEmail,
                TriggeredByID = model.TriggeredByID,
                Type = model.Type,
                UserType = model.UserType,
                RowKey = model.RowKey,
                PartitionKey = model.PartitionKey,
                ETag = model.ETag
            };
        }

        public static LogModel ToModel(this Log entity)
        {
            if (entity == null) return null;

            return new LogModel()
            {
                AccountOwnerEmail = entity.AccountOwnerEmail,
                AccountOwnerID = entity.AccountOwnerID,
                DateExpires = entity.DateExpires,
                DateTriggered = entity.DateTriggered,
                IpAddress = entity.IpAddress,
                IsDeleted = entity.IsDeleted,
                IsRead = entity.IsRead,
                TriggeredByEmail = entity.TriggeredByEmail,
                TriggeredByID = entity.TriggeredByID,
                Type = entity.Type,
                UserType = entity.UserType,
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                ETag = entity.ETag
            };
        }

        public static Usage ToEntity(this UsageModel model)
        {
            if (model == null) return null;

            return new Usage()
            {
                RowKey = model.RowKey,
                PartitionKey = model.PartitionKey,
                ETag = model.ETag,
                UserEmail = model.UserEmail,
                DateExpires = model.DateExpires,
                DateTriggered = model.DateTriggered,
                Type = model.Type
            };
        }

        public static UsageModel ToModel(this Usage entity)
        {
            if (entity == null) return null;

            return new UsageModel()
            {
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                ETag = entity.ETag,
                UserEmail = entity.UserEmail,
                DateExpires = entity.DateExpires,
                DateTriggered = entity.DateTriggered,
                Type = entity.Type
            };
        }

        public static PermissionType ToEntity(this PermissionTypeModel model)
        {
            if (model == null) return null;

            return new PermissionType()
            {
                Name = model.Name,
                PermissionTypeID = model.PermissionTypeID
            };
        }

        public static PermissionTypeModel ToModel(this PermissionType entity)
        {
            if (entity == null) return null;

            return new PermissionTypeModel()
            {
                Name = entity.Name,
                PermissionTypeID = entity.PermissionTypeID
            };
        }

        public static Permission ToEntity(this PermissionModel model)
        {
            if (model == null) return null;

            return new Permission()
            {
                Name = model.Name,
                PermissionID = model.PermissionID,
                PermissionTypeID = model.PermissionTypeID
            };
        }

        public static PermissionModel ToModel(this Permission entity)
        {
            if (entity == null) return null;

            var name = entity.PermissionType != null ? entity.PermissionType.Name + entity.Name : null;

            return new PermissionModel()
            {
                Name = name,
                PermissionID = entity.PermissionID,
                PermissionTypeID = entity.PermissionTypeID,
                PermissionType = entity.PermissionType != null ? entity.PermissionType.ToModel() : null
            };
        }

        public static StatisticSeriesDTO ToEntity(this StatisticSeriesDTOModel model)
        {
            if (model == null) return null;

            return new StatisticSeriesDTO()
            {
                Data = model.Data,
                Name = model.Name
            };
        }

        public static StatisticSeriesDTOModel ToModel(this StatisticSeriesDTO entity)
        {
            if (entity == null) return null;

            return new StatisticSeriesDTOModel()
            {
                Data = entity.Data,
                Name = entity.Name
            };
        }

        public static StatisticsDTO ToEntity(this StatisticsDTOModel model)
        {
            if (model == null) return null;

            return new StatisticsDTO()
            {
                Series = model.Series.Select(o => o.ToEntity()).ToList(),
                xAxis = model.XAxis
            };
        }

        public static StatisticsDTOModel ToModel(this StatisticsDTO entity)
        {
            if (entity == null) return null;

            return new StatisticsDTOModel()
            {
                Series = entity.Series.Select(o => o.ToModel()).ToList(),
                XAxis = entity.xAxis
            };
        }

        public static UsageAndStatisticsDTO ToEntity(this UsageAndStatisticsDTOModel model)
        {
            if (model == null) return null;

            return new UsageAndStatisticsDTO()
            {
                AccountStatisticsDTO = model.AccountStatisticsDTOModel.ToEntity(),
                UsageDTO = model.UsageDTOModel.ToEntity()
            };
        }

        public static UsageAndStatisticsDTOModel ToModel(this UsageAndStatisticsDTO entity)
        {
            if (entity == null) return null;

            return new UsageAndStatisticsDTOModel()
            {
                AccountStatisticsDTOModel = entity.AccountStatisticsDTO.ToModel(),
                UsageDTOModel = entity.UsageDTO.ToModel()
            };
        }
    }
}
