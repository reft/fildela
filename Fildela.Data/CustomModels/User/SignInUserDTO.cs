using System;
using System.Collections.Generic;

namespace Fildela.Data.CustomModels.User
{
    public class SignInUserDTO
    {
        public bool IsDeleted { get; set; }

        public int UserID { get; set; }

        public IEnumerable<String> UserRoles { get; set; }
    }
}
