using Fildela.Data.CustomModels.UploadDirectly;

namespace Fildela.Data.Repositories.UploadDirectly
{
    public interface IUploadDirectlyRepository
    {
        string GetUploadSASURI(string blobname);

        string GetDownloadSASURI(string blobname, string filename, int ExpiredInHours);

        UploadDirectlyUsageDTO GetUploadDirectlyUsage(string ipAddress);

        UploadDirectlyDTO GetBlobAttributes(string blobName);

        void DeleteBlob(string blobName);

        bool FileOwnedByAnyone(string blobName);

        void InsertUploadDirectly(Fildela.Data.Storage.Models.UploadDirectly uploadDirectly);

        bool BlobExist(string blobName);

        int GetSentEmailCountForFile(string blobName, string ipAddress);

        UploadDirectlyEmailDTO GetUploadDirectlyEmailModel(string blobName, string ipAddress);

        bool DownloadLinkSentToEmail(string email, string blobName);

        Fildela.Data.Storage.Models.UploadDirectly GetUploadDirectlyFromRowKey(string rowKey);
    }
}
