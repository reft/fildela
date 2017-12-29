using Fildela.Data.Helpers;
using System;

namespace Fildela.Business.Domains.Account.Models
{
    public class AccountLinkVerificationModel
    {
        public AccountLinkVerificationModel()
        {
            this.PartitionKey = "accountlinkverifications";
        }

        public AccountLinkVerificationModel(string rowKey, string guestEmail, string key, string firstName, string lastName,
    string password, bool agreeUserAgreement, bool guestExist, int userID,
    string userEmail, string permissions, DateTime dateStartAccountLink, DateTime dateExpiresAccountLink)
        {
            this.PartitionKey = "accountlinkverifications";
            this.RowKey = rowKey;
            this.GuestEmail = guestEmail;
            this.Key = key;
            this.GuestExist = guestExist;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateCreated = DataTimeZoneExtensions.GetCurrentDate();
            this.DateExpiresVerification = DataTimeZoneExtensions.GetCurrentDate().AddHours(6);
            this.Password = password;
            this.IsDeleted = IsDeleted;
            this.AgreeUserAgreement = agreeUserAgreement;

            this.UserID = userID;
            this.UserEmail = userEmail;
            this.Permissions = permissions;
            this.DateStartAccountLink = dateStartAccountLink;
            this.DateExpiresAccountLink = dateExpiresAccountLink;
        }

        public string PartitionKey { get; set; }

        public bool GuestExist { get; set; }

        public string ETag { get; set; }

        public string GuestEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateExpiresVerification { get; set; }

        public string Password { get; set; }

        public string Key { get; set; }

        public bool IsDeleted { get; set; }

        public bool AgreeUserAgreement { get; set; }

        public int UserID { get; set; }

        public string UserEmail { get; set; }

        public string Permissions { get; set; }

        public DateTime DateStartAccountLink { get; set; }

        public DateTime DateExpiresAccountLink { get; set; }

        public string RowKey { get; set; }
    }
}
