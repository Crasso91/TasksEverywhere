using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICeScheduler.DataLayer.Models;
using ICeScheduler.HttpUtilities.HttpModels.Concrete;
using ICeScheduler.HttpUtilities.Service.Concrete;
using ICeScheduler.HttpUtils.Models.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using ICeScheduler.Extensions.Extensions;
using ICeScheduler.Dashboard.Extensions;
using ICeScheduler.Dashboard.Exceptions;
using ICeScheduler.HttpUtils.Entities;

namespace ICeScheduler.Dashboard.Controllers
{
    [Route("api/[controller]")]
    public class ServersController : Controller
    {
        private string webApiUrl = "http://localhost:65269/api/";

        [HttpPost("[action]")]
        public IEnumerable<Server> List([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.ExistsSessionKey(request.sessionKey as string) &&
                    HttpContext.IsSessionKeyValid(request.sessionKey as string))
            {
                var account = HttpContext.GetAccount(request.sessinKey as string).Deserialize<Account>();

                var _req = new ApiHttpRequest<Dictionary<string, object>>
                {
                    data = request.data as Dictionary<string, object>
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
        
        [HttpPost("[action]")]
        public OperationResult Add([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.ExistsSessionKey(request.sessionKey as string) &&
                    HttpContext.IsSessionKeyValid(request.sessionKey as string))
            {
                var account = HttpContext.GetAccount(request.sessinKey as string).Deserialize<Account>();

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

        [HttpPost("[action]")]
        public OperationResult Update([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.ExistsSessionKey(request.sessionKey as string) &&
                    HttpContext.IsSessionKeyValid(request.sessionKey as string))
            {
                var account = HttpContext.GetAccount(request.sessinKey as string).Deserialize<Account>();

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

        [HttpPost("[action]")]
        public OperationResult Delete([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.ExistsSessionKey(request.sessionKey as string) &&
                    HttpContext.IsSessionKeyValid(request.sessionKey as string))
            {
                var account = HttpContext.GetAccount(request.sessinKey as string).Deserialize<Account>();

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

        [HttpPost("[action]")]
        public OperationResult AppId([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.ExistsSessionKey(request.sessionKey as string) &&
                    HttpContext.IsSessionKeyValid(request.sessionKey as string))
            {
                var account = HttpContext.GetAccount(request.sessinKey as string).Deserialize<Account>();

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
        [HttpPost("[action]")]
        public OperationResult appToken([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.ExistsSessionKey(request.sessionKey as string) &&
                    HttpContext.IsSessionKeyValid(request.sessionKey as string))
            {
                var account = HttpContext.GetAccount(request.sessinKey as string).Deserialize<Account>();

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
