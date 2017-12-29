using Fildela.Business.Domains.User.Models;
using System.Collections.Generic;

namespace Fildela.Business.Domains.Administration.Models
{
    public class AdministrationAccountsDTOModel : AdministrationCountDTOModel
    {
        public IEnumerable<AccountDTOModel> Accounts { get; set; }
    }
}
