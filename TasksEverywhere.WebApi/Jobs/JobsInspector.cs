using TasksEverywhere.DataLayer.Context.Concrete;
using TasksEverywhere.DataLayer.Utils;
using TasksEverywhere.HttpUtilities.HttpModel;
using TasksEverywhere.HttpUtilities.Service.Concrete;
using TasksEverywhere.HttpUtilities.Services.Concrete;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using TasksEverywhere.Quartz.Extensions;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace TasksEverywhere.WebApi.Jobs
{
    public class JobsInspector : ICustomJob
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; } = true;
        public List<IJobParameter> Parameters { get; set; }
        public List<ICustomTrigger> Triggers { get; set; }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                using (var dbContext = new QuartzExecutionDataContext())
                {
                    foreach (var job in dbContext.Jobs.ToList())
                    {
                        if (!job.IsExecutingCorrectly)
                        {
                            LogsAppendersManager.Instance.Error("Il job " + job.Key + " del cliente " + job.Server.Name + " su ip " + job.Server.IP + " non sta funzionando correttamente");
                            try
                            {
                                WebSocketClient.SendMessage(new SocketMessage
                                {
                                    Title = "Il job " + job.Key + " del cliente " + job.Server.Name + " su ip " + job.Server.IP + " non sta funzionando correttamente",
                                    DataType = typeof(string),
                                    IsImportant = true,
                                    Data = ""

                                });
                            }
                            catch (Exception ex) { LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex); }
                        }
                    }
                }
            });
        }

        public global::Quartz.IJobDetail GetQuartzJob()
        {
          

            return JobBuilder.Create(this.GetType())
                .WithDescription(this.Description)
                .WithIdentity(new JobKey(this.Name, this.Group))
                .Build();
        }

        public List<global::Quartz.ITrigger> GetQuartzTriggers()
        {
            List<ITrigger> triggers = new List<ITrigger>();

            this.Triggers.ForEach(x => {
                var trigger = x.GetQuartzTrigger(this.Name, this.Group);
                trigger.Log();
                triggers.Add(trigger);
            });
            return triggers;
        }
    }
}