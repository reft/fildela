using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public class Permission
    {
        public Permission()
        {
            AccountLinkPermissions = new HashSet<AccountLinkPermission>();
        }

        [Key]
        public int PermissionID { get; set; }

        [ForeignKey("PermissionType")]
        public int PermissionTypeID { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Name { get; set; }

        public virtual ICollection<AccountLinkPermission> AccountLinkPermissions { get; set; }
        public virtual PermissionType PermissionType { get; set; }
    }
}
