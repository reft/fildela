using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Fildela.Data.Storage.Models
{
    public class UploadDirectly : TableEntity
    {
        public string BlobName { get; set; }
        public string UploadedByIpAddress { get; set; }
        public long FileSizeBytes { get; set; }
        public string FileName { get; set; }
        public string BlobSASURI { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateExpires { get; set; }
    }
}