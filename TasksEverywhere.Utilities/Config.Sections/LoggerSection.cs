using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Utilities.Config.Sections
{
    public sealed class LoggerSection : ConfigurationSection
    {
        public static LoggerSection GetConfig()
        {
            string path = Process.GetCurrentProcess().MainModule.FileName;
            if (!path.ToLower().Contains("iis") && !path.ToLower().Contains("inetsrv"))
            {
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(path);
                return (LoggerSection)cfg.GetSection("LoggerSection") ?? new LoggerSection();
            }
            else
                return (LoggerSection)ConfigurationManager.GetSection("LoggerSection") ?? new LoggerSection();
        }

        [ConfigurationProperty("loggers")]
        [ConfigurationCollection(typeof(LoggerSectionElement), AddItemName = "logger")]
        public LoggerSectionCollection Loggers
        {
            get
            {
                return ((LoggerSectionCollection)base["loggers"]);
            }
        }
    }
}
