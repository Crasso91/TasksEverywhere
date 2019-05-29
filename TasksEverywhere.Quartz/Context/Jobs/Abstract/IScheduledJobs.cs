using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using System.Collections.Generic;

namespace TasksEverywhere.Quartz.Jobs.Abstract
{
    public interface IScheduledJobs
    {
        List<ICustomJob> Jobs { get; set; }
    }
}
