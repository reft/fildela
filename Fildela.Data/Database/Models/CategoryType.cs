using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public class CategoryType
    {
        public CategoryType()
        {
            Categories = new HashSet<Category>();
        }

        [Key]
        public int CategoryTypeID { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Name { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}
