using TasksEverywhere.Api.Dashboard.Exceptions;
using TasksEverywhere.Api.Dashboard.Models;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.HttpUtilities.Service.Concrete;
using TasksEverywhere.HttpUtils.Models.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TasksEverywhere.Api.Dashboard.Extensions;
using System.Net.Http;
using System.Configuration;
using TasksEverywhere.HttpUtils.Attributes;
using TasksEverywhere.Logging.Services.Concrete;
using System.Reflection;

namespace TasksEverywhere.Api.Dashboard.Controllers
{
    public class AuthenticationController : ApiController
    {
        private string webApiUrl = ConfigurationManager.AppSettings["ApiEndpoint"];
        [HttpPost, ActionName("Login"), LogActionFilter]
        public HttpResponseMessage Login(Account account)
        {
            var response = new ApiHttpResponse<LoginInfo>();
            try
            {
                var httpService = new HttpComunicationService();
                var wsResponse = httpService.Post<ApiHttpResponse<HttpDataSingle<Account>>>(webApiUrl + "AccountsVerification/exists",
                "",
                "",
                false,
                account
                );
                if (wsResponse.operationResult.Code == HttpUtilities.Enumerators.ResultCode.OK)
                {
                response.data = new LoginInfo
                {
                    Account = wsResponse.data.result,
                    SessionKey = HttpContext.Current.GetSessionKey(wsResponse.data.result)
                };
                response.Complete(HttpUtilities.Enumerators.ResultCode.OK);
                }
                else
                {
                    throw new AngularException(response.operationResult.Message, response.operationResult.Stack);
                }
            }
            catch (Exception ex)
            {
                response.Fault(ex);
            }
            return Request.CreateResponse(response.Code, response);
        }
        [HttpPost, ActionName("Logout"), LogActionFilter]
        public HttpResponseMessage Logout([FromBody] dynamic request)
        {
            var response = new ApiHttpResponse<bool>();
            HttpContext.Current.RemoveSessionKey((string)request.accountID);
            response.data = HttpContext.Current.ExistsSessionKey((string)request.sessionKey, (string)request.accountID);
            response.Complete(HttpUtilities.Enumerators.ResultCode.OK);
            return Request.CreateResponse(response.Code, response);
        }
        [HttpGet,ActionName("IsValid"), LogActionFilter]
        public bool IsValid(string sessionKey, string accountID)
        {
            try
            {
                return HttpContext.Current.IsSessionKeyValid(sessionKey, accountID);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
                return false;
            }
            
        }
    }
}
