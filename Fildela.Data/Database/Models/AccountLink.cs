using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public class AccountLink
    {
        public AccountLink()
        {
        }

        public AccountLink(int userID, int guestID, DateTime dateCreated, DateTime dateStart, DateTime dateExpires)
        {
            this.UserID = userID;
            this.GuestID = guestID;
            this.DateCreated = dateCreated;
            this.DateStart = dateStart;
            this.DateExpires = dateExpires;
        }

        [Key, Column(Order = 0), ForeignKey("User")]
        public int UserID { get; set; }

        [Key, Column(Order = 1), ForeignKey("Guest")]
        public int GuestID { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateExpires { get; set; }

        public virtual User User { get; set; }
        public virtual Guest Guest { get; set; }

        public virtual ICollection<AccountLinkPermission> AccountLinkPermissions { get; set; }
    }
}
