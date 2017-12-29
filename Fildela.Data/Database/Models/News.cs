using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public class News
    {
        public News()
        {

        }

        [Key]
        public int NewsID { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        [ForeignKey("Account")]
        public int PublishedByID { get; set; }

        [StringLength(300)]
        [Column(TypeName = "VARCHAR")]
        public string PublishedByFullName { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR")]
        public string PublishedByEmail { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        [Column(TypeName = "VARCHAR")]
        public string Title { get; set; }

        [Required]
        public string PreviewText { get; set; }

        [Required]
        public string TextBlobName { get; set; }

        [Required]
        public string ImageBlobURL { get; set; }

        public DateTime DatePublished { get; set; }

        public virtual Category Category { get; set; }
        public virtual Account Account { get; set; }
    }
}
