using Fildela.Data.Storage.Models;
using Fildela.Data.Storage.Services;
using Fildela.Worker.Helpers;
using Microsoft.WindowsAzure.Storage.Table;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fildela.Worker.AdminJobs.Storage
{
    public class AdminJobUsage : IJob
    {
        CloudStorageServices CloudStorageService = new CloudStorageServices();

        public void Execute(IJobExecutionContext context)
        {
            DateTime currentTime = WorkerTimeZoneExtensions.GetCurrentDate();

            CloudTable usageTable = CloudStorageService.GetCloudUsageTable();
            TableQuery<Usage> usageQuery = usageTable.CreateQuery<Usage>();

            List<Usage> usageEntities = (from r in usageTable.ExecuteQuery(usageQuery)
                                         where r.PartitionKey == "usage" &&
                                         currentTime > r.DateExpires
                                         select r).ToList();

            int affectedRows = 0;
            int errorCount = 0;

            if (usageEntities != null && usageEntities.Count() > 0)
            {
                int deleteCount = 0;

                while (deleteCount < usageEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<Usage> usageEntities100 = usageEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (Usage item in usageEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                    {
                        try
                        {
                            usageTable.ExecuteBatch(batchOperation);

                            affectedRows += usageEntities100.Count();
                        }
                        catch
                        {
                            errorCount += usageEntities100.Count();
                        }
                    }

                    deleteCount += 100;
                }
            }

            //Insert adminjob entity
            string rowKey = "usage-" + currentTime.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString();
            AdminJob adminJobEntity = new AdminJob(rowKey, "Storage.Usage", affectedRows, errorCount, currentTime, WorkerTimeZoneExtensions.GetCurrentDate());

            CloudTable adminJobTable = CloudStorageService.GetCloudAdminJobTable();
            TableOperation insertOperation = TableOperation.InsertOrReplace(adminJobEntity);
            adminJobTable.Execute(insertOperation);
        }
    }
}