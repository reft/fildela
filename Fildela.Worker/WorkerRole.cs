using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;
using Quartz;
using Fildela.Worker.AdminJobs.Database;
using Fildela.Worker.AdminJobs.Storage;
using Quartz.Impl;

namespace Fildela.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("Fildela Worker entry point called");

            //AdminJobs
            ScheduleJob();

            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            return base.OnStart();
        }

        public static void ScheduleJob()
        {
            DateTimeOffset runTime = DateBuilder.EvenMinuteDate(DateTime.Now);
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            var cronScheduleBuilder = CronScheduleBuilder.DailyAtHourAndMinute(3, 0).InTimeZone(timeZoneInfo);

            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            JobDataMap jobDataMap = new JobDataMap();

            //Datebase
            IJobDetail accountLinksJobDetail = JobBuilder.Create<AdminJobAccountLinks>().WithIdentity("accountlinksjob", "group1").Build();

            //Storage
            IJobDetail accountLinkVerificationsJobDetail = JobBuilder.Create<AdminJobAccountLinkVerifications>().WithIdentity("accountlinkverificationsjob", "group2").UsingJobData(jobDataMap).Build();
            IJobDetail adminLogsJobDetail = JobBuilder.Create<AdminJobAdminLogs>().WithIdentity("adminlogsjob", "group2").UsingJobData(jobDataMap).Build();
            IJobDetail linksJobDetail = JobBuilder.Create<AdminJobLinks>().WithIdentity("linksjob", "group2").UsingJobData(jobDataMap).Build();
            IJobDetail logsJobDetail = JobBuilder.Create<AdminJobLogs>().WithIdentity("logsjob", "group2").UsingJobData(jobDataMap).Build();
            IJobDetail registerVerificationsJobDetail = JobBuilder.Create<AdminJobRegisterVerifications>().WithIdentity("registerverificationsjob", "group2").UsingJobData(jobDataMap).Build();
            IJobDetail resetPasswordsJobDetail = JobBuilder.Create<AdminJobResetPasswords>().WithIdentity("resetpasswordsjob", "group2").UsingJobData(jobDataMap).Build();
            IJobDetail uploadDirectlysJobDetail = JobBuilder.Create<AdminJobUploadDirectlys>().WithIdentity("uploaddirectlysjob", "group2").UsingJobData(jobDataMap).Build();
            IJobDetail usageJobDetail = JobBuilder.Create<AdminJobUsage>().WithIdentity("usagejob", "group2").UsingJobData(jobDataMap).Build();

            //Trigger 1.1
            ITrigger accountLinksJobTrigger = TriggerBuilder.Create()
                .WithIdentity("accountlinksjob", "group1")
                .StartAt(runTime)
                .WithSchedule(cronScheduleBuilder)
                .StartNow()
                .Build();

            //Schedule 1.1
            sched.ScheduleJob(accountLinksJobDetail, accountLinksJobTrigger);


            //Trigger 2.1
            ITrigger accountLinkVerificationsJobTrigger = TriggerBuilder.Create()
                .WithIdentity("accountlinkverificationsjob", "group2")
                .StartAt(runTime)
                .WithSchedule(cronScheduleBuilder)
                .StartNow()
                .Build();

            //Schedule 2.1
            sched.ScheduleJob(accountLinkVerificationsJobDetail, accountLinkVerificationsJobTrigger);


            //Trigger 2.2
            ITrigger adminLogsJobTrigger = TriggerBuilder.Create()
                .WithIdentity("adminlogsjob", "group2")
                .StartAt(runTime)
                .WithSchedule(cronScheduleBuilder)
                .StartNow()
                .Build();

            //Schedule 2.2
            sched.ScheduleJob(adminLogsJobDetail, adminLogsJobTrigger);


            //Trigger 2.3
            ITrigger linksJobTrigger = TriggerBuilder.Create()
                .WithIdentity("linksjob", "group2")
                .StartAt(runTime)
                .WithSchedule(cronScheduleBuilder)
                .StartNow()
                .Build();

            //Schedule 2.3
            sched.ScheduleJob(linksJobDetail, linksJobTrigger);


            //Trigger 2.4
            ITrigger logsJobTrigger = TriggerBuilder.Create()
                .WithIdentity("logsjob", "group2")
                .StartAt(runTime)
                .WithSchedule(cronScheduleBuilder)
                .StartNow()
                .Build();

            //Schedule 2.4
            sched.ScheduleJob(logsJobDetail, logsJobTrigger);


            //Trigger 2.5
            ITrigger registerVerificationsJobTrigger = TriggerBuilder.Create()
                .WithIdentity("registerverificationsjob", "group2")
                .StartAt(runTime)
                .WithSchedule(cronScheduleBuilder)
                .StartNow()
                .Build();

            //Schedule 2.5
            sched.ScheduleJob(registerVerificationsJobDetail, registerVerificationsJobTrigger);


            //Trigger 2.6
            ITrigger resetPasswordsJobTrigger = TriggerBuilder.Create()
                .WithIdentity("resetpasswordsjob", "group2")
                .StartAt(runTime)
                .WithSchedule(cronScheduleBuilder)
                .StartNow()
                .Build();

            //Schedule 2.6
            sched.ScheduleJob(resetPasswordsJobDetail, resetPasswordsJobTrigger);


            //Trigger 2.7
            ITrigger uploadDirectlysJobTrigger = TriggerBuilder.Create()
                .WithIdentity("uploaddirectlysjob", "group2")
                .StartAt(runTime)
                .WithSchedule(cronScheduleBuilder)
                .StartNow()
                .Build();

            //Schedule 2.7
            sched.ScheduleJob(uploadDirectlysJobDetail, uploadDirectlysJobTrigger);


            //Trigger 2.8
            ITrigger usageJobTrigger = TriggerBuilder.Create()
                .WithIdentity("usagejob", "group2")
                .StartAt(runTime)
                .WithSchedule(cronScheduleBuilder)
                .StartNow()
                .Build();

            //Schedule 2.8
            sched.ScheduleJob(usageJobDetail, usageJobTrigger);
        }
    }
}
