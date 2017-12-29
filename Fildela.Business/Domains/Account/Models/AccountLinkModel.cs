using System;

namespace Fildela.Business.Domains.Account.Models
{
    public class AccountLinkModel
    {
        public int UserID { get; set; }

        public int GuestID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateExpires { get; set; }
    }
}
