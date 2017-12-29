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
    public class AdminJobAdminLogs : IJob
    {
        CloudStorageServices CloudStorageService = new CloudStorageServices();

        public void Execute(IJobExecutionContext context)
        {
            DateTime currentTime = WorkerTimeZoneExtensions.GetCurrentDate();

            CloudTable adminLogTable = CloudStorageService.GetCloudAdminLogTable();
            TableQuery<AdminLog> adminLogQuery = adminLogTable.CreateQuery<AdminLog>();

            List<AdminLog> adminLogEntities = (from r in adminLogTable.ExecuteQuery(adminLogQuery)
                                               where r.PartitionKey == "adminlogs" &&
                                               currentTime > r.DateExpires
                                               select r).ToList();

            int affectedRows = 0;
            int errorCount = 0;

            if (adminLogEntities != null && adminLogEntities.Count() > 0)
            {
                int deleteCount = 0;

                while (deleteCount < adminLogEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<AdminLog> adminLogEntities100 = adminLogEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (AdminLog item in adminLogEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                    {
                        try
                        {
                            adminLogTable.ExecuteBatch(batchOperation);

                            affectedRows += adminLogEntities100.Count();
                        }
                        catch
                        {
                            errorCount += adminLogEntities100.Count();
                        }
                    }

                    deleteCount += 100;
                }
            }

            //Insert adminjob entity
            string rowKey = "adminlogs-" + currentTime.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString();
            AdminJob adminJobEntity = new AdminJob(rowKey, "Storage.AdminLogs", affectedRows, errorCount, currentTime, WorkerTimeZoneExtensions.GetCurrentDate());

            CloudTable adminJobTable = CloudStorageService.GetCloudAdminJobTable();
            TableOperation insertOperation = TableOperation.InsertOrReplace(adminJobEntity);
            adminJobTable.Execute(insertOperation);
        }
    }
}
