using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fildela.Web.Models.UserModels
{
    public class DeleteAccountAuthenticationProviderModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public int AuthenticationProviderID { get; set; }
    }
}