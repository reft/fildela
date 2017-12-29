using System;
using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.ContactModels
{
    public class ContactViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 6)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string Email { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int CategoryID { get; set; }

        [StringLength(5000, MinimumLength = 25)]
        [Required(AllowEmptyStrings = false)]
        public string Message { get; set; }
    }
}