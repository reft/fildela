using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UserModels
{
    public class SignInGuestAccountLinksViewModel
    {
        public List<UserSignInGuestAccountLinksModel> AccountLinks { get; set; }

        public string ReturnURL { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class UserSignInGuestAccountLinksModel
    {
        [Required]
        public int AccountOwnerID { get; set; }

        public string AccountOwnerEmail { get; set; }

        public string ReturnURL { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        public string Password { get; set; }
    }
}