using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ICeScheduler.Dashboard.Exceptions;
using ICeScheduler.Dashboard.Extensions;
using ICeScheduler.DataLayer.Models;
using ICeScheduler.HttpUtilities.HttpModels.Concrete;
using ICeScheduler.HttpUtilities.Service.Concrete;
using ICeScheduler.HttpUtils.Models.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ICeScheduler.Dashboard.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        [ActionName("[action]"), HttpPost]
        public LoginInfo Login([FromBody] Account account)
        {
            var httpService = new HttpComunicationService();
            var response = httpService.Post<ApiHttpResponse<HttpDataSingle<Account>>>("",
                account.Username,
                account.Password,
                false,
                account
            );
            if(response.operationResult.Code == HttpUtilities.Enumerators.ResultCode.OK)
            {
                return new LoginInfo
                {
                    Account = response.data.result,
                    SessionKey = HttpContext.GetSessionKey(response.data.result)
                };
            }
            else
            {
                throw new AngularException(response.operationResult.Message, response.operationResult.Stack);
            }
        }

        [ActionName("[action]"), HttpPost]
        public bool Logout([FromBody] string sessionKey)
        {
             HttpContext.RemoveSessionKey(sessionKey);
            return HttpContext.ExistsSessionKey(sessionKey);
        }
    }
}
