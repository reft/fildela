using System;

namespace Fildela.Business.Domains.News.Models
{
    public class NewsModel
    {
        public int NewsID { get; set; }

        public int CategoryID { get; set; }

        public int PublishedByID { get; set; }

        public string PublishedByFullName { get; set; }

        public string PublishedByEmail { get; set; }

        public DateTime DatePublished { get; set; }

        public string Title { get; set; }

        public string PreviewText { get; set; }

        public string TextBlobName { get; set; }

        public string ImageBlobURL { get; set; }
    }
}
