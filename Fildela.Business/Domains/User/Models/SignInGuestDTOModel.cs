using System;
using System.Collections.Generic;

namespace Fildela.Business.Domains.User.Models
{
    public class SignInGuestDTOModel
    {
        public bool IsDeleted { get; set; }

        public int GuestID { get; set; }

        public bool IsGuestLinkedWithUser { get; set; }
    }
}
