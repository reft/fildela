
namespace Fildela.Web.Models.NewsModels
{
    public class NewsModel
    {
        public bool? Table { get; set; }

        public string NewsTitle { get; set; }

        public int? Page { get; set; }

        public int? NewsID { get; set; }

        public int? CategoryID { get; set; }

        public int? OrderByTitle { get; set; }

        public int? OrderByPublisher { get; set; }

        public int? OrderByDate { get; set; }

        public int? OrderByCategory { get; set; }
    }
}