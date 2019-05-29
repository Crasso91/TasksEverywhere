using TasksEverywhere.CastleWindsor.Services.Abstract;
using TasksEverywhere.DataLayer.Context.Abstract;
using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using TasksEverywhere.Quartz.Jobs.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Context.Abstract
{
    public interface IQuartzContext : ISingletonService
    {
        IConnection Connection { get; set; }
        IScheduledJobs data { get; set; }
        void Connect();
        void Init();
        void Commit();
        void Reload();
        void Add(ICustomJob job);
        void Delete(ICustomJob job);
        void Update(ICustomJob job);

    }
}
