using System;
using System.Collections.Generic;

namespace Fildela.Business.Domains.User.Models
{
    public class GuestDTOModel
    {
        public int UserID { get; set; }

        public string Email { get; set; }

        public DateTime DateRegistered { get; set; }

        public List<String> PermissionNames { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateExpires { get; set; }
    }
}
