using Fildela.Data.CustomModels.User;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.Administration
{
    [DataContract]
    public class AdministrationAccountsDTO : AdministrationCountDTO
    {
        [DataMember]
        public IEnumerable<AccountDTO> Accounts { get; set; }
    }
}
