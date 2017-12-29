using PagedList;
using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.News
{
    [DataContract]
    public class NewsPagedListDTO
    {
        [DataMember]
        public StaticPagedList<NewsDTO> NewsDTO { get; set; }
    }
}
