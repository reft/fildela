using Fildela.Data.Helpers;
using System.Collections.Generic;

namespace Fildela.Data.Database.Models
{
    public class Guest : Account
    {
        public Guest()
        {
            AccountLinks = new HashSet<AccountLink>();
        }

        public Guest(string firstName, string lastName, string email,
            string password, string passwordSalt, bool agreeUserAgreement, string defaultIpAddress, string defaultEmailAddress)
        {
            AccountLinks = new HashSet<AccountLink>();

            this.FirstName = firstName ?? string.Empty;
            this.LastName = lastName ?? string.Empty;
            this.Email = email;
            this.DateRegistered = DataTimeZoneExtensions.GetCurrentDate();
            this.DateLastActive = DataTimeZoneExtensions.GetCurrentDate();
            this.PasswordHash = password;
            this.PasswordSalt = passwordSalt;
            this.IsDeleted = false;
            this.AgreeUserAgreement = agreeUserAgreement;
            this.DefaultEmailAddress = defaultEmailAddress;
            this.DefaultIpAddress = defaultIpAddress;
        }

        public virtual ICollection<AccountLink> AccountLinks { get; set; }
    }
}
