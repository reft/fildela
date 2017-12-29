using Fildela.Business.Domains.Category.Models;
using Fildela.Business.Domains.News.Models;
using System.Collections.Generic;

namespace Fildela.Web.Models.AdministrationModels
{
    public class AdministrationNewsViewModel : AdministrationViewModel
    {
        public List<NewsDTOModel> News { get; set; }

        public List<CategoryModel> Categories { get; set; }
    }
}