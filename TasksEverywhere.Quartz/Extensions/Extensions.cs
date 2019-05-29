using TasksEverywhere.Extensions;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Utilities.Enums;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Extensions
{
    public static class Extensions
    {
        public static void Log(this ITrigger _in)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Trigger " + _in.Key);
            var _props = _in.GetProperties();
            foreach (var prop in _props)
            {
                sb.AppendLine(" - " + prop.Name + " : " + prop.GetValue(_in));
            }
            LogsAppendersManager.Instance.Debug(typeof(ITrigger), MethodBase.GetCurrentMethod(), sb.ToString());
        }

        public static void Log(this IJobDetail _in)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Job " + _in.Key);
            var _props = _in.GetProperties();
            foreach (var prop in _props)
            {
                sb.AppendLine(" - " + prop.Name + " : " + prop.GetValue(_in));
            }
            LogsAppendersManager.Instance.Debug(typeof(IJobDetail), MethodBase.GetCurrentMethod(), sb.ToString());
        }

        public static bool _IsPrimitive(this Type type)
        {
            return type.IsPrimitive || type.Equals(typeof(String)) || type.Equals(typeof(Char)) || type.Equals(typeof(DateTime)) || type.IsEnum;
        }

        public static LogLevelType GetLogLevel(this IJobExecutionContext jobContext)
        {
            try
            {
                return jobContext.JobDetail.JobDataMap.GetString(JobDataMapKeys.LogLevel.ToString()).ToEnum<LogLevelType>();
            }
            catch (Exception)
            {
                return LogLevelType.Nil;
            }
        }
    }
}
