
namespace Fildela.Web.Models.AccountModels
{
    public class AccountViewModel
    {
        public string UserEmail { get; set; }

        public int FileCount { get; set; }

        public int GuestAccountCount { get; set; }

        public int LinkCount { get; set; }

        public int LogCount { get; set; }

        public bool FileRead { get; set; }

        public bool FileWrite { get; set; }

        public bool FileEdit { get; set; }

        public bool LinkRead { get; set; }

        public bool LinkWrite { get; set; }

        public bool LinkEdit { get; set; }
    }
}