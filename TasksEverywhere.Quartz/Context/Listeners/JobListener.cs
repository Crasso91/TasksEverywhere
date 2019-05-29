using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Quartz.Extensions;
using TasksEverywhere.Quartz.Services;
using TasksEverywhere.Utilities.Enums;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Context.Listeners
{
    public class JobListener : IJobListener
    {
        public string Name => this.GetType().Name;

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                /*
                if(context.GetLogLevel() == Enums.LogLevelType.Nil)
                    LogsAppendersManager.Instance.Info(context.JobDetail.GetType(), MethodBase.GetCurrentMethod(), "Trigger " + context.JobDetail.Key + " Vetoed");
                else
                    LogsAppendersManager.Instance.ConfiguredLog(context.GetLogLevel(), context.JobDetail.GetType(), MethodBase.GetCurrentMethod(), "Trigger " + context.JobDetail.Key + " Vetoed");
                */
                return context.Recovering;

            });
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => {
                if (context.GetLogLevel() == LogLevelType.Nil)
                    LogsAppendersManager.Instance.Info(context.JobDetail.GetType(), MethodBase.GetCurrentMethod(), "Job " + context.JobDetail.Key + " Started");
                else
                    LogsAppendersManager.Instance.ConfiguredLog(context.GetLogLevel(), context.JobDetail.GetType(), MethodBase.GetCurrentMethod(), "Job " + context.JobDetail.Key + " Started");
                RemoteLoggerManager.Instance.CallStart(context);
            });
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(jobException == null)
                return Task.Run(() => {
                    if (context.GetLogLevel() == LogLevelType.Nil)
                        LogsAppendersManager.Instance.Info(context.GetType(), MethodBase.GetCurrentMethod(), "Job " + context.JobDetail.Key + " Executed");
                    else
                        LogsAppendersManager.Instance.ConfiguredLog(context.GetLogLevel(), context.JobDetail.GetType(), MethodBase.GetCurrentMethod(), "Job " + context.JobDetail.Key + " Executed");
                    RemoteLoggerManager.Instance.CallEnd(context);
                });
            else
                return Task.Run(() => {
                    if (context.GetLogLevel() == LogLevelType.Nil)
                        LogsAppendersManager.Instance.Error(context.JobDetail.GetType(), MethodBase.GetCurrentMethod(), "Job Executed with Error: " + context.JobDetail.Key, jobException);
                    else
                        LogsAppendersManager.Instance.ConfiguredLog(context.GetLogLevel(), context.JobDetail.GetType(), MethodBase.GetCurrentMethod(), "Job Executed with Error: " + context.JobDetail.Key, jobException);
                    RemoteLoggerManager.Instance.Error(context, jobException);
                });
        }
    }
}
