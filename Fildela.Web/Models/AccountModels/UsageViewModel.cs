
namespace Fildela.Web.Models.AccountModels
{
    public class UsageViewModel : AccountViewModel
    {
        public int AllowedFileCount { get; set; }

        public int AllowedGuestAccountCount { get; set; }

        public int AllowedLinkCount { get; set; }

        public long AllowedStoredBytes { get; set; }

        public long StoredBytes { get; set; }

        public StatisticsViewModel StatisticsViewModel { get; set; }
    }
}