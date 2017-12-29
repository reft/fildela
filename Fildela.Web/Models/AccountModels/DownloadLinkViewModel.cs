using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.AccountModels
{
    public class DownloadLinkViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string RowKey { get; set; }
    }
}