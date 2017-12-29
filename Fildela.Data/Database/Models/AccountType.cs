using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fildela.Data.Database.Models
{
    public class AccountType
    {
        [Key]
        public int AccountTypeID { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR")]
        public string Name { get; set; }
    }
}
