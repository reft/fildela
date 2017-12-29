
namespace Fildela.Business.Domains.Account.Models
{
    public class AccountLinkPermissionBoolDTOModel
    {
        public bool FileRead { get; set; }

        public bool FileWrite { get; set; }

        public bool FileEdit { get; set; }

        public bool LinkRead { get; set; }

        public bool LinkWrite { get; set; }

        public bool LinkEdit { get; set; }
    }
}
