using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TasksEverywhere.Logging.Services.Concrete;

namespace TasksEverywhere.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new IceQuartzScheduler()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(typeof(IceQuartzScheduler), MethodBase.GetCurrentMethod(), "Eccezione durante l'inizializzazione", ex);
                throw ex;
            }
        }
    }
}
