using Fildela.Business.Domains.User.Models;
using System.Collections.Generic;

namespace Fildela.Web.Models.AdministrationModels
{
    public class AdministrationAccountsViewModel : AdministrationViewModel
    {
        public List<AccountDTOModel> Accounts { get; set; }
    }
}