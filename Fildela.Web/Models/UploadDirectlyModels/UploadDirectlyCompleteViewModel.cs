using System;

namespace Fildela.Web.Models.UploadDirectlyModels
{
    public class UploadDirectlyCompleteViewModel
    {
        public string DownloadLink { get; set; }

        public string FileName { get; set; }

        public long FileSize { get; set; }

        public string BlobName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}