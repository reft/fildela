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
    public class AdminJobLinks : IJob
    {
        CloudStorageServices CloudStorageService = new CloudStorageServices();

        public void Execute(IJobExecutionContext context)
        {
            DateTime currentTime = WorkerTimeZoneExtensions.GetCurrentDate();

            CloudTable linkTable = CloudStorageService.GetCloudLinkTable();
            TableQuery<Link> linkQuery = linkTable.CreateQuery<Link>();

            List<Link> linkEntities = (from r in linkTable.ExecuteQuery(linkQuery)
                                       where r.PartitionKey == "links" &&
                                       currentTime > r.DateExpires
                                       select r).ToList();

            int affectedRows = 0;
            int errorCount = 0;

            if (linkEntities != null && linkEntities.Count() > 0)
            {
                int deleteCount = 0;

                while (deleteCount < linkEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<Link> linkEntities100 = linkEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (Link item in linkEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                    {
                        try
                        {
                            linkTable.ExecuteBatch(batchOperation);

                            affectedRows += linkEntities100.Count();
                        }
                        catch
                        {
                            errorCount += linkEntities100.Count();
                        }
                    }

                    deleteCount += 100;
                }
            }

            //Insert adminjob entity
            string rowKey = "links-" + currentTime.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString();
            AdminJob adminJobEntity = new AdminJob(rowKey, "Storage.Links", affectedRows, errorCount, currentTime, WorkerTimeZoneExtensions.GetCurrentDate());

            CloudTable adminJobTable = CloudStorageService.GetCloudAdminJobTable();
            TableOperation insertOperation = TableOperation.InsertOrReplace(adminJobEntity);
            adminJobTable.Execute(insertOperation);
        }
    }
}
