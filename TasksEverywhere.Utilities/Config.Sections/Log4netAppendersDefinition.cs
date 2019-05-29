using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Utilities.Config.Sections
{
    public sealed class Log4netAppendersDefinition : ConfigurationSection
    {
        public static Log4netAppendersDefinition GetConfig()
        {
            string path = Process.GetCurrentProcess().MainModule.FileName;
            if(!path.ToLower().Contains("iis") && !path.ToLower().Contains("inetsrv"))
            {
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(path);
                return (Log4netAppendersDefinition)cfg.GetSection("Log4netAppendersDefinition") ?? new Log4netAppendersDefinition();
            }
            else
                return (Log4netAppendersDefinition)ConfigurationManager.GetSection("Log4netAppendersDefinition") ?? new Log4netAppendersDefinition();

            //using (var eventLog = new EventLog("Application"))
            //{
            //    eventLog.Source = System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);
            //    eventLog.WriteEntry(typeof(LoggerSection).Name + " Exe path " + path, EventLogEntryType.Information);
            //}
        }

        [ConfigurationProperty("appenders")]
        [ConfigurationCollection(typeof(Log4netAppendersElement), AddItemName = "appender")]
        public Log4netAppendersCollection Appenders
        {
            get
            {
                return ((Log4netAppendersCollection)base["appenders"]);
            }
        }
    }
}
