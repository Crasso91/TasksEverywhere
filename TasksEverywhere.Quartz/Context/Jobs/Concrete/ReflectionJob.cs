using Castle.MicroKernel.Registration;
using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.CastleWindsor.Services.Abstract;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using TasksEverywhere.Quartz.Context.Models;
using TasksEverywhere.Quartz.Extensions;
using TasksEverywhere.Utilities.Enums;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Context.Jobs.Concrete
{
    public class ReflectionJob : ICustomJob
    {
        public string Name  { get; set; }
        public string Group  { get; set; }
        public string Description  { get; set; }
        public bool Active { get; set; }
        public List<IJobParameter> Parameters { get; set; } = new List<IJobParameter>();
        public List<ICustomTrigger> Triggers { get; set; } = new List<ICustomTrigger>();

        public Task Execute(IJobExecutionContext context)
        {
            var logId = "Fire Instance ID : " + context.FireInstanceId + " Task: " + context.JobDetail.Key + " --> ";
            var reflectionJobDatas = (context.JobDetail.JobDataMap.Get(JobDataMapKeys.ReflectionJobData.ToString()) as List<ReflectionJobData>);
            var logLevel = context.GetLogLevel();
            return Task.Run(() =>
            {
                foreach (var data in reflectionJobDatas)
                {
                    try
                    {
                        InvokeMethod(data);
                    }
                    catch (Exception ex)
                    {
                        LogsAppendersManager.Instance.ConfiguredLog(logLevel, this.GetType(), MethodBase.GetCurrentMethod(), logId + " Exception", ex);
                    }
                }
            });
        }

        public IJobDetail GetQuartzJob()
        {
            IDictionary<string, object> _params = new Dictionary<string, object>();
            Parameters.ForEach(x => _params.Add(x.Key.ToString(), x.Value));

            return JobBuilder.Create(this.GetType())
                .SetJobData(new JobDataMap(_params))
                .WithDescription(this.Description)
                .WithIdentity(new JobKey(this.Name, this.Group))
                .Build();
        }

        public List<ITrigger> GetQuartzTriggers()
        {
            List<ITrigger> triggers = new List<ITrigger>();

            this.Triggers.Where(trigger => trigger.Active).ToList().ForEach(x => {
                var trigger = x.GetQuartzTrigger(this.Name, this.Group);
                trigger.Log();
                triggers.Add(trigger);
            });
            return triggers;
        }

        private void InvokeMethod(ReflectionJobData data)
        {
            try
            {
                var constuctorArgs = (object[]) data.ConstructorArgs.Split(';');
                var methodArgs =(object[]) data.MethodArgs.Split(';');

                var assembly = Assembly.LoadFile(data.LibraryPath);
                var type = assembly.GetTypes().FirstOrDefault(x => x.Name == data.ClassName);
                var obj = Activator.CreateInstance(type, constuctorArgs.Length > 0 ? constuctorArgs : null);
                var method = obj.GetType().GetMethod(data.MethodName);
                method.Invoke(obj, methodArgs.Length > 0 ? methodArgs : null);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
