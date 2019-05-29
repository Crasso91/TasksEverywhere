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
    public class TriggerListener : ITriggerListener
    {
        public string Name => this.GetType().Name;

        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                /* if (context.GetLogLevel() == Enums.LogLevelType.Nil)
                    LogsAppendersManager.Instance.Info(trigger.GetType(), MethodBase.GetCurrentMethod(), "Trigger " + trigger.Key + " Completed");
                else
                    LogsAppendersManager.Instance.ConfiguredLog(context.GetLogLevel(), trigger.GetType(), MethodBase.GetCurrentMethod(), "Trigger " + trigger.Key + " Completed");
                */
            });
        }

        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => {
                /*if (context.GetLogLevel() == Enums.LogLevelType.Nil)
                    LogsAppendersManager.Instance.Info(trigger.GetType(), MethodBase.GetCurrentMethod(), "Trigger " + trigger.Key + " Started");
                else
                    LogsAppendersManager.Instance.ConfiguredLog(context.GetLogLevel(), trigger.GetType(), MethodBase.GetCurrentMethod(), "Trigger " + trigger.Key + " Started");
                */
            });
        }

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => {
                    LogsAppendersManager.Instance.Debug(trigger.GetType(), MethodBase.GetCurrentMethod(), "Trigger " + trigger.Key + " Misfired");
            });
        }

        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                /*if (context.GetLogLevel() == Enums.LogLevelType.Nil)
                    LogsAppendersManager.Instance.Info(trigger.GetType(), MethodBase.GetCurrentMethod(), "Trigger " + trigger.Key + " Veto for context.Recovering");
                else
                    LogsAppendersManager.Instance.ConfiguredLog(context.GetLogLevel(), trigger.GetType(), MethodBase.GetCurrentMethod(), "Trigger " + trigger.Key + " Veto for context.Recovering");
                */
                return context.Recovering;
            });
        }
    }
}
