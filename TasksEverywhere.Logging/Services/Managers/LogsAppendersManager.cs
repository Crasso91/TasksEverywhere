using TasksEverywhere.Utilities.Config.Sections;
using TasksEverywhere.Utilities.Enums;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Logging.Services.Concrete
{
    public class LogsAppendersManager : BaseLoggingService<LogsAppendersManager>
    {
        public static readonly List<log4net.ILog> loggers = new List<log4net.ILog>();

        public override void Configure()
        {
            XmlConfigurator.Configure();
            var config = Log4netAppendersDefinition.GetConfig();
            foreach(var log in config.Appenders.Cast<Log4netAppendersElement>())
            {
                if(log.Active) loggers.Add(log4net.LogManager.GetLogger(log.Name));
            }
        }

        public log4net.ILog GetLogger(string loggerType)
        {
            return loggers.FirstOrDefault(x => x.GetType().Name.ToLower().Contains(loggerType.ToLower()));
        }

        public void Error(string message)
        {
            foreach (var logger in loggers)
            {
                logger.Error(message);
            }
        }

        public void Error(Type type, object methodBase)
        {
            throw new NotImplementedException();
        }

        public override void Debug(Type type, MethodBase method, string message)
        {
            foreach (var logger in loggers)
            {
                logger.Debug(GetFormattedMessage(type, method, message));
            }
        }

        public override void Error(Type type, MethodBase method, string message, Exception ex)
        {
            foreach (var logger in loggers)
            {
                logger.Error(GetFormattedMessage(type, method, message), ex);
            }
        }

        public override void Info(Type type, MethodBase method, string message)
        {
            foreach (var logger in loggers)
            {
                logger.Info(GetFormattedMessage(type, method, message));
            }
        }

        public override void ConfiguredLog(LogLevelType logLevel, Type type, MethodBase method, string message, Exception ex = null)
        {
            switch (logLevel)
            {
                case LogLevelType.Info:
                    Info(type, method, message);
                    break;
                case LogLevelType.Debug:
                    Debug(type, method, message);
                    break;
                case LogLevelType.Error:
                    Error(type, method, message, ex);
                    break;
                default:
                    break;
            }
        }
    }
}
