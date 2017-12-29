using System;
using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.AccountModels
{
    public class InsertLinkViewModel
    {
        [DataType(DataType.Date)]
        public DateTime DateStart { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateExpires { get; set; }

        [DataType(DataType.Date)]
        public DateTime CurrentTime { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string File { get; set; }
    }
}