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
    public class AdminJobRegisterVerifications : IJob
    {
        CloudStorageServices CloudStorageService = new CloudStorageServices();

        public void Execute(IJobExecutionContext context)
        {
            DateTime currentTime = WorkerTimeZoneExtensions.GetCurrentDate();

            CloudTable registerVerificationTable = CloudStorageService.GetCloudRegisterVerificationTable();
            TableQuery<RegisterVerification> registerVerificationQuery = registerVerificationTable.CreateQuery<RegisterVerification>();

            List<RegisterVerification> registerVerificationEntities = (from r in registerVerificationTable.ExecuteQuery(registerVerificationQuery)
                                                                       where r.PartitionKey == "registerverifications" &&
                                                                       currentTime > r.DateExpires
                                                                       select r).ToList();

            int affectedRows = 0;
            int errorCount = 0;

            if (registerVerificationEntities != null && registerVerificationEntities.Count() > 0)
            {
                int deleteCount = 0;

                while (deleteCount < registerVerificationEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<RegisterVerification> registerVerificationEntities100 = registerVerificationEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (RegisterVerification item in registerVerificationEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                    {
                        try
                        {
                            registerVerificationTable.ExecuteBatch(batchOperation);

                            affectedRows += registerVerificationEntities100.Count();
                        }
                        catch
                        {
                            errorCount += registerVerificationEntities100.Count();
                        }
                    }

                    deleteCount += 100;
                }
            }

            //Insert adminjob entity
            string rowKey = "registerverification-" + currentTime.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString();
            AdminJob adminJobEntity = new AdminJob(rowKey, "Storage.RegisterVerifications", affectedRows, errorCount, currentTime, WorkerTimeZoneExtensions.GetCurrentDate());

            CloudTable adminJobTable = CloudStorageService.GetCloudAdminJobTable();
            TableOperation insertOperation = TableOperation.InsertOrReplace(adminJobEntity);
            adminJobTable.Execute(insertOperation);
        }
    }
}
