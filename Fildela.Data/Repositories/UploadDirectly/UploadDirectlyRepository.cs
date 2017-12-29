using Fildela.Data.CustomModels.UploadDirectly;
using Fildela.Data.Database.DataLayer;
using Fildela.Data.Database.Models;
using Fildela.Data.Helpers;
using Fildela.Data.Storage.Services;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Fildela.Data.Repositories.UploadDirectly
{
    public class UploadDirectlyRepository : RepositoryBase, IUploadDirectlyRepository
    {
        private readonly string UploadDirectlyAllowedFileCount = ConfigurationManager.AppSettings["UploadDirectlyAllowedFileCount"];
        private readonly string UploadDirectlyAllowedStoredBytes = ConfigurationManager.AppSettings["UploadDirectlyAllowedStoredBytes"];
        private readonly string UploadDirectlyAllowedEmailCountPerFile = ConfigurationManager.AppSettings["UploadDirectlyAllowedEmailCountPerFile"];
        private readonly string UploadDirectlyFileLifetimeInHours = ConfigurationManager.AppSettings["UploadDirectlyFileLifetimeInHours"];

        public UploadDirectlyRepository(DataLayer db, CloudStorageServices storage) : base(db, storage) { }

        public string GetUploadSASURI(string blobname)
        {
            CloudBlobContainer blobContainer = Storage.GetCloudBlobUploadDirectlyContainer();
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

        public string GetDownloadSASURI(string blobname, string filename, int ExpiredInHours)
        {
            CloudBlobContainer blobContainer = Storage.GetCloudBlobUploadDirectlyContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobname);

            string sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(ExpiredInHours),
                Permissions = SharedAccessBlobPermissions.Read
            }, new SharedAccessBlobHeaders()
            {
                ContentDisposition = "Attachment; filename=" + filename
            });

            string blobSASURI = (string.Format(CultureInfo.InvariantCulture, "{0}{1}", blob.Uri, sas));

            return blobSASURI;
        }

        public UploadDirectlyUsageDTO GetUploadDirectlyUsage(string ipAddress)
        {
            UploadDirectlyUsageDTO uploadDirectlyUsage = GetUploadDirectlyAllowedUsage(ipAddress);

            //Get active files and bytes (AZURE TABLE)
            CloudTable uploadDirectlyTable = Storage.GetCloudUploadDirectlyTable();
            TableQuery<Fildela.Data.Storage.Models.UploadDirectly> uploadDirectlyQuery = uploadDirectlyTable.CreateQuery<Fildela.Data.Storage.Models.UploadDirectly>();

            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            List<Fildela.Data.Storage.Models.UploadDirectly> uploadDirectlyEntity = (from r in uploadDirectlyTable.ExecuteQuery(uploadDirectlyQuery)
                                                                                     where r.PartitionKey == "uploaddirectlys" &&
                                                                                     r.UploadedByIpAddress == ipAddress &&
                                                                                     String.IsNullOrEmpty(r.Email) &&
                                                                                     (r.DateExpires - currentTime).TotalDays > 0
                                                                                     select r).ToList();

            if (uploadDirectlyEntity != null && uploadDirectlyEntity.Count > 0)
            {
                foreach (var item in uploadDirectlyEntity)
                {
                    uploadDirectlyUsage.StoredBytes += item.FileSizeBytes;
                    uploadDirectlyUsage.FileCount += 1;
                }
            }

            return uploadDirectlyUsage;
        }

        public UploadDirectlyDTO GetBlobAttributes(string blobName)
        {
            UploadDirectlyDTO uploadDirectlyBlobFileModel = new UploadDirectlyDTO();

            //Get blob file
            CloudBlobContainer blobFileContainer = Storage.GetCloudBlobUploadDirectlyContainer();
            CloudBlockBlob blob = blobFileContainer.GetBlockBlobReference(blobName);

            if (blob.Exists())
            {
                blob.FetchAttributes();

                uploadDirectlyBlobFileModel = new UploadDirectlyDTO()
                {
                    BlobURL = blob.Uri.ToString(),
                    FileName = blob.Name,
                    FileSize = blob.Properties.Length
                };
            }

            return uploadDirectlyBlobFileModel;
        }

        public void DeleteBlob(string blobName)
        {
            CloudBlobContainer blobContainer = Storage.GetCloudBlobUploadDirectlyContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);
            if (blob.Exists())
            {
                blob.Delete();
            }
        }

        public bool FileOwnedByAnyone(string blobName)
        {
            CloudTable uploadDirectlyTable = Storage.GetCloudUploadDirectlyTable();
            TableQuery<Fildela.Data.Storage.Models.UploadDirectly> tableQuery = new TableQuery<Fildela.Data.Storage.Models.UploadDirectly>();
            bool fileOwnedByAnyone = (from u in uploadDirectlyTable.ExecuteQuery(tableQuery)
                                      where u.PartitionKey == "uploaddirectlys" && u.BlobName == blobName
                                      select u).Any();

            return fileOwnedByAnyone;
        }

        public void InsertUploadDirectly(Fildela.Data.Storage.Models.UploadDirectly uploadDirectly)
        {
            CloudTable uploadDirectlyTable = Storage.GetCloudUploadDirectlyTable();

            TableOperation insertOperation = TableOperation.InsertOrReplace(uploadDirectly);
            uploadDirectlyTable.Execute(insertOperation);
        }

        public bool BlobExist(string blobName)
        {
            CloudStorageServices CloudStorageService = new CloudStorageServices();
            CloudBlobContainer blobContainer = CloudStorageService.GetCloudBlobUploadDirectlyContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);

            return blob.Exists();
        }

        public int GetSentEmailCountForFile(string blobName, string ipAddress)
        {
            CloudTable uploadDirectlyTable = Storage.GetCloudUploadDirectlyTable();
            TableQuery<Fildela.Data.Storage.Models.UploadDirectly> tableQuery = new TableQuery<Fildela.Data.Storage.Models.UploadDirectly>();

            int count = (from c in uploadDirectlyTable.ExecuteQuery(tableQuery)
                         where c.PartitionKey == "uploaddirectlys" &&
                         c.BlobName == blobName &&
                         c.UploadedByIpAddress == ipAddress &&
                         !String.IsNullOrEmpty(c.Email)
                         select c).Count();

            return count;
        }

        public UploadDirectlyEmailDTO GetUploadDirectlyEmailModel(string blobName, string ipAddress)
        {
            UploadDirectlyUsageDTO uploadDirectlyUsage = GetUploadDirectlyAllowedUsage(ipAddress);

            CloudTable uploadDirectlyTable = Storage.GetCloudUploadDirectlyTable();
            TableQuery<Fildela.Data.Storage.Models.UploadDirectly> tableQuery = new TableQuery<Fildela.Data.Storage.Models.UploadDirectly>();

            int emailCount = (from c in uploadDirectlyTable.ExecuteQuery(tableQuery)
                              where c.PartitionKey == "uploaddirectlys" &&
                              c.BlobName == blobName &&
                              c.UploadedByIpAddress == ipAddress &&
                              !String.IsNullOrEmpty(c.Email)
                              select c).Count();

            UploadDirectlyEmailDTO uploadDirectlyEmailModel = new UploadDirectlyEmailDTO()
            {
                EmailCountForCurrentFile = emailCount,
                AllowedEmailCountPerFile = uploadDirectlyUsage.AllowedEmailCountPerFile
            };

            return uploadDirectlyEmailModel;
        }

        public bool DownloadLinkSentToEmail(string email, string blobName)
        {
            CloudTable uploadDirectlyTable = Storage.GetCloudUploadDirectlyTable();
            TableQuery<Fildela.Data.Storage.Models.UploadDirectly> tableQuery = new TableQuery<Fildela.Data.Storage.Models.UploadDirectly>();
            bool linkSentToEmail = (from l in uploadDirectlyTable.ExecuteQuery(tableQuery)
                                    where l.PartitionKey == "uploaddirectlys" && l.BlobName == blobName && l.Email == email.ToLower().Trim()
                                    select l).Any();

            return linkSentToEmail;
        }

        public Fildela.Data.Storage.Models.UploadDirectly GetUploadDirectlyFromRowKey(string rowKey)
        {
            CloudTable linkTable = Storage.GetCloudUploadDirectlyTable();

            TableQuery<Fildela.Data.Storage.Models.UploadDirectly> uploadDirectlyQuery = new TableQuery<Fildela.Data.Storage.Models.UploadDirectly>();
            Fildela.Data.Storage.Models.UploadDirectly uploadDirectlyEntity = (from u in linkTable.ExecuteQuery(uploadDirectlyQuery)
                                                                               where u.PartitionKey == "uploaddirectlys" &&
                                                                               u.RowKey == rowKey
                                                                               select u).SingleOrDefault();

            return uploadDirectlyEntity;
        }

        public UploadDirectlyUsageDTO GetUploadDirectlyAllowedUsage(string ipAddress)
        {
            UploadDirectlyUsageDTO uploadDirectlyUsageDTO = new UploadDirectlyUsageDTO();

            uploadDirectlyUsageDTO.AllowedTotalFileCount = Convert.ToInt32(UploadDirectlyAllowedFileCount);
            uploadDirectlyUsageDTO.AllowedTotalStoredBytes = Convert.ToInt32(UploadDirectlyAllowedStoredBytes);
            uploadDirectlyUsageDTO.FileLifetimeInHours = Convert.ToInt32(UploadDirectlyFileLifetimeInHours);
            uploadDirectlyUsageDTO.AllowedEmailCountPerFile = Convert.ToInt32(UploadDirectlyAllowedEmailCountPerFile);

            return uploadDirectlyUsageDTO;
        }
    }
}
