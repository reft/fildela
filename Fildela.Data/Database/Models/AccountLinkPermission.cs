using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public class AccountLinkPermission
    {
        public AccountLinkPermission()
        {
        }

        public AccountLinkPermission(int userID, int guestID, int permissionID)
        {
            this.UserID = userID;
            this.GuestID = guestID;
            this.PermissionID = permissionID;
        }

        [Key, ForeignKey("AccountLink"), Column(Order = 0)]
        public int UserID { get; set; }

        [Key, ForeignKey("AccountLink"), Column(Order = 1)]
        public int GuestID { get; set; }

        [Key, ForeignKey("Permission"), Column(Order = 2)]
        public int PermissionID { get; set; }

        public virtual AccountLink AccountLink { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
