using System.Collections.Generic;

namespace Fildela.Business.Domains.User.Models
{
    public class SignInAccountLinksDTOModel
    {
        public bool IsDeleted { get; set; }

        public IEnumerable<UserModel> Users { get; set; }
    }
}
