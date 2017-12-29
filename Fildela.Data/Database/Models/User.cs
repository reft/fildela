using System;
using System.Collections.Generic;

namespace Fildela.Data.Database.Models
{
    public class User : Account
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
            News = new HashSet<News>();
            AccountLinks = new HashSet<AccountLink>();
            AccountAuthenticationProvider = new HashSet<AccountAuthenticationProvider>();
        }

        public User(string firstName, string lastName, string email,
            string password, string passwordSalt, bool agreeUserAgreement, string defaultIpAddress, string defaultEmailAddress)
        {
            UserRoles = new HashSet<UserRole>();
            News = new HashSet<News>();
            AccountLinks = new HashSet<AccountLink>();
            AccountAuthenticationProvider = new HashSet<AccountAuthenticationProvider>();

            this.FirstName = firstName ?? string.Empty;
            this.LastName = lastName ?? string.Empty;
            this.Email = email;
            this.DateRegistered = DateTime.UtcNow.AddHours(1);
            this.DateLastActive = DateTime.UtcNow.AddHours(1);
            this.PasswordHash = password;
            this.PasswordSalt = passwordSalt;
            this.IsDeleted = false;
            this.AgreeUserAgreement = agreeUserAgreement;
            this.DefaultEmailAddress = defaultEmailAddress;
            this.DefaultIpAddress = defaultIpAddress;
        }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual AccountUsagePremium AccountUsagePremium { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<AccountLink> AccountLinks { get; set; }
        public virtual ICollection<AccountAuthenticationProvider> AccountAuthenticationProvider { get; set; }
    }
}
