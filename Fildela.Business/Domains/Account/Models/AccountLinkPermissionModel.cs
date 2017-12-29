
namespace Fildela.Business.Domains.Account.Models
{
    public class AccountLinkPermissionModel
    {
        public int UserID { get; set; }

        public int GuestID { get; set; }

        public int PermissionID { get; set; }

        public PermissionModel Permission { get; set; }
    }
}
