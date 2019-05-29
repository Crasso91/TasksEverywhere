using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Logging.Services.Abstract
{
    public interface IWebApiLogger : ILoggingService
    {
        void AddJob(IJobDetail jobDetail);
        void CallStart(IJobExecutionContext jobContext);
        void CallEnd(IJobExecutionContext jobContext);
        void Error(IJobExecutionContext jobContext, Exception ex);
    }
}
