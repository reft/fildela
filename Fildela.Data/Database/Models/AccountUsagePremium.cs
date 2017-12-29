using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public class AccountUsagePremium
    {
        public AccountUsagePremium()
        {
        }

        [Key, ForeignKey("User")]
        public int UserID { get; set; }

        public int AllowedFileCount { get; set; }
        public int AllowedGuestAccountCount { get; set; }
        public int AllowedLinkCount { get; set; }
        public long AllowedStoredBytes { get; set; }
        public DateTime DateExpires { get; set; }

        public virtual User User { get; set; }
    }
}
