using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Fildela.Data.Storage.Models
{
    public class AccountLinkVerification : TableEntity
    {
        public bool GuestExist { get; set; }
        public string GuestEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateExpiresVerification { get; set; }
        public string Password { get; set; }
        public string Key { get; set; }
        public bool IsDeleted { get; set; }
        public bool AgreeUserAgreement { get; set; }
        public int AccountOwnerID { get; set; }
        public string AccountOwnerEmail { get; set; }
        public string Permissions { get; set; }
        public DateTime DateStartAccountLink { get; set; }
        public DateTime DateExpiresAccountLink { get; set; }
    }
}
