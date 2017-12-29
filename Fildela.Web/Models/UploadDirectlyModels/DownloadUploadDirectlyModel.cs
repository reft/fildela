using System.ComponentModel.DataAnnotations;

namespace Fildela.Web.Models.UploadDirectlyModels
{
    public class DownloadUploadDirectlyModel
    {
        [Required(AllowEmptyStrings = false)]
        public string RowKey { get; set; }
    }
}