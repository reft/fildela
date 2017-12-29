using Fildela.Data.Database.DataLayer;
using Fildela.Data.Database.Models;
using Fildela.Data.Storage.Models;
using Fildela.Data.Storage.Services;
using Fildela.Worker.Helpers;
using Microsoft.WindowsAzure.Storage.Table;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Fildela.Worker.AdminJobs.Database
{
    public class AdminJobAccountLinks : IJob
    {
        CloudStorageServices CloudStorageService = new CloudStorageServices();
        DataLayer DB = new DataLayer();

        public void Execute(IJobExecutionContext context)
        {
            DateTime currentTime = WorkerTimeZoneExtensions.GetCurrentDate();

            List<AccountLink> accountLinks = (from a in DB.AccountLink
                                              where SqlFunctions.DateDiff("second", currentTime, a.DateExpires) < 0
                                              select a).ToList();

            int affectedRows = 0;
            int errorCount = 0;

            if (accountLinks != null && accountLinks.Count > 0)
            {
                foreach (var item in accountLinks)
                {
                    //Delete permissions
                    List<AccountLinkPermission> accountLinkPermission = (from a in DB.AccountLinkPermission
                                                                         where a.UserID == item.UserID &&
                                                                         a.GuestID == item.GuestID
                                                                         select a).ToList();

                    if (accountLinkPermission != null && accountLinkPermission.Count > 0)
                    {
                        foreach (var permissions in accountLinkPermission)
                        {
                            DB.AccountLinkPermission.Remove(permissions);
                        }
                    }

                    //Delete account link
                    DB.AccountLink.Remove(item);
                }

                try
                {
                    DB.SaveChanges();

                    affectedRows = accountLinks.Count();
                }
                catch
                {
                    errorCount = accountLinks.Count();
                }
            }

            //Insert adminjob entity
            string rowKey = "accountlinks-" + currentTime.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString();
            AdminJob adminJobEntity = new AdminJob(rowKey, "Database.AccountLinks", affectedRows, errorCount, currentTime, WorkerTimeZoneExtensions.GetCurrentDate());

            CloudTable adminJobTable = CloudStorageService.GetCloudAdminJobTable();
            TableOperation insertOperation = TableOperation.InsertOrReplace(adminJobEntity);
            adminJobTable.Execute(insertOperation);
        }
    }
}
