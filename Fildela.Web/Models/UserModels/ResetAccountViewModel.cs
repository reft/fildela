using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UserModels
{
    public class ResetAccountViewModel
    {
        [StringLength(150, MinimumLength = 6)]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}