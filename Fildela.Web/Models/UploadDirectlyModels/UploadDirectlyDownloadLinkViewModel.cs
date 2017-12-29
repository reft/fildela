using System;
using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UploadDirectlyModels
{
    public class UploadDirectlyDownloadLinkViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string FileName { get; set; }

        [Required]
        public long FileSize { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string BlobName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string URI { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}