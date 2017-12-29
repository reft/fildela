using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public class AccountAuthenticationProvider
    {
        public AccountAuthenticationProvider()
        {

        }

        public AccountAuthenticationProvider(int accountID, int authenticationProviderID)
        {
            this.AccountID = accountID;
            this.AuthenticationProviderID = authenticationProviderID;
        }

        [Key, Column(Order = 0), ForeignKey("User")]
        public int AccountID { get; set; }

        [Key, Column(Order = 1), ForeignKey("AuthenticationProvider")]
        public int AuthenticationProviderID { get; set; }

        [Required]
        [StringLength(1000)]
        [Column(TypeName = "VARCHAR")]
        public string KeyHash { get; set; }

        [Required]
        [StringLength(1000)]
        [Column(TypeName = "VARCHAR")]
        public string KeySalt { get; set; }

        public virtual User User { get; set; }
        public virtual AuthenticationProvider AuthenticationProvider { get; set; }
    }
}
