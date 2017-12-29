using Fildela.Resources;
using Fildela.Business.Helpers;
using Fildela.Data.Repositories.UploadDirectly;
using System;
using System.Configuration;
using System.Text;

namespace Fildela.Business.Domains.UploadDirectly
{
    public class UploadDirectlyService : IUploadDirectlyService
    {
        private readonly string BaseAddress = ConfigurationManager.AppSettings["BaseAddress"];
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];

        private readonly IUploadDirectlyRepository _uploadDirectlyRepository;

        public UploadDirectlyService(IUploadDirectlyRepository uploadDirectlyRepository)
        {
            _uploadDirectlyRepository = uploadDirectlyRepository;
        }

        public string GetUploadSASURI(string blobname)
        {
            return _uploadDirectlyRepository.GetUploadSASURI(blobname);
        }

        public string GetDownloadSASURI(string blobname, string filename, int ExpiredInHours)
        {
            return _uploadDirectlyRepository.GetDownloadSASURI(blobname, filename, ExpiredInHours);
        }

        public Models.UploadDirectlyUsageDTOModel GetUploadDirectlyUsage(string ipAddress)
        {
            return _uploadDirectlyRepository.GetUploadDirectlyUsage(ipAddress).ToModel();
        }

        public Models.UploadDirectlyDTOModel GetBlobAttributes(string blobName)
        {
            return _uploadDirectlyRepository.GetBlobAttributes(blobName).ToModel();
        }

        public void DeleteBlob(string blobName)
        {
            _uploadDirectlyRepository.DeleteBlob(blobName);
        }

        public bool FileOwnedByAnyone(string blobName)
        {
            return _uploadDirectlyRepository.FileOwnedByAnyone(blobName);
        }

        public void InsertUploadDirectly(Models.UploadDirectlyModel uploadDirectly)
        {
            _uploadDirectlyRepository.InsertUploadDirectly(uploadDirectly.ToEntity());
        }

        public bool BlobExist(string blobName)
        {
            return _uploadDirectlyRepository.BlobExist(blobName);
        }

        public int GetSentEmailCountForFile(string blobName, string ipAddress)
        {
            return _uploadDirectlyRepository.GetSentEmailCountForFile(blobName, ipAddress);
        }

        public Models.UploadDirectlyEmailDTOModel GetUploadDirectlyEmailModel(string blobName, string ipAddress)
        {
            return _uploadDirectlyRepository.GetUploadDirectlyEmailModel(blobName, ipAddress).ToModel();
        }

        public bool DownloadLinkSentToEmail(string email, string blobName)
        {
            return _uploadDirectlyRepository.DownloadLinkSentToEmail(email, blobName);
        }

        public Models.UploadDirectlyModel GetUploadDirectlyFromRowKey(string rowKey)
        {
            return _uploadDirectlyRepository.GetUploadDirectlyFromRowKey(rowKey).ToModel();
        }

        public void SendEmailDownloadLink(string email, string fileName, long fileSizeBytes, string URI, DateTime dateStart, DateTime dateEnd, string fromEmail)
        {
            StringBuilder sbEmailBody = new StringBuilder();

            if (String.IsNullOrEmpty(fromEmail))
                sbEmailBody.Append(Resource.You_have_been_sent_a_download_link_from_an_anonymous_user + "<br/><br/>");
            else
                sbEmailBody.Append(Resource.You_have_received_a_download_link_from + " <b>" + BusinessStringExtensions.FirstCharToUpper(fromEmail) + "</b>.<br/><br/>");

            sbEmailBody.Append("<b>" + Resource.File_name + ":<br/></b> " + fileName + "<br/>");
            sbEmailBody.Append("<b>" + Resource.Size + ":<br/></b> " + BusinessUnitConverterExtensions.ConvertByteToASuitableUnit(fileSizeBytes) + "<br/>");
            sbEmailBody.Append("<b>" + Resource.Start_date + ":<br/></b> " + dateStart.ToString("yyyy-MM-dd HH:mm") + "<br/>");
            sbEmailBody.Append("<b>" + Resource.End_date + ":<br/></b> " + dateEnd.ToString("yyyy-MM-dd HH:mm") + "<br/><br/>");

            sbEmailBody.Append(Resource.Click_on_the_link_below_to_start_your_download + "<br/>");

            sbEmailBody.Append("<a href=\"" + URI + "\"" +
            " style=\"text-decoration:none;\">" + URI + "</a>");

            sbEmailBody.Append("<br/><br/><a href=\"" + BaseAddress + "\"" +
    " style=\"text-decoration:none;\">" + ProductNameWithDomain + "</a>");

            string subject = ProductNameWithDomain + " - " + Resource.Download_link_received;

            BusinessSMTPExtensions.SendEmail(subject, email.ToLower().Trim(), sbEmailBody.ToString(), false);
        }
    }
}
