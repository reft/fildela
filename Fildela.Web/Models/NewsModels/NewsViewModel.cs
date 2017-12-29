using Fildela.Business.Domains.Category.Models;
using Fildela.Business.Domains.News.Models;
using System.Collections.Generic;

namespace Fildela.Web.Models.NewsModels
{
    public class NewsViewModel
    {
        public NewsPagedListDTOModel NewsPagedListDTO { get; set; }

        public List<CategoryModel> Categories { get; set; }

        public int? NewsID { get; set; }

        public string NewsTitle { get; set; }

        public int? CategoryID { get; set; }

        public int? OrderByTitle { get; set; }

        public int? OrderByPublisher { get; set; }

        public int? OrderByDate { get; set; }

        public int? OrderByCategory { get; set; }
    }
}