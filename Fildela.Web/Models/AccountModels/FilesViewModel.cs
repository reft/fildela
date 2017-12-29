using Fildela.Business.Domains.Account.Models;
using System.Collections.Generic;

namespace Fildela.Web.Models.AccountModels
{
    public class FilesViewModel : AccountViewModel
    {
        public List<FileModel> Files { get; set; }

        public long StoredBytes { get; set; }

        public int AllowedFileCount { get; set; }

        public long AllowedStoredBytes { get; set; }
    }
}