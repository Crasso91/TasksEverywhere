using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Quartz.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TasksEverywhere.WindowsService
{
    partial class IceQuartzScheduler : ServiceBase
    {
        QuartzService _service;
        //CancellationTokenSource fsToken;
        public IceQuartzScheduler()
        {
            InitializeComponent();
            CastleWindsorService.Init();
            LogsAppendersManager.Instance.Info(this.GetType(), MethodBase.GetCurrentMethod(), "Service inizilization");
            _service = CastleWindsorService.Resolve<QuartzService>();
            _service.Init();
        }

        protected override void OnStart(string[] args)
        {
            _service.Start();
           // fsToken = FileSystemWatcherService.Activate();
            LogsAppendersManager.Instance.Info(this.GetType(), MethodBase.GetCurrentMethod(), "Service Started");
        }

        protected override void OnStop()
        {
            _service.Shutdown();
            //fsToken.Cancel();
            LogsAppendersManager.Instance.Info(this.GetType(), MethodBase.GetCurrentMethod(), "Service Stopped");
        }

        protected override void OnPause()
        {
            _service.Standby();
            LogsAppendersManager.Instance.Info(this.GetType(), MethodBase.GetCurrentMethod(), "Service Paused");
        }

        protected override void OnContinue()
        {
            _service.Continue();
            LogsAppendersManager.Instance.Info(this.GetType(), MethodBase.GetCurrentMethod(), "Service Resumed");
        }
    }
}
