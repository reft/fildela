using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Fildela.Data.Storage.Models
{
    public class Usage : TableEntity
    {
        public string UserEmail { get; set; }
        public string Type { get; set; }
        public DateTime DateTriggered { get; set; }
        public DateTime DateExpires { get; set; }
    }
}
