using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using TasksEverywhere.Quartz.Jobs.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Context.Jobs.Concrete
{
    public class ScheduledJobs : IScheduledJobs
    {
        public List<ICustomJob> Jobs { get; set; } = new List<ICustomJob>();
    } 
}
