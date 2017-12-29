
namespace Fildela.Business.Domains.Account.Models
{
    public class UsageDTOModel
    {
        public int FileCount { get; set; }

        public int GuestAccountCount { get; set; }

        public int LinkCount { get; set; }

        public int LogCount { get; set; }

        public long StoredBytes { get; set; }

        public int AllowedFileCount { get; set; }

        public int AllowedGuestAccountCount { get; set; }

        public int AllowedLinkCount { get; set; }

        public long AllowedStoredBytes { get; set; }
    }
}
