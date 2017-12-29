using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.AccountModels
{
    public class UpdateFileNameViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        public string FileName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string RowKey { get; set; }
    }
}