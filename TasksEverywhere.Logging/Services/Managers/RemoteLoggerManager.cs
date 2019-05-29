using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.Logging.Services.Abstract;
using TasksEverywhere.Utilities.Config.Sections;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Logging.Services.Concrete
{
    public class RemoteLoggerManager : BaseLoggingService<RemoteLoggerManager>, IWebApiLogger
    {
        private List<IWebApiLogger> loggers = new List<IWebApiLogger>();

        public override void Configure()
        {
            var config = LoggerSection.GetConfig();
            foreach (var logger in config.Loggers.Cast<LoggerSectionElement>())
            {
                var loggerInstance = CastleWindsorService.Resolve(logger.Type, null);
                if(loggerInstance.GetType().GetInterface(typeof(IWebApiLogger).Name) != null)
                    loggers.Add(loggerInstance as IWebApiLogger);
            }
        }

        public void AddJob(IJobDetail jobDetail)
        {
            foreach(var logger in loggers)
            {
                if (logger.Active) logger.AddJob(jobDetail);
            }
        }

        public void CallEnd(IJobExecutionContext jobContext)
        {
            foreach (var logger in loggers)
            {
                if (logger.Active) logger.CallEnd(jobContext);
            }
        }

        public void CallStart(IJobExecutionContext jobContext)
        {
            foreach (var logger in loggers)
            {
                if (logger.Active) logger.CallStart(jobContext);
            }
        }
        public void Error(IJobExecutionContext jobContext, Exception ex)
        {
            foreach (var logger in loggers)
            {
                if (logger.Active) logger.Error(jobContext, ex);
            }
        }
    }
}
