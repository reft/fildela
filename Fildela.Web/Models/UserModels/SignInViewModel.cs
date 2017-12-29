using System;
using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UserModels
{
    public class SignInViewModel
    {
        [Required]
        [Range(1, Int32.MaxValue)]
        public int UserType { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string Email { get; set; }

        [StringLength(150, MinimumLength = 6)]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}