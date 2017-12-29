using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Fildela.Data.Storage.Models
{
    public class RegisterVerification : TableEntity
    {
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
