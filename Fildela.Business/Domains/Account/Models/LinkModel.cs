using Fildela.Data.Helpers;
using System;

namespace Fildela.Business.Domains.Account.Models
{
    public class LinkModel
    {
        public LinkModel()
        {
            this.PartitionKey = "links";
        }

        public LinkModel(string rowKey, int userID, string userEmail, int createdByID,
    string createdByEmail, string fileName, string fileExtension, string blobName, string blobIconURL, string downloadLink, DateTime dateStart, DateTime dateExpire)
        {
            this.PartitionKey = "links";
            this.RowKey = rowKey;
            this.UserID = userID;
            this.UserEmail = userEmail;
            this.CreatedByID = createdByID;
            this.CreatedByEmail = createdByEmail;
            this.FileName = fileName;
            this.FileExtension = fileExtension;
            this.BlobName = blobName;
            this.BlobIconURL = blobIconURL;
            this.BlobSASURI = string.Empty;
            this.DownloadURL = downloadLink;
            this.DateBlobSASURLExpires = DataTimeZoneExtensions.GetCurrentDate();
            this.DateCreated = DataTimeZoneExtensions.GetCurrentDate();
            this.DateStart = dateStart;
            this.DateExpires = dateExpire;
            this.DateLastDownload = DataTimeZoneExtensions.GetCurrentDate().AddMinutes(-15);
        }

        public string ETag { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public int UserID { get; set; }

        public string UserEmail { get; set; }

        public int CreatedByID { get; set; }

        public string CreatedByEmail { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string BlobName { get; set; }

        public string BlobSASURI { get; set; }

        public DateTime DateBlobSASURLExpires { get; set; }

        public string DownloadURL { get; set; }

        public string BlobIconURL { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateExpires { get; set; }

        public DateTime DateLastDownload { get; set; }
    }
}
