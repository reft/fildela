using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Fildela.Data.Storage.Models
{
    public class ResetPassword : TableEntity
    {
        public string Key { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateExpires { get; set; }
        public bool IsUser { get; set; }
    }
}
