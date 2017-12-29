using Fildela.Data.CustomModels.News;
using System.Drawing;

namespace Fildela.Data.Repositories.News
{
    public interface INewsRepository
    {
        NewsDTO GetLatestNewsFromCacheOrDB();

        NewsPagedListDTO GetNews(int pageSize, int? newsID, int? pageNo, string newsTitle, int? categoryID, int? orderByTitle, int? orderByPublisher, int? orderByDate, int? orderByCategory);

        string UploadNewsImage(string currentUserEmail, string title, Bitmap image);

        string UploadNewsText(string currentUserEmail, string title, string text);

        void InsertNews(Fildela.Data.Database.Models.News news);

        string GetNewsBlob(string blobName);
    }
}
