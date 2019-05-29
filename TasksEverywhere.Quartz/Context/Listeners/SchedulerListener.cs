using TasksEverywhere.Logging.Services.Concrete;
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
    public class SchedulerListener : ISchedulerListener
    {
        public Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(jobDetail.GetType(), MethodBase.GetCurrentMethod(), jobDetail.Key.Name));
        }

        public Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), jobKey.Name));
        }

        public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), jobKey.Name));
        }

        public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), jobKey.Name));
        }

        public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), jobKey.Name));
        }

        public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), trigger.Key.Name + " At " + trigger.GetNextFireTimeUtc().Value.ToLocalTime()));
        }

        public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), jobGroup));
        }

        public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), jobGroup));
        }

        public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), triggerKey.Name));
        }

        public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), msg, cause));
        }

        public Task SchedulerInStandbyMode(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => { });
        }

        public Task SchedulerShutdown(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => { });
        }

        public Task SchedulerShuttingdown(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => { });
        }

        public Task SchedulerStarted(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => { });
        }

        public Task SchedulerStarting(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => { });
        }

        public Task SchedulingDataCleared(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => { });
        }

        public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), trigger.Key.Name));
        }

        public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), triggerKey.Name));
        }

        public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), triggerKey.Name));
        }

        public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), triggerGroup));
        }

        public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), triggerGroup));
        }
    }
}
