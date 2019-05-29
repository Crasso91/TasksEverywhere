using TasksEverywhere.Api.Dashboard.Exceptions;
using TasksEverywhere.Api.Dashboard.Extensions;
using TasksEverywhere.Api.Dashboard.Models;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.Extensions;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.Extensions.Models;
using TasksEverywhere.HttpUtilities.HttpModels.Concrete;
using TasksEverywhere.HttpUtilities.Service.Concrete;
using TasksEverywhere.HttpUtils.Models.Concrete;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace TasksEverywhere.Api.Dashboard.Controllers
{
    public class StatsController : ApiController
    {
        private string webApiUrl = ConfigurationManager.AppSettings["ApiEndpoint"];

        [HttpGet, ActionName("Last20Call")]
        public Models.Last20CallResponse Last20Call([FromUri] string sessionKey, string accountID)
        {
            Last20CallResponse response = new Last20CallResponse();
            var httpService = new HttpComunicationService();
            if (HttpContext.Current.ExistsSessionKey((string)sessionKey, (string)accountID) &&
                    HttpContext.Current.IsSessionKeyValid((string)sessionKey, (string)accountID))
            {
                var account = HttpContext.Current.GetAccount((string)sessionKey).Deserialize<Account>();

                var _req = new ApiHttpRequest<DbFiltersCollection>
                {
                    data = new DbFiltersCollection
                    {
                        new DbTop { Number = 10 },
                        new DbFilter { PropertyName = "StartedAt", Sign = CompareSign.GreaterEquals, Value = DateTime.Now.SetMidnight()}
                    }
                };

                var _resCalls = httpService
                    .Post<ApiHttpResponse<HttpDataList<Call>>>(webApiUrl + "calls/List",
                    account.Username,
                    account.Password,
                    false,
                    _req);

                if (_resCalls.operationResult.Code == HttpUtilities.Enumerators.ResultCode.KO)
                {
                    throw new AngularException(_resCalls.operationResult.Message, _resCalls.operationResult.Stack);
                }

                _req = new ApiHttpRequest<DbFiltersCollection>
                {
                    data = new DbFiltersCollection
                    {
                        new DbTop { Number = 10 },
                        new DbFilter { PropertyName = "StartedAt", Sign = CompareSign.GreaterEquals, Value = DateTime.Now.SetMidnight()},
                        new DbFilter { PropertyName = "Error", Sign = CompareSign.NotEquals, Value = null}
                    }
                };

                var _errorCalls = httpService
                    .Post<ApiHttpResponse<HttpDataList<Call>>>(webApiUrl + "calls/List",
                    account.Username,
                    account.Password,
                    false,
                    _req);

                if (_resCalls.operationResult.Code == HttpUtilities.Enumerators.ResultCode.KO)
                {
                    throw new AngularException(_resCalls.operationResult.Message, _resCalls.operationResult.Stack);
                }

                response.Last10Calls = _resCalls.data.result.OrderByDescending(x => x.StartedAt);//.Take(10).ToList();
                response.Last10ErrorsCalls = _resCalls.data.result.OrderByDescending(x => x.StartedAt);//.Where(x => x.Error != null).Take(10).ToList();

                return response;
            }
            else return null;
        }

        [HttpGet, ActionName("CallPerHour")]
        public CallPerHourResponse CallPerHour([FromUri] string sessionKey, [FromUri] string accountID)
        {
            var response = new CallPerHourResponse();
            var httpService = new HttpComunicationService();
            if(HttpContext.Current.ExistsSessionKey(sessionKey, accountID) &&
                    HttpContext.Current.IsSessionKeyValid(sessionKey, accountID))
            {
                var account = HttpContext.Current.GetAccount(sessionKey).Deserialize<Account>();
                var _req = new ApiHttpRequest<Dictionary<string, object>>
                {
                    data = new Dictionary<string, object> ()
                };

                var _resCalls = httpService
                    .Post<ApiHttpResponse<HttpDataList<Call>>>(webApiUrl + "calls/List",
                    account.Username,
                    account.Password,
                    false,
                    _req);

                if (_resCalls.operationResult.Code == HttpUtilities.Enumerators.ResultCode.KO)
                {
                    throw new AngularException(_resCalls.operationResult.Message, _resCalls.operationResult.Stack);
                }
                response.data = (from call in _resCalls.data.result
                                 where call.StartedAt >= DateTime.Today
                                 group call by call.StartedAt.Hour into cals
                                 select new CallPerHour
                                 {
                                     calls = cals.Count(),
                                     hour = cals.Key
                                 }).ToList();

                return response;
            }
            else return null;
        }

        [HttpGet, ActionName("ErrorPerHour")]
        public CallPerHourResponse ErrorPerHour([FromUri] string sessionKey, [FromUri] string accountID)
        {
            var response = new CallPerHourResponse();
            var httpService = new HttpComunicationService();
            if (HttpContext.Current.ExistsSessionKey(sessionKey, accountID) &&
                    HttpContext.Current.IsSessionKeyValid(sessionKey, accountID))
            {
                var account = HttpContext.Current.GetAccount(sessionKey).Deserialize<Account>();
                var _req = new ApiHttpRequest<Dictionary<string, object>>
                {
                    data = new Dictionary<string, object>()
                };

                var _resCalls = httpService
                    .Post<ApiHttpResponse<HttpDataList<Call>>>(webApiUrl + "calls/List",
                    account.Username,
                    account.Password,
                    false,
                    _req);

                if (_resCalls.operationResult.Code == HttpUtilities.Enumerators.ResultCode.KO)
                {
                    throw new AngularException(_resCalls.operationResult.Message, _resCalls.operationResult.Stack);
                }
                response.data = (from call in _resCalls.data.result
                                 where call.Error != null
                                 && call.StartedAt >= DateTime.Today
                                 group call by call.StartedAt.Hour into cals
                                 select new CallPerHour
                                 {
                                     calls = cals.Count(),
                                     hour = cals.Key
                                 }).ToList();

                return response;
            }
            else return null;
        }
    }
}