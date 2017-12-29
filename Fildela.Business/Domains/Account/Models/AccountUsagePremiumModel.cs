using System;

namespace Fildela.Business.Domains.Account.Models
{
    public class AccountUsagePremiumModel
    {
        public int UserID { get; set; }

        public int AllowedFileCount { get; set; }

        public int AllowedGuestAccountCount { get; set; }

        public int AllowedLinkCount { get; set; }

        public long AllowedStoredBytes { get; set; }

        public DateTime DateExpires { get; set; }
    }
}
