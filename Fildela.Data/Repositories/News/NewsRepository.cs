using Fildela.Data.CustomModels.News;
using Fildela.Data.Database.DataLayer;
using Fildela.Data.Database.Models;
using Fildela.Data.Helpers;
using Fildela.Data.Storage.Services;
using Microsoft.ApplicationServer.Caching;
using Microsoft.WindowsAzure.Storage.Blob;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Fildela.Data.Repositories.News
{
    public class NewsRepository : RepositoryBase, INewsRepository
    {
        public NewsRepository(DataLayer db, CloudStorageServices storage) : base(db, storage) { }

        public NewsDTO GetLatestNewsFromCacheOrDB()
        {
            DataCache cache = new DataCache("default");
            object cacheLatestNews = cache.Get("cacheLatestNews");

            NewsDTO newsDTO = new NewsDTO();

            if (cacheLatestNews == null)
            {
                Fildela.Data.Database.Models.News news = (from n in DB.News.Include("Category")
                                                          orderby n.DatePublished descending
                                                          select n).FirstOrDefault();

                if (news != null)
                {
                    Fildela.Data.Database.Models.Category category = new Fildela.Data.Database.Models.Category()
                    {
                        CategoryID = news.Category.CategoryID,
                        CategoryTypeID = news.Category.CategoryTypeID,
                        Name = news.Category.Name
                    };

                    newsDTO.TextBlobName = news.TextBlobName;
                    newsDTO.ImageBlobURL = news.ImageBlobURL;
                    newsDTO.DatePublished = news.DatePublished;
                    newsDTO.NewsID = news.NewsID;
                    newsDTO.PreviewText = news.PreviewText;
                    newsDTO.PublishedByEmail = news.PublishedByEmail;
                    newsDTO.PublishedByFullName = news.PublishedByFullName;
                    newsDTO.PublishedByID = news.PublishedByID;
                    newsDTO.Title = news.Title;
                    newsDTO.Category = category;
                    newsDTO.CategoryToString = category.Name;

                    cache.Remove("cacheLatestNews");
                    cache.Add("cacheLatestNews", newsDTO);
                }
                else
                    return null;
            }
            else
                newsDTO = (NewsDTO)cacheLatestNews;

            return newsDTO;
        }

        public NewsPagedListDTO GetNews(int pageSize, int? newsID, int? pageNo, string newsTitle, int? categoryID, int? orderByTitle, int? orderByPublisher, int? orderByDate, int? orderByCategory)
        {
            IQueryable<Database.Models.News> news = DB.News.Include("Category").OrderByDescending(d => d.DatePublished).AsQueryable();

            if (newsID != null)
                news = news.Where(n => n.NewsID == newsID);

            if (categoryID != null)
                news = news.Where(n => n.Category.CategoryID == categoryID);

            if (!String.IsNullOrEmpty(newsTitle))
                news = news.Where(n => n.Title.Contains(newsTitle));

            if (orderByTitle != null)
                if (orderByTitle == 0)
                    news = news.OrderBy(n => n.Title);
                else
                    news = news.OrderByDescending(n => n.Title);

            if (orderByPublisher != null)
                if (orderByPublisher == 0)
                    news = news.OrderBy(n => n.PublishedByFullName);
                else
                    news = news.OrderByDescending(n => n.PublishedByFullName);

            if (orderByDate != null)
                if (orderByDate == 0)
                    news = news.OrderByDescending(n => n.DatePublished);
                else
                    news = news.OrderBy(n => n.DatePublished);

            if (orderByCategory != null)
                if (orderByCategory == 0)
                    news = news.OrderByDescending(n => n.Category.Name);
                else
                    news = news.OrderBy(n => n.Category.Name);

            PagedList<Database.Models.News> newsPagedList = (PagedList<Database.Models.News>)news.ToPagedList(pageNo ?? 1, pageSize);
            List<NewsDTO> newsDTO = new List<NewsDTO>();

            foreach (var item in newsPagedList)
            {
                NewsDTO newsDTOtemp = new NewsDTO();

                newsDTOtemp.TextBlobName = item.TextBlobName;
                newsDTOtemp.ImageBlobURL = item.ImageBlobURL;
                newsDTOtemp.DatePublished = item.DatePublished;
                newsDTOtemp.NewsID = item.NewsID;
                newsDTOtemp.PreviewText = item.PreviewText;
                newsDTOtemp.PublishedByEmail = item.PublishedByEmail;
                newsDTOtemp.PublishedByFullName = item.PublishedByFullName;
                newsDTOtemp.PublishedByID = item.PublishedByID;
                newsDTOtemp.Title = item.Title;
                newsDTOtemp.CategoryID = item.Category.CategoryID;
                newsDTOtemp.CategoryToString = item.Category.Name;
                newsDTO.Add(newsDTOtemp);
            }

            //Pagination
            NewsPagedListDTO newsResultDTO = new NewsPagedListDTO();

            var newsPagedListDTO = new StaticPagedList<NewsDTO>(newsDTO, pageNo ?? 1, pageSize, newsPagedList.TotalItemCount);
            newsResultDTO.NewsDTO = newsPagedListDTO;

            return newsResultDTO;
        }

        public string UploadNewsImage(string currentUserEmail, string title, Bitmap image)
        {
            string guid = currentUserEmail + "-" + title + "-image-" + DataGuidExtensions.DateAndGuid();

            CloudBlobContainer container = Storage.GetCloudBlobNewsContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(guid);

            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;
            blob.UploadFromStream(memoryStream);

            return blob.Uri.ToString();
        }

        public string UploadNewsText(string currentUserEmail, string title, string text)
        {
            string guid = currentUserEmail + "-" + title + "-text-" + DataGuidExtensions.DateAndGuid();

            CloudBlobContainer container = Storage.GetCloudBlobNewsContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(guid);

            var bytesToUpload = Encoding.UTF8.GetBytes(text.Trim());

            using (var ms = new MemoryStream(bytesToUpload))
            {
                blob.UploadFromStream(ms);
            }

            return guid;
        }

        public void InsertNews(Database.Models.News news)
        {
            Fildela.Data.Database.Models.Category category = DB.Category.Where(c => c.CategoryID == news.CategoryID).SingleOrDefault();
            var user = DB.User.Where(u => u.UserID == news.PublishedByID && u is Database.Models.User).SingleOrDefault();

            if (user != null && category != null)
            {
                Fildela.Data.Database.Models.News newNews = new Database.Models.News()
                {
                    TextBlobName = news.TextBlobName,
                    ImageBlobURL = news.ImageBlobURL,
                    Title = news.Title,
                    PublishedByID = news.PublishedByID,
                    PublishedByFullName = news.PublishedByFullName,
                    PublishedByEmail = news.PublishedByEmail,
                    DatePublished = news.DatePublished,
                    Category = category,
                    PreviewText = news.PreviewText
                };

                DB.News.Add(newNews);
                DB.SaveChanges();
            }
        }

        public string GetNewsBlob(string blobName)
        {
            string result = string.Empty;

            CloudBlobContainer container = Storage.GetCloudBlobNewsContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            using (var ms = new MemoryStream())
            {
                blob.DownloadToStream(ms);

                result = Encoding.UTF8.GetString(ms.ToArray());
            }

            return result;
        }
    }
}
