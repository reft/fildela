using Fildela.Data.Database.Models;
using System;
using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.News
{
    [DataContract]
    public class NewsDTO
    {
        [DataMember]
        public int NewsID { get; set; }

        [DataMember]
        public int PublishedByID { get; set; }

        [DataMember]
        public string PublishedByFullName { get; set; }

        [DataMember]
        public string PublishedByEmail { get; set; }

        [DataMember]
        public DateTime DatePublished { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string PreviewText { get; set; }

        [DataMember]
        public string TextBlobName { get; set; }

        [DataMember]
        public string ImageBlobURL { get; set; }

        [DataMember]
        public string CategoryToString { get; set; }

        [DataMember]
        public int CategoryID { get; set; }

        [DataMember]
        public virtual Category Category { get; set; }
    }
}
