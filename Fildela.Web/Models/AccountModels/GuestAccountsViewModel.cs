using Fildela.Business.Domains.Account.Models;
using Fildela.Business.Domains.User.Models;
using System.Collections.Generic;

namespace Fildela.Web.Models.AccountModels
{
    public class GuestAccountsViewModel : AccountViewModel
    {
        public List<GuestDTOModel> GuestAccounts { get; set; }

        public List<AccountLinkVerificationModel> PendingGuestAccounts { get; set; }

        public int AllowedGuestAccountCount { get; set; }
    }
}