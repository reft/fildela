using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Fildela.Web.Models.AdministrationModels
{
    public class InsertNewsViewModel
    {
        [Required]
        [Range(1, Int32.MaxValue)]
        public int CategoryID { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 6)]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(Int32.MaxValue, MinimumLength = 50)]
        public string Text { get; set; }

        [Required]
        public HttpPostedFileBase Image { get; set; }
    }
}