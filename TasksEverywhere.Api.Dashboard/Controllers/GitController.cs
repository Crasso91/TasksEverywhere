using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TasksEverywhere.Api.Dashboard.Extensions;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.HttpUtilities.Service.Concrete;
using TasksEverywhere.HttpUtils.Models.Concrete;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.HttpUtilities.HttpModels.Concrete;
using TasksEverywhere.Api.Dashboard.Exceptions;
using System.Web.Http;
using System.Configuration;
using Newtonsoft.Json.Linq;
using TasksEverywhere.Api.Dashboard.Services;
using TasksEverywhere.Api.Dashboard.Models;
using TasksEverywhere.Logging.Services.Concrete;
using System.Reflection;
using TasksEverywhere.HttpUtilities.Services.Concrete;

namespace TasksEverywhere.Dashboard.Controllers
{
    public class GitController : ApiController
    {
        private string webApiUrl = ConfigurationManager.AppSettings["ApiEndpoint"];

        [HttpPost, ActionName("payload")]
        public string payload([FromBody]object contents)
        {
            try
            {
                switch (GitWebhookManager.GetGitRequestType(Request))
                {
                    case GitWebhookManager.GitRequestType.Undefined:
                        throw new Exception("Undefined Webhook Type");
                    case GitWebhookManager.GitRequestType.Branch:
                        var branch = contents.ToString().Deserialize<Branch>();
                        var messageBranch = GitWebhookManager.GetMessageFor(branch);
                        WebSocketServer.SendMessage(messageBranch);
                        break;
                    case GitWebhookManager.GitRequestType.Push:
                        var push = contents.ToString().Deserialize<Push>();
                        var messagePush = GitWebhookManager.GetMessageFor(push);
                        WebSocketServer.SendMessage(messagePush);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "", ex);
            }
            return "Received";
        }

    }
}
