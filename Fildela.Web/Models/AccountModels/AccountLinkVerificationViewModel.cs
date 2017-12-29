using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.AccountModels
{
    public class AccountLinkVerificationViewModel
    {
        [StringLength(150, MinimumLength = 6)]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string AccountOwnerEmail { get; set; }

        [StringLength(150, MinimumLength = 6)]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string GuestEmail { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Key { get; set; }
    }
}