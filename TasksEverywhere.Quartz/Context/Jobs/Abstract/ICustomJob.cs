using log4net.Core;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Context.Jobs.Abstract
{
    public interface ICustomJob : IJob
    {
        string Name { get; set; }
        string Group { get; set; }
        string Description { get; set; }
        bool Active { get; set; }
        List<IJobParameter> Parameters { get; set; }
        List<ICustomTrigger> Triggers {get; set;}
        IJobDetail GetQuartzJob();
        List<ITrigger> GetQuartzTriggers();
    }
}
