using Fildela.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fildela.Business.Domains.Account.Models
{
    public class UsageModel
    {
        public UsageModel()
        {
            this.PartitionKey = "usage";
        }

        public UsageModel(string rowKey, string userEmail, string type)
        {
            this.PartitionKey = "usage";
            this.RowKey = rowKey;
            this.UserEmail = userEmail;
            this.Type = type;
            this.DateTriggered = DataTimeZoneExtensions.GetCurrentDate();
            this.DateExpires = DataTimeZoneExtensions.GetCurrentDate().AddDays(30);
        }

        public string ETag { get; set; }

        public string UserEmail { get; set; }

        public string Type { get; set; }

        public DateTime DateTriggered { get; set; }

        public DateTime DateExpires { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }
    }
}
