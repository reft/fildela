using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UserModels
{
    public class InsertAccountAuthenticationProviderViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string Email { get; set; }

        [StringLength(150, MinimumLength = 6)]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ProviderName { get; set; }

        [StringLength(10000)]
        [Required(AllowEmptyStrings = false)]
        public string ProviderKey { get; set; }

        public string ReturnURL { get; set; }
    }
}