using TasksEverywhere.Extensions;
using TasksEverywhere.Logging.Services.Abstract;
using TasksEverywhere.Utilities.Config.Sections;
using TasksEverywhere.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Logging.Services.Concrete
{
    public class BaseLoggingService<T> : ILoggingService
        where T : ILoggingService, new()
    {
        private static readonly string messageFormat = "{0}.{1} --> {2}";
        protected static LogLevelType LogLevel;
        private static T _instance = default(T);
        public bool Active { get; set; }
        public LoggerSectionParameterCollection parameters;
        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new T();
                    _instance.Configure();
                }
                return _instance;
            }
        }

        public virtual void Configure()
        {
            var config = LoggerSection.GetConfig();
            var section = config.Loggers.Cast<LoggerSectionElement>().FirstOrDefault(x => x.Type == typeof(T).Name);
            LogLevel = section != null ? section.Level.ToEnum<LogLevelType>() : LogLevelType.Debug;
            Active = section != null ? section.Active : true;
            parameters = section != null ? section.Parameters : new LoggerSectionParameterCollection();
        }
        public virtual void Debug(Type type, MethodBase method, string message) => throw new NotImplementedException(MethodBase.GetCurrentMethod().Name);
        public virtual void Error(Type type, MethodBase method, string message, Exception ex) => throw new NotImplementedException(MethodBase.GetCurrentMethod().Name);
        public virtual void Info(Type type, MethodBase method, string message) => throw new NotImplementedException(MethodBase.GetCurrentMethod().Name);
        public virtual void ConfiguredLog(LogLevelType logLevel, Type type, MethodBase method, string message, Exception ex = null) => throw new NotImplementedException(MethodBase.GetCurrentMethod().Name);
        public string GetFormattedMessage(Type type, MethodBase method, string message) => string.Format(messageFormat, type.Name, method.Name, message);
    }
}
