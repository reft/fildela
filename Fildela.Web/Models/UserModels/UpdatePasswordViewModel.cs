using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UserModels
{
    public class UpdatePasswordViewModel
    {
        [StringLength(150, MinimumLength = 6)]
        [Required(AllowEmptyStrings = false)]
        public string CurrentPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [CompareAttribute("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}