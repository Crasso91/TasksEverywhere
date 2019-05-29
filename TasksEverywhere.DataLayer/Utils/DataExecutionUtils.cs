using TasksEverywhere.DataLayer.Context.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.DataLayer.Utils
{
    public class DataExecutionUtils
    {
        public static bool IsJobExecutingCorrectly(long jobId)
        {
            var _context = new QuartzExecutionDataContext();
            var lastCall = (from job in _context.Jobs
                            join cal in _context.Calls on job.JobID equals cal.JobID
                            where job.JobID == jobId
                            orderby cal.EndedAt descending
                            select cal).FirstOrDefault();
            return DateTime.Now <= lastCall.NextStart.AddMinutes(2);
        }
    }
}
