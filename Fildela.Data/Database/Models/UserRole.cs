using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public class UserRole
    {
        public UserRole()
        {

        }

        public UserRole(int userID, int roleID)
        {
            this.UserID = userID;
            this.RoleID = roleID;
        }

        [Key, Column(Order = 0), ForeignKey("User")]
        public int UserID { get; set; }

        [Key, Column(Order = 1), ForeignKey("Role")]
        public int RoleID { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
