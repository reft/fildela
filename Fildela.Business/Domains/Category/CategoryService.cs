using Fildela.Data.Repositories.Category;
using System.Collections.Generic;
using System.Linq;

namespace Fildela.Business.Domains.Category
{
    public class CategoryService : ICategoryService
    {
        public readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public string GetContactCategoryName(int categoryID)
        {
            return _categoryRepository.GetContactCategoryName(categoryID);
        }

        public IEnumerable<Models.CategoryModel> GetContactCategoriesFromCacheOrDB()
        {
            return _categoryRepository.GetContactCategoriesFromCacheOrDB().Select(o => o.ToModel());
        }

        public IEnumerable<Models.CategoryModel> GetNewsCategoriesFromCacheOrDB()
        {
            return _categoryRepository.GetNewsCategoriesFromCacheOrDB().Select(o => o.ToModel());
        }

        public Models.CategoryModel GetNewsCategory(int categoryID)
        {
            return _categoryRepository.GetNewsCategory(categoryID).ToModel();
        }
    }
}
