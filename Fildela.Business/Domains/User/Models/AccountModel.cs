using System;

namespace Fildela.Business.Domains.User.Models
{
    public class AccountModel
    {
        public int UserID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime DateLastActive { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public string DefaultIpAddress { get; set; }

        public string DefaultEmailAddress { get; set; }

        public bool IsDeleted { get; set; }

        public bool AgreeUserAgreement { get; set; }
    }
}
