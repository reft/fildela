using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.AccountModels
{
    public class GetUploadSASURIViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        public string FileName { get; set; }

        [Required]
        public long FileSize { get; set; }
    }
}