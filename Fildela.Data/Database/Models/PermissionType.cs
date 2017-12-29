using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public class PermissionType
    {
        public PermissionType()
        {
            Permissions = new HashSet<Permission>();
        }

        [Key]
        public int PermissionTypeID { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Name { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
