using Fildela.Business.Domains.News.Models;
using Fildela.Data.Repositories.News;
using System.Drawing;

namespace Fildela.Business.Domains.News
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;

        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public Models.NewsDTOModel GetLatestNewsFromCacheOrDB()
        {
            return _newsRepository.GetLatestNewsFromCacheOrDB().ToModel();
        }

        public Models.NewsPagedListDTOModel GetNews(int pageSize, int? newsID, int? pageNo, string newsTitle, int? categoryID, int? orderByTitle, int? orderByPublisher, int? orderByDate, int? orderByCategory)
        {
            return _newsRepository.GetNews(pageSize, newsID, pageNo, newsTitle, categoryID, orderByTitle, orderByPublisher, orderByDate, orderByCategory).ToModel();
        }

        public string UploadNewsImage(string currentUserEmail, string title, Bitmap image)
        {
            return _newsRepository.UploadNewsImage(currentUserEmail, title, image);
        }

        public string UploadNewsText(string currentUserEmail, string title, string text)
        {
            return _newsRepository.UploadNewsText(currentUserEmail, title, text);
        }

        public void InsertNews(NewsModel news)
        {
            _newsRepository.InsertNews(news.ToEntity());
        }

        public string GetNewsBlob(string blobName)
        {
            return _newsRepository.GetNewsBlob(blobName);
        }
    }
}
