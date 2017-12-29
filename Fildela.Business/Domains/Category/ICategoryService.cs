using Fildela.Business.Domains.Category.Models;
using System.Collections.Generic;

namespace Fildela.Business.Domains.Category
{
    public interface ICategoryService
    {
        string GetContactCategoryName(int categoryID);

        IEnumerable<CategoryModel> GetContactCategoriesFromCacheOrDB();

        IEnumerable<CategoryModel> GetNewsCategoriesFromCacheOrDB();

        CategoryModel GetNewsCategory(int categoryID);
    }
}
