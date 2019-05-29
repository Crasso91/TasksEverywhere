using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.HttpUtilities.HttpModels.Concrete;
using TasksEverywhere.HttpUtilities.Service.Concrete;
using TasksEverywhere.HttpUtils.Models.Concrete;
using TasksEverywhere.Logging.Services.Abstract;
using TasksEverywhere.Utilities.Exceptions;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Logging.Services.Concrete
{
    public class WebApiLogger : BaseLoggingService<WebApiLogger>, IWebApiLogger
    {
        private string Endpoint { get; set; }
        private string AppId { get; set; }
        private string AppToken { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }

        public WebApiLogger()
        {
            this.Configure();
        }

        public override void Configure()
        {
            try
            {
                base.Configure();
                Endpoint = parameters["Endpoint"].Value;
                AppId = parameters["AppId"].Value;
                AppToken = parameters["AppToken"].Value;
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
            }
        }

        public void AddJob(IJobDetail jobDetail)
        {
            try
            {
                var httpService = CastleWindsorService.Resolve<HttpComunicationService>();
                var job = new Job
                {
                    Key = GetJobKey(jobDetail),
                    Executing = false,
                    StartedAt = DateTime.Parse("01/01/1753"),
                    Description = jobDetail.Description
                };
                var req = new ApiHttpRequest<Job>
                {
                    AppID = AppId,
                    AppToken = AppToken,
                    data = job
                };
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Calling API: " + Endpoint + "/Jobs/Add with body: " + req.Stringify());
                var resp = httpService.Post<ApiHttpResponse<HttpDataSingle<long>>>(Endpoint + "/Jobs/Add", AppId, AppToken, false, req);
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "response " + resp.Stringify());
                if (resp.Code != System.Net.HttpStatusCode.OK)
                    throw new HttpCommunicationException(resp.operationResult.Code + " : " + resp.operationResult.Message + "  -> " + resp.operationResult.Stack);
                else LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(),
                    "Http Comunication 'Job Add' for job" + jobDetail.Key.Name + " to Endpoint : " + Endpoint + " exit with code " + resp.Code.ToString() + " returns message " + resp.operationResult.Message);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
            }
        }

        public void CallStart(IJobExecutionContext jobContext)
        {
            try
            {
                var httpService = CastleWindsorService.Resolve<HttpComunicationService>();

                var call = new Call
                {
                    FireInstenceID = jobContext.FireInstanceId,
                    StartedAt = jobContext.FireTimeUtc.DateTime.ToLocalTime(),
                    NextStart = jobContext.NextFireTimeUtc.HasValue ? jobContext.NextFireTimeUtc.Value.DateTime.ToLocalTime() : DateTime.Parse("01/01/1753"),
                    EndedAt = DateTime.Parse("01/01/1753")
                };

                var req = new ApiHttpRequest<Call>
                {
                    AppID = AppId,
                    AppToken = AppToken,
                    data = call
                };
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Calling API: " + Endpoint + "/Calls/End with body: " + req.Stringify());
                var resp = httpService.Post<ApiHttpResponse<HttpDataSingle<bool>>>(Endpoint + "/Calls/Start?jobKey=" + GetJobKey(jobContext.JobDetail), AppId, AppToken, false, req);
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "response " + resp.Stringify());
                if (resp.Code != System.Net.HttpStatusCode.OK)
                    throw new HttpCommunicationException(resp.operationResult.Code + " : " + resp.operationResult.Message + "  -> " + resp.operationResult.Stack);
                else LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(),
                    "Http Comunication 'Call Started' for job" + jobContext.JobDetail.Key.Name + " to Endpoint : " + Endpoint + " exit with code " + resp.Code.ToString() + " returns message " + resp.operationResult.Message);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
            }
        }


        public void CallEnd(IJobExecutionContext jobContext)
        {
            try
            {
                var httpService = CastleWindsorService.Resolve<HttpComunicationService>();

                var call = new Call
                {
                    FireInstenceID = jobContext.FireInstanceId,
                    EndedAt = DateTime.Now
                };


                var req = new ApiHttpRequest<Call>
                {
                    AppID = AppId,
                    AppToken = AppToken,
                    data = call
                };
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Calling API: " + Endpoint + "/Calls/End with body: " + req.Stringify());
                var resp = httpService.Post<ApiHttpResponse<HttpDataSingle<bool>>>(Endpoint + "/Calls/End?jobKey=" + GetJobKey(jobContext.JobDetail), AppId, AppToken, false, req);
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "response " + resp.Stringify());
                if (resp.Code != System.Net.HttpStatusCode.OK)
                    throw new HttpCommunicationException(resp.operationResult.Code + " : " + resp.operationResult.Message + "  -> " + resp.operationResult.Stack);
                else LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(),
                    "Http Comunication 'Call Ended' for job" + jobContext.JobDetail.Key.Name + " to Endpoint : " + Endpoint + " exit with code " + resp.Code.ToString() + " returns message " + resp.operationResult.Message);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
            }
        }

        public void Error(IJobExecutionContext jobContext, Exception ex)
        {
            try
            {
                var httpService = CastleWindsorService.Resolve<HttpComunicationService>();

                var err = new Error
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                var call = new Call
                {
                    FireInstenceID = jobContext.FireInstanceId,
                    EndedAt = DateTime.Now,
                    Error = err
                };


                var req = new ApiHttpRequest<Call>
                {
                    AppID = AppId,
                    AppToken = AppToken,
                    data = call
                };
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Calling API: " + Endpoint + "/Calls/End with body: " + req.Stringify());
                var resp = httpService.Post<ApiHttpResponse<HttpDataSingle<bool>>>(Endpoint + "/Calls/End?jobKey=" + GetJobKey(jobContext.JobDetail), AppId, AppToken, false, req);
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "response " + resp.Stringify());
                if (resp.Code != System.Net.HttpStatusCode.OK)
                    throw new HttpCommunicationException(resp.operationResult.Code + " : " + resp.operationResult.Message + "  -> " + resp.operationResult.Stack);
                else LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(),
                    "Http Comunication 'Call Ended with Error' for job" + jobContext.JobDetail.Key.Name + " to Endpoint : " + Endpoint + " exit with code " + resp.Code.ToString() + " returns message " + resp.operationResult.Message);
            }
            catch (Exception ex1)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex1);
            }
        }

        private string GetJobKey(IJobDetail jobDetail)
        {
            return AppId.Split('-')[0] + "." + jobDetail.Key.Group + "." + jobDetail.Key.Name;
        }
    }
}
