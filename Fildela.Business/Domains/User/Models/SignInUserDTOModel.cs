using System;
using System.Collections.Generic;

namespace Fildela.Business.Domains.User.Models
{
    public class SignInUserDTOModel
    {
        public bool IsDeleted { get; set; }

        public int UserID { get; set; }

        public IEnumerable<String> UserRoles { get; set; }
    }
}
