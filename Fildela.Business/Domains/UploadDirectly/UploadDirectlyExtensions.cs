using Fildela.Business.Domains.UploadDirectly.Models;
using Fildela.Data.CustomModels.UploadDirectly;

namespace Fildela.Business.Domains.UploadDirectly
{
    public static class UploadDirectlyExtensions
    {
        public static UploadDirectlyDTO ToEntity(this UploadDirectlyDTOModel model)
        {
            if (model == null) return null;

            return new UploadDirectlyDTO()
            {
                BlobURL = model.BlobURL,
                FileName = model.FileName,
                FileSize = model.FileSize
            };
        }

        public static UploadDirectlyDTOModel ToModel(this UploadDirectlyDTO entity)
        {
            if (entity == null) return null;

            return new UploadDirectlyDTOModel()
            {
                BlobURL = entity.BlobURL,
                FileName = entity.FileName,
                FileSize = entity.FileSize
            };
        }

        public static UploadDirectlyEmailDTO ToEntity(this UploadDirectlyEmailDTOModel model)
        {
            if (model == null) return null;

            return new UploadDirectlyEmailDTO()
            {
                AllowedEmailCountPerFile = model.AllowedEmailCountPerFile,
                EmailCountForCurrentFile = model.EmailCountForCurrentFile
            };
        }

        public static UploadDirectlyEmailDTOModel ToModel(this UploadDirectlyEmailDTO entity)
        {
            if (entity == null) return null;

            return new UploadDirectlyEmailDTOModel()
            {
                AllowedEmailCountPerFile = entity.AllowedEmailCountPerFile,
                EmailCountForCurrentFile = entity.EmailCountForCurrentFile
            };
        }

        public static Fildela.Data.Storage.Models.UploadDirectly ToEntity(this UploadDirectlyModel model)
        {
            if (model == null) return null;

            return new Data.Storage.Models.UploadDirectly()
            {
                BlobName = model.BlobName,
                BlobSASURI = model.BlobSASURI,
                DateCreated = model.DateCreated,
                DateExpires = model.DateExpires,
                Email = model.Email,
                FileName = model.FileName,
                FileSizeBytes = model.FileSizeBytes,
                UploadedByIpAddress = model.UploadedByIpAddress,
                RowKey = model.RowKey,
                PartitionKey = model.PartitionKey,
                ETag = model.ETag
            };
        }

        public static UploadDirectlyModel ToModel(this Fildela.Data.Storage.Models.UploadDirectly entity)
        {
            if (entity == null) return null;

            return new UploadDirectlyModel()
            {
                BlobName = entity.BlobName,
                BlobSASURI = entity.BlobSASURI,
                DateCreated = entity.DateCreated,
                DateExpires = entity.DateExpires,
                Email = entity.Email,
                FileName = entity.FileName,
                FileSizeBytes = entity.FileSizeBytes,
                UploadedByIpAddress = entity.UploadedByIpAddress,
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                ETag = entity.ETag
            };
        }

        public static UploadDirectlyUsageDTO ToEntity(this UploadDirectlyUsageDTOModel model)
        {
            if (model == null) return null;

            return new UploadDirectlyUsageDTO()
            {
                AllowedEmailCountPerFile = model.AllowedEmailCountPerFile,
                AllowedTotalFileCount = model.AllowedTotalFileCount,
                AllowedTotalStoredBytes = model.AllowedTotalStoredBytes,
                FileCount = model.FileCount,
                FileLifetimeInHours = model.FileLifetimeInHours,
                StoredBytes = model.StoredBytes
            };
        }

        public static UploadDirectlyUsageDTOModel ToModel(this UploadDirectlyUsageDTO entity)
        {
            if (entity == null) return null;

            return new UploadDirectlyUsageDTOModel()
            {
                AllowedEmailCountPerFile = entity.AllowedEmailCountPerFile,
                AllowedTotalFileCount = entity.AllowedTotalFileCount,
                AllowedTotalStoredBytes = entity.AllowedTotalStoredBytes,
                FileCount = entity.FileCount,
                FileLifetimeInHours = entity.FileLifetimeInHours,
                StoredBytes = entity.StoredBytes
            };
        }
    }
}
