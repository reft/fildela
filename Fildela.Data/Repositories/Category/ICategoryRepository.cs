using System.Collections.Generic;

namespace Fildela.Data.Repositories.Category
{
    public interface ICategoryRepository
    {
        string GetContactCategoryName(int ID);

        IEnumerable<Fildela.Data.Database.Models.Category> GetContactCategoriesFromCacheOrDB();

        IEnumerable<Fildela.Data.Database.Models.Category> GetNewsCategoriesFromCacheOrDB();

        Fildela.Data.Database.Models.Category GetNewsCategory(int categoryID);
    }
}
