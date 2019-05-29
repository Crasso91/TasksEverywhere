using TasksEverywhere.Logging.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TasksEverywhere.HttpUtils.Attributes
{
    public class LogActionFilter : ActionFilterAttribute, IActionFilter
    {
        bool logAction = ConfigurationManager.AppSettings["LogAction"] == "true";
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (logAction)
            {
                LogsAppendersManager.Instance.Info(actionContext.ControllerContext.ControllerDescriptor.ControllerType,
                    actionContext.ControllerContext.ControllerDescriptor.ControllerType.GetMethods().FirstOrDefault(x => x.Name.Contains(actionContext.ActionDescriptor.ActionName)),
                    "Uri: " + actionContext.Request.RequestUri.AbsoluteUri + "\r\n" +
                    "Body: " + actionContext.Request.Content.ReadAsStringAsync().Result);
            }
            else
                LogsAppendersManager.Instance.Info(actionContext.ControllerContext.ControllerDescriptor.ControllerType,
                    actionContext.ControllerContext.ControllerDescriptor.ControllerType.GetMethods().FirstOrDefault(x => x.Name.Contains(actionContext.ActionDescriptor.ActionName)), "request received");
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (logAction)
            {
                var logAction = ConfigurationManager.AppSettings["LogAction"] = "true";
                LogsAppendersManager.Instance.Info(actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType,
                    actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.GetMethods().FirstOrDefault(x => x.Name.Contains(actionExecutedContext.ActionContext.ActionDescriptor.ActionName)),
                    "Response: " + actionExecutedContext.Response.Content.ReadAsStringAsync().Result);
            }
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
