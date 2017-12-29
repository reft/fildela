using Fildela.Data.Helpers;
using System;

namespace Fildela.Business.Domains.Administration.Models
{
    public class AdminJobModel
    {
        public AdminJobModel()
        {
            this.PartitionKey = "adminjobs";
            this.RowKey = DataGuidExtensions.DateAndGuid();
        }

        public AdminJobModel(string rowKey, string type, int affectedRows, int errorCount, DateTime dateStart, DateTime dateEnd)
        {
            this.PartitionKey = "adminjobs";
            this.RowKey = DataGuidExtensions.DateAndGuid();
            this.RowKey = rowKey;
            this.Type = type;
            this.AffectedRows = affectedRows;
            this.ErrorCount = errorCount;
            this.DateStart = dateStart;
            this.DateEnd = dateEnd;
        }

        public string ETag { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public string Type { get; set; }

        public int AffectedRows { get; set; }

        public int ErrorCount { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }
    }
}
