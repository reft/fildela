using Fildela.Data.Helpers;
using System;

namespace Fildela.Business.Domains.Account.Models
{
    public class LogModel
    {
        public LogModel()
        {
            this.PartitionKey = "logs";
        }

        public LogModel(string rowKey, int accountOwnerID, string accountOwnerEmail, int triggeredByID,
            string triggeredByEmail, string userType, string ipAddress, string type)
        {
            this.PartitionKey = "logs";
            this.RowKey = rowKey;
            this.AccountOwnerID = accountOwnerID;
            this.AccountOwnerEmail = accountOwnerEmail;
            this.TriggeredByID = triggeredByID;
            this.TriggeredByEmail = triggeredByEmail;
            this.DateTriggered = DataTimeZoneExtensions.GetCurrentDate();
            this.DateExpires = DataTimeZoneExtensions.GetCurrentDate().AddDays(30);
            this.IpAddress = ipAddress;
            this.Type = type;
            this.UserType = userType;
        }

        public string ETag { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public int AccountOwnerID { get; set; }

        public string AccountOwnerEmail { get; set; }

        public int TriggeredByID { get; set; }

        public string TriggeredByEmail { get; set; }

        public string UserType { get; set; }

        public DateTime DateTriggered { get; set; }

        public DateTime DateExpires { get; set; }

        public string IpAddress { get; set; }

        public string Type { get; set; }

        public int IsRead { get; set; }

        public int IsDeleted { get; set; }
    }
}
