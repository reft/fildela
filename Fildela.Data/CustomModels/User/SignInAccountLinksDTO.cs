using Fildela.Data.Database.Models;
using System.Collections.Generic;

namespace Fildela.Data.CustomModels.User
{
    public class SignInAccountLinksDTO
    {
        public bool IsDeleted { get; set; }

        public IEnumerable<Database.Models.User> Users { get; set; }
    }
}
