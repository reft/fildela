using Fildela.Data.Helpers;
using System;

namespace Fildela.Business.Domains.User.Models
{
    public class RegisterVerificationModel
    {
        public RegisterVerificationModel()
        {
            this.PartitionKey = "registerverifications";
        }

        public RegisterVerificationModel(string rowKey, string key, string firstName, string lastName,
            string password, string passwordSalt, bool agreeUserAgreement, string defaultIpAddress)
        {
            this.PartitionKey = "registerverifications";
            this.RowKey = rowKey;
            this.Key = key;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateRegistered = DataTimeZoneExtensions.GetCurrentDate();
            this.DateExpires = DataTimeZoneExtensions.GetCurrentDate().AddMinutes(15);
            this.Password = password;
            this.PasswordSalt = passwordSalt;
            this.IsDeleted = IsDeleted;
            this.AgreeUserAgreement = agreeUserAgreement;
            this.DefaultIpAddress = defaultIpAddress;
        }

        public string ETag { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime DateExpires { get; set; }

        public string Password { get; set; }

        public string PasswordSalt { get; set; }

        public string Key { get; set; }

        public string DefaultIpAddress { get; set; }

        public bool IsDeleted { get; set; }

        public bool AgreeUserAgreement { get; set; }
    }
}
