using PagedList;

namespace Fildela.Business.Domains.News.Models
{
    public class NewsPagedListDTOModel
    {
        public StaticPagedList<NewsDTOModel> NewsDTOModel { get; set; }
    }
}
