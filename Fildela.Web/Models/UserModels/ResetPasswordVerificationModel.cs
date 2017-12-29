using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UserModels
{
    public class ResetPasswordVerificationModel
    {
        [Required(AllowEmptyStrings = false)]
        public string RowKey { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Key { get; set; }
    }
}