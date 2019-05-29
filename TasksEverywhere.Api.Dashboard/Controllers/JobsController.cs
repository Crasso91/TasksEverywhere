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

namespace TasksEverywhere.Dashboard.Controllers
{
    public class JobsController : ApiController
    {
        private string webApiUrl = ConfigurationManager.AppSettings["ApiEndpoint"];

        [HttpPost, ActionName("List")]
        public IEnumerable<dynamic> List([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.Current.ExistsSessionKey((string) request.sessionKey, (string)request.accountID) &&
                    HttpContext.Current.IsSessionKeyValid((string) request.sessionKey, (string)request.accountID))
            {
                var account = HttpContext.Current.GetAccount((string)request.sessionKey).Deserialize<Account>();

                var _req = new ApiHttpRequest<Dictionary<string, object>>
                {
                    data = ((JObject)request.data) != null ? ((JObject)request.data).ToObject<Dictionary<string, object>>() : new Dictionary<string, object>()
                };
                var response = httpService
                    .Post<ApiHttpResponse<HttpDataList<dynamic>>>(webApiUrl + "Jobs/List",
                    account.Username,
                    account.Password,
                    false,
                    _req);

                if (response.operationResult.Code == HttpUtilities.Enumerators.ResultCode.KO)
                {
                    throw new AngularException(response.operationResult.Message, response.operationResult.Stack);
                }
                return response.data.result;
            }
            else return null;
        }
    }
}
