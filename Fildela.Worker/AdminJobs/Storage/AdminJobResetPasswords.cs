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
    public class AdminJobResetPasswords : IJob
    {
        CloudStorageServices CloudStorageService = new CloudStorageServices();

        public void Execute(IJobExecutionContext context)
        {
            DateTime currentTime = WorkerTimeZoneExtensions.GetCurrentDate();

            CloudTable resetPasswordTable = CloudStorageService.GetCloudResetPasswordTable();
            TableQuery<ResetPassword> resetPasswordQuery = resetPasswordTable.CreateQuery<ResetPassword>();

            List<ResetPassword> resetPasswordEntities = (from r in resetPasswordTable.ExecuteQuery(resetPasswordQuery)
                                                         where r.PartitionKey == "resetpasswords" &&
                                                         currentTime > r.DateExpires
                                                         select r).ToList();

            int affectedRows = 0;
            int errorCount = 0;

            if (resetPasswordEntities != null && resetPasswordEntities.Count() > 0)
            {
                int deleteCount = 0;

                while (deleteCount < resetPasswordEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<ResetPassword> resetPasswordEntities100 = resetPasswordEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (ResetPassword item in resetPasswordEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                    {
                        try
                        {
                            resetPasswordTable.ExecuteBatch(batchOperation);

                            affectedRows += resetPasswordEntities100.Count();
                        }
                        catch
                        {
                            errorCount += resetPasswordEntities100.Count();
                        }
                    }

                    deleteCount += 100;
                }
            }

            //Insert adminjob entity
            string rowKey = "resetpasswords-" + currentTime.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString();
            AdminJob adminJobEntity = new AdminJob(rowKey, "Storage.ResetPasswords", affectedRows, errorCount, currentTime, WorkerTimeZoneExtensions.GetCurrentDate());

            CloudTable adminJobTable = CloudStorageService.GetCloudAdminJobTable();
            TableOperation insertOperation = TableOperation.InsertOrReplace(adminJobEntity);
            adminJobTable.Execute(insertOperation);
        }
    }
}
