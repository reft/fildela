using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UserModels
{
    public class DeleteAccountViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        public string Password { get; set; }
    }
}