using TasksEverywhere.Quartz.Context.Abstract;
using System.Collections.Generic;
using System.Linq;
using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using Quartz;
using TasksEverywhere.Quartz.Jobs.Abstract;
using TasksEverywhere.Quartz.Context.Jobs.Concrete;
using Newtonsoft.Json;
using TasksEverywhere.Quartz.Services;
using System.Reflection;
using TasksEverywhere.DataLayer.Context.Abstract;
using TasksEverywhere.Logging.Services.Concrete;

namespace TasksEverywhere.Quartz.Context.Concrete
{
    public class QuartzJsonContext : IQuartzContext
    {
        private static QuartzJsonContext _instance;
        public IScheduledJobs data { get; set; }

        public static IQuartzContext Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new QuartzJsonContext();
                    _instance.Init();
                }
                return _instance;
            }
        }

        public QuartzJsonContext()
        {
            this.Init();
        }

        public IConnection Connection { get; set; } = new JsonConnection();

        public void Init()
        {
            this.Connect();
            LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Context Initialized");
        }

        public void Connect()
        {
            var path = Connection.GetProperty<string>("Path");
            if (System.IO.File.Exists(path))
            {
                var config = System.IO.File.ReadAllText(path);
                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All
                };
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<ScheduledJobs>(config, settings);
            }
            else data = new ScheduledJobs();
        }

        public void Add(ICustomJob job)
        {
            if(!data.Jobs.Exists(x => x.Name == job.Name && job.Group == job.Group))
            {
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Added new job  : " + job.Name + " json: " + Newtonsoft.Json.JsonConvert.SerializeObject(job));
                data.Jobs.Add(job);
            }
        }

        public void Delete(ICustomJob job)
        {
            var _job = data.Jobs.SingleOrDefault(x=>x.Name == job.Name && job.Group == job.Group);
            data.Jobs.Remove(_job);
            LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Deleted job  : " + job.Name);
        }

        public void Update(ICustomJob job)
        {
            var _job = data.Jobs.First(x => x.Name == job.Name && job.Group == job.Group);
            var index = data.Jobs.IndexOf(_job);
            data.Jobs[index] = job;
            LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Updated job  : " + job.Name + " json: " + Newtonsoft.Json.JsonConvert.SerializeObject(job));
        }

        public void Commit()
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Formatting.Indented, settings);
            var path = Connection.GetProperty<string>("Path");
            System.IO.File.WriteAllText(path, json);
            LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Context Commit");

        }

        public void Reload()
        {
            data = null;
            this.Init();
            LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Context Releoaded");
        }
    }
}
