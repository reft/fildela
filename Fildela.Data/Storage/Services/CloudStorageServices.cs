using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;

namespace Fildela.Data.Storage.Services
{
    public class CloudStorageServices
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["FildelaStorage"].ConnectionString);
        //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

        //Admin log table
        public CloudTable GetCloudAdminLogTable()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("adminlogs");

            table.CreateIfNotExists();

            return table;
        }

        //Admin job table
        public CloudTable GetCloudAdminJobTable()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("adminjobs");

            table.CreateIfNotExists();

            return table;
        }

        //Log table
        public CloudTable GetCloudLogTable()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("logs");

            table.CreateIfNotExists();

            return table;
        }

        //File table
        public CloudTable GetCloudFileTable()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("files");

            table.CreateIfNotExists();

            return table;
        }

        //Link table
        public CloudTable GetCloudLinkTable()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("links");

            table.CreateIfNotExists();

            return table;
        }

        //Usage table
        public CloudTable GetCloudUsageTable()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("usage");

            table.CreateIfNotExists();

            return table;
        }

        //Reset password table
        public CloudTable GetCloudResetPasswordTable()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("resetpasswords");

            table.CreateIfNotExists();

            return table;
        }

        //Register verification account owner table
        public CloudTable GetCloudRegisterVerificationTable()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("registerverifications");

            table.CreateIfNotExists();

            return table;
        }

        //Register verification guest table
        public CloudTable GetCloudAccountLinkVerificationTable()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("accountlinkverifications");

            table.CreateIfNotExists();

            return table;
        }

        //Upload directly table
        public CloudTable GetCloudUploadDirectlyTable()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("uploaddirectlys");

            table.CreateIfNotExists();

            return table;
        }

        //Upload directly blob container
        public CloudBlobContainer GetCloudBlobUploadDirectlyContainer()
        {
            CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer BlobContainer = BlobClient.GetContainerReference("uploaddirectlys");

            BlobContainer.CreateIfNotExists();

            BlobContainerPermissions containerPermissions = new BlobContainerPermissions();

            containerPermissions.PublicAccess = BlobContainerPublicAccessType.Off;

            BlobContainer.SetPermissions(containerPermissions);

            return BlobContainer;
        }

        //File blob container
        public CloudBlobContainer GetCloudBlobFileContainer()
        {
            CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer BlobContainer = BlobClient.GetContainerReference("files");

            BlobContainer.CreateIfNotExists();

            BlobContainerPermissions containerPermissions = new BlobContainerPermissions();

            containerPermissions.PublicAccess = BlobContainerPublicAccessType.Off;

            BlobContainer.SetPermissions(containerPermissions);

            return BlobContainer;
        }

        //Icon blob container
        public CloudBlobContainer GetCloudBlobIconContainer()
        {
            CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer BlobContainer = BlobClient.GetContainerReference("fileicons");

            BlobContainer.CreateIfNotExists();

            BlobContainer.SetPermissions(
                new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

            return BlobContainer;
        }

        // blob container
        public CloudBlobContainer GetCloudBlobNewsContainer()
        {
            CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer BlobContainer = BlobClient.GetContainerReference("news");

            BlobContainer.CreateIfNotExists();

            BlobContainer.SetPermissions(
                new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

            return BlobContainer;
        }
    }
}
