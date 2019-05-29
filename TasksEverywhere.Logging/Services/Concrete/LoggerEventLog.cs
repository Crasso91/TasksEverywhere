using ICeScheduler.Quartz.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICeScheduler.Quartz.Services.Concrete
{
    public class LoggerEventLog : BaseLoggingService<LoggerEventLog>
    {
        public LoggerEventLog()
        {
            this.Configure();
        }

        public override void Configure()
        {
            base.Configure();
            bool sourceExists = EventLog.SourceExists(System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName));
            if (!sourceExists)
            {
                EventLog.CreateEventSource(System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName), "Application");
            }
        }

        public override void Debug(Type type, MethodBase method, string message)
        {
            if (LogLevel != LogLevelType.Debug) return;

            using (var eventLog = new EventLog("Application"))
            {
                eventLog.Source = System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);;
                eventLog.WriteEntry(GetFormattedMessage(type, method, message), EventLogEntryType.Information, 101, 1);
            }
        }

        public override void Error(Exception ex)
        {
            using (var eventLog = new EventLog("Application"))
            {
                eventLog.Source = System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);;
                eventLog.WriteEntry(GetFormattedMessage(this.GetType(), MethodBase.GetCurrentMethod(), ex.StackTrace), EventLogEntryType.Error, 101, 1);
            }
        }

        public override void Error(string msg, Exception ex)
        {
            using (var eventLog = new EventLog("Application"))
            {
                eventLog.Source = System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);;
                eventLog.WriteEntry(GetFormattedMessage(this.GetType(), MethodBase.GetCurrentMethod(), msg + " \r\n " + ex.StackTrace), EventLogEntryType.Error, 101, 1);
            }
        }

        public override void Error(Type type, MethodBase method, string message, Exception ex)
        {
            using (var eventLog = new EventLog("Application"))
            {
                eventLog.Source = System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);;
                eventLog.WriteEntry(GetFormattedMessage(type, method, message), EventLogEntryType.Error, 101, 1);
            }
        }

        public override void Info(string msg)
        {
            if (LogLevel == LogLevelType.Error) return;
            using (var eventLog = new EventLog("Application"))
            {
                eventLog.Source = System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);;
                eventLog.WriteEntry(msg, EventLogEntryType.Information, 101, 1);
            }
        }

        public override void Info(Type type, MethodBase method, string message)
        {
            if (LogLevel == LogLevelType.Error) return;
            using (var eventLog = new EventLog("Application"))
            {
                eventLog.Source = System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);;
                eventLog.WriteEntry(GetFormattedMessage(type, method, message), EventLogEntryType.Information, 101, 1);
            }
        }
    }
}
