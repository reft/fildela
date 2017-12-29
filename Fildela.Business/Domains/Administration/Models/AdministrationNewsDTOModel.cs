using Fildela.Business.Domains.News.Models;
using System.Collections.Generic;

namespace Fildela.Business.Domains.Administration.Models
{
    public class AdministrationNewsDTOModel : AdministrationCountDTOModel
    {
        public IEnumerable<NewsDTOModel> News { get; set; }
    }
}
