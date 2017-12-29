using Fildela.Data.Helpers;
using System;

namespace Fildela.Business.Domains.Account.Models
{
    public class FileModel
    {
        public FileModel()
        {
            this.PartitionKey = "files";
        }

        public FileModel(string rowKey, int userID, string userEmail, int uploadedByID,
    string uploadedByEmail, string fileName, long fileSizeBytes, string fileExtension)
        {
            this.PartitionKey = "files";
            this.RowKey = rowKey;
            this.UserID = userID;
            this.UserEmail = userEmail;
            this.UploadedByID = uploadedByID;
            this.UploadedByEmail = uploadedByEmail;
            this.FileName = fileName;
            this.FileSizeBytes = fileSizeBytes;
            this.FileExtension = fileExtension;
            this.DateCreated = DataTimeZoneExtensions.GetCurrentDate();
            this.DateBlobSASURLExpires = DataTimeZoneExtensions.GetCurrentDate();
            this.DateLastModified = DataTimeZoneExtensions.GetCurrentDate().AddMinutes(-15);
            this.DateLastDownload = DataTimeZoneExtensions.GetCurrentDate().AddMinutes(-15);
            this.DateLastShared = DataTimeZoneExtensions.GetCurrentDate().AddMinutes(-15);
            this.BlobIconURL = string.Empty;
            this.BlobSASURI = string.Empty;
        }

        public string ETag { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public int UserID { get; set; }

        public string UserEmail { get; set; }

        public int UploadedByID { get; set; }

        public string UploadedByEmail { get; set; }

        public string FileName { get; set; }

        public long FileSizeBytes { get; set; }

        public string FileExtension { get; set; }

        public string BlobIconURL { get; set; }

        public string BlobSASURI { get; set; }

        public DateTime DateBlobSASURLExpires { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateLastModified { get; set; }

        public DateTime DateLastDownload { get; set; }

        public DateTime DateLastShared { get; set; }
    }
}
