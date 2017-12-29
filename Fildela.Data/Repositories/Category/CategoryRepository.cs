using Fildela.Data.Database.DataLayer;
using Fildela.Data.Storage.Services;
using Microsoft.ApplicationServer.Caching;
using System.Collections.Generic;
using System.Linq;

namespace Fildela.Data.Repositories.Category
{
    public class CategoryRepository : RepositoryBase, ICategoryRepository
    {
        public CategoryRepository(DataLayer db, CloudStorageServices storage) : base(db, storage) { }

        public string GetContactCategoryName(int ID)
        {
            string name = string.Empty;

            Fildela.Data.Database.Models.Category category = (from c in DB.Category
                                                              where c.CategoryTypeID == 1 && c.CategoryID == ID
                                                              select c).SingleOrDefault();

            if (category != null)
                name = category.Name;

            return name;
        }

        public IEnumerable<Fildela.Data.Database.Models.Category> GetContactCategoriesFromCacheOrDB()
        {
            List<Fildela.Data.Database.Models.Category> contactCategoriesDTO = new List<Fildela.Data.Database.Models.Category>();

            DataCache cache = new DataCache("default");
            object cacheContactCategories = cache.Get("cacheContactCategories");

            if (cacheContactCategories == null)
            {
                List<Fildela.Data.Database.Models.Category> contactCategories = (from cc in DB.Category
                                                                                 where cc.CategoryTypeID == 1
                                                                                 select cc).ToList();

                foreach (var item in contactCategories)
                {
                    Fildela.Data.Database.Models.Category contactCategoryDTO = new Fildela.Data.Database.Models.Category()
                    {
                        CategoryID = item.CategoryID,
                        Name = item.Name,
                        CategoryTypeID = item.CategoryTypeID
                    };

                    contactCategoriesDTO.Add(contactCategoryDTO);
                }

                cache.Remove("cacheContactCategories");
                cache.Add("cacheContactCategories", contactCategoriesDTO);
            }
            else
                contactCategoriesDTO = (List<Fildela.Data.Database.Models.Category>)cacheContactCategories;

            return contactCategoriesDTO;
        }

        public IEnumerable<Fildela.Data.Database.Models.Category> GetNewsCategoriesFromCacheOrDB()
        {
            List<Fildela.Data.Database.Models.Category> newsCategories = new List<Fildela.Data.Database.Models.Category>();

            DataCache cache = new DataCache("default");
            object cacheNewsCategories = cache.Get("cacheNewsCategories");

            if (cacheNewsCategories == null)
            {
                newsCategories = (from nc in DB.Category
                                  where nc.CategoryTypeID == 2
                                  select nc).ToList();

                cache.Remove("cacheNewsCategories");
                cache.Add("cacheNewsCategories", newsCategories);
            }
            else
                newsCategories = (List<Fildela.Data.Database.Models.Category>)cacheNewsCategories;

            return newsCategories;
        }

        public Fildela.Data.Database.Models.Category GetNewsCategory(int categoryID)
        {
            Fildela.Data.Database.Models.Category newsCategory = (from nc in DB.Category
                                     where nc.CategoryTypeID == 2 && nc.CategoryID == categoryID
                                     select nc).SingleOrDefault();

            return newsCategory;
        }
    }
}
