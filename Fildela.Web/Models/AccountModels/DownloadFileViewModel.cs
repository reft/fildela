using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.AccountModels
{
    public class DownloadFileViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string BlobName { get; set; }
    }
}