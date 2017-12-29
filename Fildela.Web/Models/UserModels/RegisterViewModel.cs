using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UserModels
{
    public class RegisterViewModel
    {
        public bool isTrue { get { return true; } }

        [Required]
        [Compare("isTrue")]
        public bool AgreeUserAgreement { get; set; }

        [StringLength(150, MinimumLength = 6)]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Compare("Email")]
        public string ConfirmEmail { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}