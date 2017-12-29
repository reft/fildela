using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Fildela.Data.Storage.Models
{
    public class Link : TableEntity
    {
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