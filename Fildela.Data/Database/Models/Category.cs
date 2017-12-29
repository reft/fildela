using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public class Category
    {
        public Category()
        {
            News = new HashSet<News>();
        }

        [Key]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryType")]
        public int CategoryTypeID { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Name { get; set; }

        public virtual ICollection<News> News { get; set; }
        public virtual CategoryType CategoryType { get; set; }
    }
}
