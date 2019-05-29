using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Quartz.Services;
using TasksEverywhere.Utilities.Services;
using Microsoft.Owin.Hosting;
using Owin;

namespace TasksEverywhere.WindwsService
{
    public class ServiceRunner
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
        }

        public static CancellationTokenSource Start(string address, int port)
        {
            var uri = "{0}:{1}";
            uri = address.Contains("http") ? string.Format(uri, address, port) : string.Format("http://" + uri, address, port);

            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            return TaskManager.RunTask((ts) => {

                try
                {
                    WebApp.Start<ServiceRunner>(uri);
                }
                catch (Exception ex)
                {
                    LogsAppendersManager.Instance.Error(typeof(ServiceRunner), MethodBase.GetCurrentMethod(), "", ex);
                }
            });

        }
    }
}
