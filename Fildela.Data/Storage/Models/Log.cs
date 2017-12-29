using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Fildela.Data.Storage.Models
{
    public class Log : TableEntity
    {
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
