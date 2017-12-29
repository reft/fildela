using Fildela.Business.Domains.Category.Models;

namespace Fildela.Business.Domains.Category
{
    public static class CategoryExtensions
    {
        public static Fildela.Data.Database.Models.Category ToEntity(this CategoryModel model)
        {
            if (model == null) return null;

            return new Data.Database.Models.Category()
            {
                CategoryID = model.CategoryID,
                Name = model.Name,
                CategoryTypeID = model.CategoryTypeID
            };
        }

        public static CategoryModel ToModel(this Fildela.Data.Database.Models.Category entity)
        {
            if (entity == null) return null;

            return new CategoryModel()
            {
                CategoryID = entity.CategoryID,
                Name = entity.Name,
                CategoryTypeID = entity.CategoryTypeID
            };
        }
    }
}
