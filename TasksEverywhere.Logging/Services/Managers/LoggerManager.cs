using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.Logging.Services.Abstract;
using TasksEverywhere.Utilities.Config.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Logging.Services.Concrete
{
    public class LoggerManager : BaseLoggingService<LoggerManager>
    {
        private List<ILoggingService> loggers = new List<ILoggingService>();

        public override void Configure()
        {
            var config = Utilities.Config.Sections.LoggerSection.GetConfig();
            foreach(var logger in config.Loggers.Cast<LoggerSectionElement>())
            {
                var loggerInstance = CastleWindsorService.Resolve(logger.Type, null);
                loggers.Add(loggerInstance as ILoggingService);
            }
        }
        public override void Debug(Type type, MethodBase method, string message)
        {
            foreach(var logger in loggers)
            {
               if(logger.Active) logger.Debug(type, method, message);
            }
        }

        public override void Error(Type type, MethodBase method, string message, Exception ex)
        {
            foreach (var logger in loggers)
            {
                if (logger.Active) logger.Error(type, method, message, ex);
            }
        }
        public override void Info(Type type, MethodBase method, string message)
        {
            foreach (var logger in loggers)
            {
                if (logger.Active) logger.Info(type, method, message);
            }
        }
    }
}
