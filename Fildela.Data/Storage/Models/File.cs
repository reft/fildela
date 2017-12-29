using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Fildela.Data.Storage.Models
{
    public class File : TableEntity
    {
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
