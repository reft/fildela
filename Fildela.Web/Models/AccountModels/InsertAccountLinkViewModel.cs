using System;
using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.AccountModels
{
    public class InsertAccountLinkViewModel
    {
        [StringLength(150, MinimumLength = 6)]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string Email { get; set; }

        [CompareAttribute("Email")]
        public string ConfirmEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateStart { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateExpires { get; set; }

        [DataType(DataType.Date)]
        public DateTime CurrentTime { get; set; }

        public string Permissions { get; set; }
    }
}