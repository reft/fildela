using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UserModels
{
    public class UpdateEmailViewModel
    {
        [StringLength(150, MinimumLength = 6)]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string NewEmail { get; set; }

        [CompareAttribute("NewEmail")]
        public string ConfirmNewEmail { get; set; }
    }
}