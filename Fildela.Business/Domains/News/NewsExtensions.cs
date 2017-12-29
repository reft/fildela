using Fildela.Business.Domains.Category;
using Fildela.Business.Domains.News.Models;
using Fildela.Data.CustomModels.News;
using PagedList;
using System.Linq;

namespace Fildela.Business.Domains.News
{
    public static class NewsExtensions
    {
        public static NewsDTO ToEntity(this NewsDTOModel model)
        {
            if (model == null) return null;

            return new NewsDTO()
            {
                CategoryID = model.CategoryID,
                CategoryToString = model.CategoryToString,
                DatePublished = model.DatePublished,
                ImageBlobURL = model.ImageBlobURL,
                NewsID = model.NewsID,
                PreviewText = model.PreviewText,
                PublishedByEmail = model.PublishedByEmail,
                PublishedByFullName = model.PublishedByFullName,
                PublishedByID = model.PublishedByID,
                Title = model.Title,
                TextBlobName = model.TextBlobName,
                Category = CategoryExtensions.ToEntity(model.Category)
            };
        }

        public static NewsDTOModel ToModel(this NewsDTO entity)
        {
            if (entity == null) return null;

            return new NewsDTOModel()
            {
                CategoryID = entity.CategoryID,
                CategoryToString = entity.CategoryToString,
                DatePublished = entity.DatePublished,
                ImageBlobURL = entity.ImageBlobURL,
                NewsID = entity.NewsID,
                PreviewText = entity.PreviewText,
                PublishedByEmail = entity.PublishedByEmail,
                PublishedByFullName = entity.PublishedByFullName,
                PublishedByID = entity.PublishedByID,
                Title = entity.Title,
                TextBlobName = entity.TextBlobName,
                Category = CategoryExtensions.ToModel(entity.Category)
            };
        }

        public static Fildela.Data.Database.Models.News ToEntity(this NewsModel model)
        {
            if (model == null) return null;

            return new Data.Database.Models.News()
            {
                CategoryID = model.CategoryID,
                DatePublished = model.DatePublished,
                NewsID = model.NewsID,
                ImageBlobURL = model.ImageBlobURL,
                PreviewText = model.PreviewText,
                Title = model.Title,
                PublishedByID = model.PublishedByID,
                PublishedByEmail = model.PublishedByEmail,
                TextBlobName = model.TextBlobName,
                PublishedByFullName = model.PublishedByFullName
            };
        }

        public static NewsModel ToModel(this Fildela.Data.Database.Models.News entity)
        {
            if (entity == null) return null;

            return new NewsModel()
            {
                CategoryID = entity.CategoryID,
                DatePublished = entity.DatePublished,
                NewsID = entity.NewsID,
                ImageBlobURL = entity.ImageBlobURL,
                PreviewText = entity.PreviewText,
                Title = entity.Title,
                PublishedByID = entity.PublishedByID,
                PublishedByEmail = entity.PublishedByEmail,
                TextBlobName = entity.TextBlobName,
                PublishedByFullName = entity.PublishedByFullName
            };
        }

        public static NewsPagedListDTOModel ToModel(this NewsPagedListDTO entity)
        {
            if (entity == null) return null;

            var newsDTOModel = new StaticPagedList<NewsDTOModel>(entity.NewsDTO.Select(o => o.ToModel()), entity.NewsDTO.PageNumber, entity.NewsDTO.PageSize, entity.NewsDTO.TotalItemCount);

            return new NewsPagedListDTOModel()
            {
                NewsDTOModel = newsDTOModel
            };
        }
    }
}
