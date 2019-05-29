using TasksEverywhere.DataLayer.Context.Concrete;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Context.Concrete
{
    public class JsonConnection : BaseConnection
    {
        public string Path
        {
            get; private set;
        }

        public JsonConnection()
        {
            var appSettings = ConfigurationManager.OpenExeConfiguration(Process.GetCurrentProcess().MainModule.FileName).AppSettings;
            var path = string.Empty;
            var configPath = appSettings.Settings["JsonConnection"].Value;
            if (!System.IO.Path.IsPathRooted(configPath)) Path = configPath;
            else Path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + configPath;
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(Path))) Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
        }

        public JsonConnection(string filePath)
        {
            if (System.IO.Path.IsPathRooted(filePath)) Path = filePath;
            else Path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + filePath;
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(Path))) Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
        }

    }
}
