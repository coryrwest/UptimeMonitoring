using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using UptimeMonitoring.Models;
using System.Diagnostics;
using UptimeMonitoring.Controllers;

namespace UptimeMonitoring
{
    public class JobConfig
    {
        public static void JobStart()
        {
            //NameValueCollection properties = new NameValueCollection();

            //properties["quartz.scheduler.instanceName"] = "JobScheduler";
            //properties["quartz.scheduler.instanceId"] = "JS1";
            //properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            //properties["quartz.threadPool.threadCount"] = "10";
            //properties["quartz.threadPool.threadPriority"] = "Normal";
            //properties["quartz.jobStore.misfireThreshold"] = "60000";
            //properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            //properties["quartz.jobStore.useProperties"] = "true";
            //properties["quartz.jobStore.dataSource"] = "default";
            //properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
            //properties["quartz.jobStore.clustered"] = "true";
            //properties["quartz.scheduler.proxy"] = "true";
            //properties["quartz.scheduler.proxy.address"] = "tcp://corysprojects.cloudapp.net:555/QuartzScheduler";
            //properties["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz";
            //properties["quartz.dataSource.default.connectionString"] = "UptimeMonitorDb";
            //properties["quartz.dataSource.default.provider"] = "SqlServer-20";

            DateTimeOffset runtime = DateBuilder.EvenMinuteDateAfterNow();

            //Set Quartz.net scheduler
            ISchedulerFactory schedService = new StdSchedulerFactory();
            IScheduler sched = schedService.GetScheduler();

            //--------------------- 1 minute checks ---------------------//
            //Set 1 minute job details
            IJobDetail oneMinuteJob = JobBuilder.Create<SiteCheckJob>()
                .WithIdentity("oneMinuteCheck")
                .Build();

            oneMinuteJob.JobDataMap.Put(SiteCheckJob.Frequency, 1);

            //Set 1 minute trigger details
            ITrigger oneMinuteTrigger = TriggerBuilder.Create()
                .WithIdentity("oneMinute")
                .StartAt(runtime)
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).WithRepeatCount(10))
                .Build();

            //--------------------- 5 minute checks ---------------------//
            //Set 5 minute job details
            IJobDetail fiveMinuteJob = JobBuilder.Create<SiteCheckJob>()
                .WithIdentity("fiveMinuteCheck")
                .Build();

            fiveMinuteJob.JobDataMap.Put(SiteCheckJob.Frequency, 5);

            //Set 5 minute trigger details
            ITrigger fiveMinuteTrigger = TriggerBuilder.Create()
                .WithIdentity("fiveMinute")
                .StartAt(runtime)
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).WithRepeatCount(10))
                .Build();

            //Schedule Checks
            sched.ScheduleJob(oneMinuteJob, oneMinuteTrigger);
            sched.ScheduleJob(fiveMinuteJob, fiveMinuteTrigger);

            //Start scheduler
            sched.Start();
        }

        public class SiteCheckJob : IJob
        {
            public const string Frequency = "5";
            public void Execute(IJobExecutionContext context)
            {
                JobDataMap data = context.JobDetail.JobDataMap;
                int frequency = data.GetIntValue(Frequency);
                SiteCheck sitechecker = new SiteCheck();
                sitechecker.CheckSite(frequency);
            }
        }
    }
}