using Fildela.Data.CustomModels.News;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.Administration
{
    [DataContract]
    public class AdministrationNewsDTO : AdministrationCountDTO
    {
        [DataMember]
        public IEnumerable<NewsDTO> News { get; set; }
    }
}
