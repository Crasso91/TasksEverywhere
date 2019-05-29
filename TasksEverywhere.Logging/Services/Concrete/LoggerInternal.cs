using ICeScheduler.Quartz.Config.Sections;
using ICeScheduler.Quartz.Services.Abstract;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICeScheduler.Quartz.Services.Concrete 
{
    public class LoggerInternal : BaseLoggingService<LoggerInternal>
    {
        public static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoggerInternal));

        public LoggerInternal()
        {
            this.Configure();
        }

        public override void Configure()
        {
            base.Configure();
            //PatternLayout layout = new PatternLayout(@"%-5p %d{MM-dd hh:mm:ss.ffff}  [%thread]  %m%n");
            //RollingFileAppender appender = new RollingFileAppender();
            //appender.Layout = layout;
            //appender.File = parameters["LogFilePath"].Value;
            //appender.AppendToFile = true;
            //appender.MaximumFileSize = "10MB";
            //appender.MaxSizeRollBackups = 0; //no backup files
            //appender.ActivateOptions();

            //switch (LogLevel)
            //{
            //    case Enums.LogLevelType.Info:
            //        appender.Threshold = log4net.Core.Level.Info;
            //        break;
            //    case Enums.LogLevelType.Debug:
            //        appender.Threshold = log4net.Core.Level.Debug;
            //        break;
            //    case Enums.LogLevelType.Error:
            //        appender.Threshold = log4net.Core.Level.Error;
            //        break;
            //    default:
            //        break;
            //}
            //log4net.Config.BasicConfigurator.Configure(appender);
        }

        public override void Debug(Type type, MethodBase method, string message)
        {
            logger.Debug(GetFormattedMessage(type, method, message));
        }

        public override void Error(Exception ex)
        {
            logger.Error(ex);
        }

        public override void Error(string msg, Exception ex)
        {
            logger.Error(msg, ex);
        }

        public override void Error(Type type, MethodBase method, string message, Exception ex)
        {
            logger.Error(GetFormattedMessage(type, method, message), ex);
        }

        public override void Info(string msg)
        {
            logger.Info(msg);
        }

        public override void Info(Type type, MethodBase method, string message)
        {
            logger.Info(GetFormattedMessage(type, method, message));
        }

    }
}
