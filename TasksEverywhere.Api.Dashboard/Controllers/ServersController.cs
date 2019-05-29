using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.HttpUtilities.HttpModels.Concrete;
using TasksEverywhere.HttpUtilities.Service.Concrete;
using TasksEverywhere.HttpUtils.Models.Concrete;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.Api.Dashboard.Extensions;
using TasksEverywhere.Api.Dashboard.Exceptions;
using TasksEverywhere.HttpUtils.Entities;
using System.Web.Http;
using System.Web;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace TasksEverywhere.Dashboard.Controllers
{
    public class ServersController : ApiController
    {
        private string webApiUrl = ConfigurationManager.AppSettings["ApiEndpoint"];

        [HttpPost, ActionName("List")]
        public IEnumerable<Server> List([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.Current.ExistsSessionKey((string) request.sessionKey, (string) request.accountID) &&
                    HttpContext.Current.IsSessionKeyValid((string) request.sessionKey, (string)request.accountID))
            {
                var account = HttpContext.Current.GetAccount((string)request.sessionKey).Deserialize<Account>();

                var _req = new ApiHttpRequest<Dictionary<string, object>>
                {
                    data = ((JObject)request.data) != null ? ((JObject)request.data).ToObject<Dictionary<string, object>>() :  new Dictionary<string, object>()
                };
                var response = httpService
                    .Post<ApiHttpResponse<HttpDataList<Server>>>(webApiUrl + "Servers/List",
                    account.Username,
                    account.Password,
                    false,
                    _req);
                
                if(response.operationResult.Code == HttpUtilities.Enumerators.ResultCode.KO)
                {
                    throw new AngularException(response.operationResult.Message, response.operationResult.Stack);
                }
                return response.data.result;
            }
            else return null;
        }
        
        [HttpPost, ActionName("Add")]
        public OperationResult Add([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.Current.ExistsSessionKey((string) request.sessionKey, (string)request.accountID) &&
                    HttpContext.Current.IsSessionKeyValid((string) request.sessionKey, (string)request.accountID))
            {
                var account = HttpContext.Current.GetAccount((string) request.sessionKey as string).Deserialize<Account>();

                var _req = new ApiHttpRequest<Server>
                {
                    data = request.data as Server
                };
                var response = httpService
                    .Post<ApiHttpResponse<HttpDataList<Server>>>(webApiUrl + "Servers/Add",
                    account.Username,
                    account.Password,
                    false,
                    _req);
                
                return response.operationResult;
            }
            else return null;
        }

        [HttpPost, ActionName("Update")]
        public OperationResult Update([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.Current.ExistsSessionKey((string) request.sessionKey, (string)request.accountID) &&
                    HttpContext.Current.IsSessionKeyValid((string) request.sessionKey, (string)request.accountID))
            {
                var account = HttpContext.Current.GetAccount((string) request.sessionKey as string).Deserialize<Account>();

                var _req = new ApiHttpRequest<Server>
                {
                    data = request.data as Server
                };
                var response = httpService
                    .Post<ApiHttpResponse<HttpDataList<Server>>>(webApiUrl + "Servers/Update?ServerID=",
                    account.Username,
                    account.Password,
                    false,
                    _req);
                return response.operationResult;
            }
            else return null;
        }

        [HttpPost, ActionName("Delete")]
        public OperationResult Delete([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.Current.ExistsSessionKey((string) request.sessionKey, (string)request.accountID) &&
                    HttpContext.Current.IsSessionKeyValid((string) request.sessionKey, (string)request.accountID))
            {
                var account = HttpContext.Current.GetAccount((string) request.sessionKey as string).Deserialize<Account>();

                var _req = new ApiHttpRequest<Server>
                {
                    data = request.data as Server
                };
                var response = httpService
                    .Post<ApiHttpResponse<HttpDataList<Server>>>(webApiUrl + "Servers/Delete?ServerID=",
                    account.Username,
                    account.Password,
                    false,
                    _req);
                
                return response.operationResult;
            }
            else return null;
        }

        [HttpPost, ActionName("AppId")]
        public OperationResult AppId([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.Current.ExistsSessionKey((string) request.sessionKey, (string)request.accountID) &&
                    HttpContext.Current.IsSessionKeyValid((string) request.sessionKey, (string)request.accountID))
            {
                var account = HttpContext.Current.GetAccount((string) request.sessionKey as string).Deserialize<Account>();

                var _req = new ApiHttpRequest<Server>
                {
                    data = request.data as Server
                };
                var response = httpService
                    .Get<ApiHttpResponse<HttpDataList<Server>>>(webApiUrl + "Servers/Delete?ServerID=" + request.data as string,
                    account.Username,
                    account.Password,
                    false);

                return response.operationResult;
            }
            else return null;
        }
        [HttpPost, ActionName("appToken")]
        public OperationResult appToken([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.Current.ExistsSessionKey((string) request.sessionKey, (string)request.accountID) &&
                    HttpContext.Current.IsSessionKeyValid((string) request.sessionKey, (string)request.accountID))
            {
                var account = HttpContext.Current.GetAccount((string) request.sessionKey as string).Deserialize<Account>();

                var response = httpService
                    .Get<ApiHttpResponse<HttpDataList<Server>>>(webApiUrl + "Servers/Delete?ServerID=" + request.data as string,
                    account.Username,
                    account.Password,
                    false);
                
                return response.operationResult;
            }
            else return null;
        }
    }
}
