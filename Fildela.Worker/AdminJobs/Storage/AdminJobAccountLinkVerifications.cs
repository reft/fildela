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
    public class AdminJobAccountLinkVerifications : IJob
    {
        CloudStorageServices CloudStorageService = new CloudStorageServices();

        public void Execute(IJobExecutionContext context)
        {
            DateTime currentTime = WorkerTimeZoneExtensions.GetCurrentDate();

            CloudTable accountLinkVerificationTable = CloudStorageService.GetCloudAccountLinkVerificationTable();
            TableQuery<AccountLinkVerification> accountLinkVerificationQuery = accountLinkVerificationTable.CreateQuery<AccountLinkVerification>();

            List<AccountLinkVerification> accountLinkVerificationEntities = (from r in accountLinkVerificationTable.ExecuteQuery(accountLinkVerificationQuery)
                                                                             where r.PartitionKey == "accountlinkverifications" &&
                                                                             currentTime > r.DateExpiresVerification
                                                                             select r).ToList();

            int affectedRows = 0;
            int errorCount = 0;

            if (accountLinkVerificationEntities != null && accountLinkVerificationEntities.Count() > 0)
            {
                int deleteCount = 0;

                while (deleteCount < accountLinkVerificationEntities.Count())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    List<AccountLinkVerification> accountLinkVerificationEntities100 = accountLinkVerificationEntities.Skip(deleteCount).Take(100).ToList();

                    foreach (AccountLinkVerification item in accountLinkVerificationEntities100)
                    {
                        batchOperation.Delete(item);
                    }

                    //Delete table entities
                    if (batchOperation.Count > 0)
                    {
                        try
                        {
                            accountLinkVerificationTable.ExecuteBatch(batchOperation);

                            affectedRows += accountLinkVerificationEntities100.Count();
                        }
                        catch
                        {
                            errorCount += accountLinkVerificationEntities100.Count();
                        }
                    }

                    deleteCount += 100;
                }
            }

            //Insert adminjob entity
            string rowKey = "accountlinkverifications-" + currentTime.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString();
            AdminJob adminJobEntity = new AdminJob(rowKey, "Storage.AccountLinkVerifications", affectedRows, errorCount, currentTime, WorkerTimeZoneExtensions.GetCurrentDate());

            CloudTable adminJobTable = CloudStorageService.GetCloudAdminJobTable();
            TableOperation insertOperation = TableOperation.InsertOrReplace(adminJobEntity);
            adminJobTable.Execute(insertOperation);
        }
    }
}
