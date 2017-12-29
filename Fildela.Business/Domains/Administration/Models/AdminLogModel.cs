using Fildela.Data.Helpers;
using System;

namespace Fildela.Business.Domains.Administration.Models
{
    public class AdminLogModel
    {
        public AdminLogModel()
        {
            this.PartitionKey = "adminlogs";
        }

        public AdminLogModel(string rowKey, int accountOwnerID, string accountOwnerEmail,
            int triggeredByID, string triggeredByEmail, string ipAddress, string type)
        {
            this.PartitionKey = "adminlogs";
            this.RowKey = rowKey;
            this.AccountOwnerID = accountOwnerID;
            this.AccountOwnerEmail = accountOwnerEmail;
            this.TriggeredByID = triggeredByID;
            this.TriggeredByEmail = triggeredByEmail;
            this.IpAddress = ipAddress;
            this.Type = type;
            this.DateExpires = DataTimeZoneExtensions.GetCurrentDate().AddMinutes(15);
            this.DateTriggered = DataTimeZoneExtensions.GetCurrentDate();
        }

        public string ETag { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public int AccountOwnerID { get; set; }

        public string AccountOwnerEmail { get; set; }

        public string TriggeredByRole { get; set; }

        public int TriggeredByID { get; set; }

        public string TriggeredByEmail { get; set; }

        public string IpAddress { get; set; }

        public string Type { get; set; }

        public DateTime DateTriggered { get; set; }

        public DateTime DateExpires { get; set; }
    }
}
