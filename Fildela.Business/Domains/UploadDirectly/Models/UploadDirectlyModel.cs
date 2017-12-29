using Fildela.Data.Helpers;
using System;

namespace Fildela.Business.Domains.UploadDirectly.Models
{
    public class UploadDirectlyModel
    {
        public UploadDirectlyModel()
        {
            this.PartitionKey = "uploaddirectlys";
        }

        public UploadDirectlyModel(string rowKey, string uploadedByIpAddress, long fileSizeBytes, string blobName,
            string fileName, string blobSASURI, int fileLifetimeInHours)
        {
            this.PartitionKey = "uploaddirectlys";
            this.RowKey = rowKey;
            this.UploadedByIpAddress = uploadedByIpAddress;
            this.FileSizeBytes = fileSizeBytes;
            this.DateCreated = DataTimeZoneExtensions.GetCurrentDate();
            this.DateExpires = DataTimeZoneExtensions.GetCurrentDate().AddHours(fileLifetimeInHours);
            this.BlobName = blobName;
            this.FileName = fileName;
            this.BlobSASURI = blobSASURI;
            this.Email = String.Empty;
        }

        public string ETag { get; set; }

        public string PartitionKey { get; set; }

        public string BlobName { get; set; }

        public string UploadedByIpAddress { get; set; }

        public long FileSizeBytes { get; set; }

        public string FileName { get; set; }

        public string BlobSASURI { get; set; }

        public string Email { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateExpires { get; set; }

        public string RowKey { get; set; }
    }
}
