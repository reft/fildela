using Fildela.Business.Domains.UploadDirectly.Models;
using System;

namespace Fildela.Business.Domains.UploadDirectly
{
    public interface IUploadDirectlyService
    {
        string GetUploadSASURI(string blobname);

        string GetDownloadSASURI(string blobname, string filename, int ExpiredInHours);

        UploadDirectlyUsageDTOModel GetUploadDirectlyUsage(string ipAddress);

        UploadDirectlyDTOModel GetBlobAttributes(string blobName);

        void DeleteBlob(string blobName);

        bool FileOwnedByAnyone(string blobName);

        void InsertUploadDirectly(UploadDirectlyModel uploadDirectly);

        bool BlobExist(string blobName);

        int GetSentEmailCountForFile(string blobName, string ipAddress);

        UploadDirectlyEmailDTOModel GetUploadDirectlyEmailModel(string blobName, string ipAddress);

        bool DownloadLinkSentToEmail(string email, string blobName);

        UploadDirectlyModel GetUploadDirectlyFromRowKey(string rowKey);

        void SendEmailDownloadLink(string email, string fileName, long fileSizeBytes, string URI, DateTime dateStart, DateTime dateEnd, string fromEmail);
    }
}
