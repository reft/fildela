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
    public class AdminJobLogs : IJob
    {
        CloudStorageServices CloudStorageService = new CloudStorageServices();

        public void Execute(IJobExecutionContext context)
        {
            DateTime currentTime = WorkerTimeZoneExtensions.GetCurrentDate();

            CloudTable logTable = CloudStorageService.GetCloudLogTable();
            TableQuery<Log> logQuery = logTable.CreateQuery<Log>();

            List<Log> logEntities = (from r in logTable.ExecuteQuery(logQuery)
                                     where r.PartitionKey == "logs" &&
                                     currentTime > r.DateExpires
                                     select r).ToList();

            int affectedRows = 0;
            int errorCount = 0;

            if (logEntities != null && logEntities.Count() > 0)
            {
                int deleteCount = 0;

                while (deleteCount < logEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<Log> logEntities100 = logEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (Log item in logEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                    {
                        try
                        {
                            logTable.ExecuteBatch(batchOperation);

                            affectedRows += logEntities100.Count();
                        }
                        catch
                        {
                            errorCount += logEntities100.Count();
                        }
                    }

                    deleteCount += 100;
                }
            }

            //Insert adminjob entity
            string rowKey = "logs-" + currentTime.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString();
            AdminJob adminJobEntity = new AdminJob(rowKey, "Storage.Logs", affectedRows, errorCount, currentTime, WorkerTimeZoneExtensions.GetCurrentDate());

            CloudTable adminJobTable = CloudStorageService.GetCloudAdminJobTable();
            TableOperation insertOperation = TableOperation.InsertOrReplace(adminJobEntity);
            adminJobTable.Execute(insertOperation);
        }
    }
}
