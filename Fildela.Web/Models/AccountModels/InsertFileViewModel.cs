using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.AccountModels
{
    public class InsertFileViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        public string FileName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string BlobName { get; set; }
    }
}