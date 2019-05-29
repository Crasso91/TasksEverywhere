using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.DataLayer.Context.Concrete;
using TasksEverywhere.DataLayer.Enumerators;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.HttpUtilities.Services.Concrete;
using TasksEverywhere.Logging.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using IntervalUnit = Quartz.IntervalUnit;

namespace TasksEverywhere.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CastleWindsorService.Init();
            var jobsInspector = new Jobs.JobsInspector()
            {
                Name = "Inspector",
                Group = "InspectorGroup",
                Description = "Jobs che si occupa di controllare se i job dai clienti stanno eseguendo correttamente",
                Triggers = new List<TasksEverywhere.Quartz.Context.Jobs.Abstract.ICustomTrigger>
                {
                    new Quartz.Context.Jobs.Concrete.CustomTrigger
                    {
                        Name = "InspectorTrigger",
                        Group = "InspectorTriggerGroup",
                        Interval = 1,
                        IntervalUnit = IntervalUnit.Minute,
                        Life = 23,
                        LifeUnit = IntervalUnit.Hour,
                        Period = TasksEverywhere.Utilities.Enums.PeriodType.Settimanale,
                        StartDate = DateTime.Parse("12/01/2019 00:00:00"),
                        WeekDays = new List<DayOfWeek>
                        {
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday,
                            DayOfWeek.Thursday,
                            DayOfWeek.Friday,
                            DayOfWeek.Saturday
                        },
                    }
                }
            };
            var quartzService = CastleWindsorService.Resolve<Quartz.Services.QuartzService>();
            quartzService.Init();
            quartzService.AddJob(jobsInspector);
            quartzService.Start();

            using (var _context = new QuartzExecutionDataContext())
            {
                var account = new Account
                {
                    Username = "Admin",
                    Password = "Ice!123",
                    Roles = RoleType.AdminAndInspector
                };
                if (_context.Accounts
                    .Where(x => x.Username == account.Username)
                    .FirstOrDefault() == null)
                    _context.Accounts.Add(account);
                _context.SaveChanges();

            }
            try
            {
                WebSocketClient.Init(ConfigurationManager.AppSettings["WebSocketUrl"]);
            }
            catch(Exception e)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), e.Message, e);
            }
            
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if(ex != null)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "", ex);
            }
        } 
    }
}
