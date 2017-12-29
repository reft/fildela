using Fildela.Data.Helpers;
using System;

namespace Fildela.Business.Domains.User.Models
{
    public class ResetPasswordModel
    {
        public ResetPasswordModel()
        {
            this.PartitionKey = "resetpasswords";
        }

        public ResetPasswordModel(string rowKey, string email, string key, bool isUser)
        {
            this.PartitionKey = "resetpasswords";
            this.RowKey = rowKey;
            this.Email = email;
            this.Key = key;
            this.DateCreated = DataTimeZoneExtensions.GetCurrentDate();
            this.DateExpires = DataTimeZoneExtensions.GetCurrentDate().AddMinutes(15);
            this.IsUser = isUser;
        }

        public string ETag { get; set; }

        public string PartitionKey { get; set; }

        public string Key { get; set; }

        public string Email { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateExpires { get; set; }

        public bool IsUser { get; set; }

        public string RowKey { get; set; }
    }
}
