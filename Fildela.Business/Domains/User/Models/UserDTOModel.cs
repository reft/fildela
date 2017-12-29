using System;

namespace Fildela.Business.Domains.User.Models
{
    public class UserDTOModel
    {
        public int UserID { get; set; }

        public string Email { get; set; }

        public string UserRolesToString { get; set; }

        public bool IsPremium { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime DateLastActive { get; set; }
    }
}
