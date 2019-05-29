using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ICeScheduler.Dashboard.Extensions;
using ICeScheduler.DataLayer.Models;
using ICeScheduler.HttpUtilities.Service.Concrete;
using ICeScheduler.HttpUtils.Models.Concrete;
using Microsoft.AspNetCore.Mvc;
using ICeScheduler.Extensions.Extensions;
using ICeScheduler.HttpUtilities.HttpModels.Concrete;
using ICeScheduler.Dashboard.Exceptions;

namespace ICeScheduler.Dashboard.Controllers
{
    [Route("api/[controller]")]
    public class JobsController : Controller
    {
        private string webApiUrl = "http://localhost:65269/api/";

        [HttpGet("[action]")]
        public IEnumerable<Job> List([FromBody] dynamic request)
        {
            var httpService = new HttpComunicationService();
            if (HttpContext.ExistsSessionKey(request.sessinKey as string) &&
                    HttpContext.IsSessionKeyValid(request.sessinKey as string))
            {
                var account = HttpContext.GetAccount(request.sessinKey as string).Deserialize<Account>();

                var _req = new ApiHttpRequest<Dictionary<string, object>>
                {
                    data = request.data as Dictionary<string, object>
                };
                var response = httpService
                    .Post<ApiHttpResponse<HttpDataList<Job>>>(webApiUrl + "Jobs/List",
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
