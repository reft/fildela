using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fildela.Business.Domains.User.Models
{
    public class AccountDTOModel
    {
        public int AccountID { get; set; }

        public string Email { get; set; }

        public string UserRolesToString { get; set; }

        public bool IsPremium { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime DateLastActive { get; set; }
    }
}
