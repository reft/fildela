
namespace Fildela.Business.Domains.UploadDirectly.Models
{
    public class UploadDirectlyUsageDTOModel
    {
        public int FileCount { get; set; }

        public long StoredBytes { get; set; }

        public int AllowedTotalFileCount { get; set; }

        public int AllowedTotalStoredBytes { get; set; }

        public int AllowedEmailCountPerFile { get; set; }

        public int FileLifetimeInHours { get; set; }
    }
}
