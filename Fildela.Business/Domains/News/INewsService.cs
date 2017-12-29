using Fildela.Business.Domains.News.Models;
using System.Drawing;

namespace Fildela.Business.Domains.News
{
    public interface INewsService
    {
        NewsDTOModel GetLatestNewsFromCacheOrDB();

        NewsPagedListDTOModel GetNews(int pageSize, int? newsID, int? pageNo, string newsTitle, int? categoryID, int? orderByTitle, int? orderByPublisher, int? orderByDate, int? orderByCategory);

        string UploadNewsImage(string currentUserEmail, string title, Bitmap image);

        string UploadNewsText(string currentUserEmail, string title, string text);

        void InsertNews(NewsModel news);

        string GetNewsBlob(string blobName);
    }
}
