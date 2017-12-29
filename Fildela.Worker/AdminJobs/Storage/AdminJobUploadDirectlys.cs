using Fildela.Data.Storage.Models;
using Fildela.Data.Storage.Services;
using Fildela.Worker.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fildela.Worker.AdminJobs.Storage
{
    public class AdminJobUploadDirectlys : IJob
    {
        CloudStorageServices CloudStorageService = new CloudStorageServices();

        public void Execute(IJobExecutionContext context)
        {
            DateTime currentTime = WorkerTimeZoneExtensions.GetCurrentDate();

            CloudTable uploadDirectlyTable = CloudStorageService.GetCloudUploadDirectlyTable();
            TableQuery<UploadDirectly> uploadDirectlyQuery = uploadDirectlyTable.CreateQuery<UploadDirectly>();

            List<UploadDirectly> uploadDirectlyEntities = (from r in uploadDirectlyTable.ExecuteQuery(uploadDirectlyQuery)
                                                           where r.PartitionKey == "uploaddirectlys" &&
                                                           currentTime > r.DateExpires
                                                           select r).ToList();

            int affectedRows = 0;
            int errorCount = 0;

            if (uploadDirectlyEntities != null && uploadDirectlyEntities.Count() > 0)
            {
                int deleteCount = 0;

                while (deleteCount < uploadDirectlyEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<UploadDirectly> uploadDirectlyEntities100 = uploadDirectlyEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (UploadDirectly item in uploadDirectlyEntities100)
                    {
                        //Add row to batch
                        batchOperation.Delete(item);

                        //Delete blob
                        CloudBlobContainer blobContainer = CloudStorageService.GetCloudBlobUploadDirectlyContainer();
                        CloudBlockBlob blob = blobContainer.GetBlockBlobReference(item.BlobName);
                        if (blob.Exists())
                            blob.Delete();
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                    {
                        try
                        {
                            uploadDirectlyTable.ExecuteBatch(batchOperation);

                            affectedRows += uploadDirectlyEntities100.Count();
                        }
                        catch
                        {
                            errorCount += uploadDirectlyEntities100.Count();
                        }
                    }

                    deleteCount += 100;
                }
            }


            //Insert adminjob entity
            string rowKey = "uploaddirectlys-" + currentTime.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString();
            AdminJob adminJobEntity = new AdminJob(rowKey, "Storage.UploadDirectlys", affectedRows, errorCount, currentTime, WorkerTimeZoneExtensions.GetCurrentDate());

            CloudTable adminJobTable = CloudStorageService.GetCloudAdminJobTable();
            TableOperation insertOperation = TableOperation.InsertOrReplace(adminJobEntity);
            adminJobTable.Execute(insertOperation);
        }
    }
}
