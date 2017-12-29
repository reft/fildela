using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UserModels
{
    public class ResetPasswordViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string Email { get; set; }
    }
}