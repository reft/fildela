using System;
using System.Collections.Generic;

namespace Fildela.Data.CustomModels.User
{
    public class SignInGuestDTO
    {
        public bool IsDeleted { get; set; }

        public int GuestID { get; set; }

        public bool IsGuestLinkedWithUser { get; set; }
    }
}
