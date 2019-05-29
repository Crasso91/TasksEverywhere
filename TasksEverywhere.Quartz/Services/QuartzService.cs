using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.CastleWindsor.Services.Abstract;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Quartz.Context.Abstract;
using TasksEverywhere.Quartz.Context.Concrete;
using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using TasksEverywhere.Quartz.Context.Listeners;
using TasksEverywhere.Quartz.Extensions;
using TasksEverywhere.Utilities.Exceptions;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Services
{
    public class QuartzService : ISingletonService
    {
        public IScheduler Scheduler { get; set; }
        private bool WithListeners = true;
        public QuartzService()
        {
            var factory = new StdSchedulerFactory();
            var scheduler = factory.GetScheduler().Result;
            Scheduler = scheduler;
         //   WithListeners = withListener;
        }

        public void Init()
        {
            try
            {
                IQuartzContext context = null;
                try
                {
                    context = QuartzJsonContext.Instance;
                }
                catch (Exception e)
                {
                    LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), e.Message, e);
                }
                if (WithListeners) Scheduler.ListenerManager.AddSchedulerListener(new SchedulerListener());
                if(context != null) {
                    foreach (var customJob in context.data.Jobs.Where(job => job.Active))
                    {
                        try
                        {

                            if (!Scheduler.CheckExists(new JobKey(customJob.Name, customJob.Group)).Result)
                            {
                                var job = customJob.GetQuartzJob();
                                job.Log();
                                Scheduler.AddJob(job, true);
                                var triggers = customJob.GetQuartzTriggers();

                                Scheduler.ScheduleJob(job, triggers, true);
                                RemoteLoggerManager.Instance.AddJob(job);

                            }
                            else throw new EntityDuplicationException("Cant't load duplicated job with key: " + customJob.Name + "." + customJob.Group);

                        }
                        catch (InvalidIntervalUnitException ex)
                        {
                            LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "Leggilo er commento pero'! " + ex.Message, ex);
                        }
                        catch (InvalidLifeUnitException ex)
                        {
                            LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "Leggilo er commento pero'! " + ex.Message, ex);
                        }
                        catch (EntityDuplicationException ex)
                        {
                            LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
                        }
                    }
                    if (WithListeners) Scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
                    if (WithListeners) Scheduler.ListenerManager.AddTriggerListener(new TriggerListener(), GroupMatcher<TriggerKey>.AnyGroup());
                }
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
            }
        }

        public void AddJob(ICustomJob customJob)
        {
            var job = customJob.GetQuartzJob();
            job.Log();
            var triggers = customJob.GetQuartzTriggers();
            Scheduler.AddJob(job, true);
            Scheduler.ScheduleJob(job, triggers, true);
        }

        public void Start() => Scheduler.Start();
        public void Shutdown() => Scheduler.Shutdown().Wait();
        public void Standby() => Scheduler.PauseAll();
        public void Continue() => Scheduler.ResumeAll();
        public void Restart()
        {
            this.Shutdown();
            this.Start();
        }
        public void Reinit()
        {
            this.Init();
        }

    }
}
