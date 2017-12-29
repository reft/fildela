using Fildela.Data.Helpers;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Fildela.Data.Storage.Models
{
    public class AdminJob : TableEntity
    {
        public AdminJob()
        {
            this.PartitionKey = "adminjobs";
            this.RowKey = DataGuidExtensions.DateAndGuid();
        }

        public AdminJob(string rowKey, string type, int affectedRows, int errorCount, DateTime dateStart, DateTime dateEnd)
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

        public string Type { get; set; }
        public int AffectedRows { get; set; }
        public int ErrorCount { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
