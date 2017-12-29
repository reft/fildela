using Fildela.Business.Domains.Category.Models;
using System;

namespace Fildela.Business.Domains.News.Models
{
    public class NewsDTOModel
    {
        public int NewsID { get; set; }

        public int PublishedByID { get; set; }

        public string PublishedByFullName { get; set; }

        public string PublishedByEmail { get; set; }

        public DateTime DatePublished { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string PreviewText { get; set; }

        public string TextBlobName { get; set; }

        public string ImageBlobURL { get; set; }

        public string CategoryToString { get; set; }

        public int CategoryID { get; set; }

        public virtual CategoryModel Category { get; set; }
    }
}
