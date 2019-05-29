using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.DataLayer.Context.Concrete;
using TasksEverywhere.DataLayer.Enumerators;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.Extensions.Models;
using TasksEverywhere.HttpUtilities.Enumerators;
using TasksEverywhere.HttpUtilities.Extensions;
using TasksEverywhere.HttpUtilities.HttpModel;
using TasksEverywhere.HttpUtilities.HttpModels.Concrete;
using TasksEverywhere.HttpUtilities.Service.Concrete;
using TasksEverywhere.HttpUtilities.Services.Concrete;
using TasksEverywhere.HttpUtils.Attributes;
using TasksEverywhere.HttpUtils.Models.Concrete;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Utilities.Exceptions;
using TasksEverywhere.Utilities.Services;
using TasksEverywhere.WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace TasksEverywhere.WebApi.Controllers
{
    [BasicAuthentication(true)]
    public class CallsController : ApiController
    {
        [HttpPost, ActionName("List"), LogActionFilter]
        public HttpResponseMessage CallsList([FromBody] ApiHttpRequest<DbFiltersCollection> request)
        {
            //var filtersString = HttpUtility.ParseQueryString(Request.RequestUri.Query)["filters"];
            //var filters = !string.IsNullOrEmpty(filtersString) ? filtersString.Deserialize<Dictionary<string, object>>() : new Dictionary<string, object>();
            var filters = request.data;
            var response = new ApiHttpResponse<HttpDataList<Call>>();
            var username = Request.Username();
            var _context = new QuartzExecutionDataContext();
            try
            {
                var account = _context.Accounts
                      .FirstOrDefault(x => x.Username == username);
                var dbCalls = (from ser in _context.Servers
                               join job in _context.Jobs on ser.ServerID equals job.ServerID
                               join cal in _context.Calls on job.JobID equals cal.JobID
                               join accser in _context.AccountServers on ser.ServerID equals accser.ServerID
                               join acc in _context.Accounts on accser.AccountID equals acc.AccountID
                               where (acc.Username == username && account.Roles == RoleType.Client)
                                      || (account.Roles == RoleType.Admin || account.Roles == RoleType.Inspector)
                               select cal)
                               .Distinct()
                              .Filter(filters);
                response.data.result = dbCalls.ToList();
                response.Complete(ResultCode.OK, "");
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
                response.Fault(ex);
            }
            return Request.CreateResponse(response.Code, response);

        }

        [HttpPost, ActionName("Start"), LogActionFilter]
        public HttpResponseMessage CallStart([FromUri] string jobKey, [FromBody] ApiHttpRequest<Call> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<bool>>();
            var call = request.data;
            var _context = new QuartzExecutionDataContext();
            try
            {
                var appID = Request.Username();
                var appToken = Request.Password();
                var dbServer = _context.Servers
                    .FirstOrDefault(x => x.AppId == appID && x.AppToken == appToken);

                if (dbServer != null)
                {

                    var dbJob = dbServer.Jobs.FirstOrDefault(x => x.Key == jobKey);
                    if (dbJob != null)
                    {
                        dbJob.Calls.Add(call);
                        dbJob.Executing = true;
                        _context.SaveChanges();
                        response.data.result = true;
                        response.Complete(ResultCode.OK, "");

                        try
                        {
                            WebSocketClient.SendMessage(new SocketMessage
                            {
                                Title = "Job " + call.Job?.Key + " in esecuzione con instance ID : " + call.FireInstenceID,
                                DataType = call.GetType(),
                                IsImportant = false,
                                Data = call

                            });
                        }
                        catch (Exception ex) { LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex); }
                    }
                    else throw new EntityNotExistsException("Job with key " + jobKey + " not exists");
                }
                else throw new EntityNotExistsException("Server with key " + Request.Username() + " not exists");
            }
            catch (EntityNotExistsException ex)
            {
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message);
                response.Warning(System.Net.HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
                response.Fault(ex);
            }
            return Request.CreateResponse(response.Code, response);
        }

        [HttpPost, ActionName("End"), LogActionFilter]
        public HttpResponseMessage CallEnd([FromUri] string jobKey, [FromBody] ApiHttpRequest<Call> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<bool>>();
            var call = request.data;
            var _context = new QuartzExecutionDataContext();
            try
            {
                var appID = Request.Username();
                var appToken = Request.Password();
                var dbServer = _context.Servers
                    .FirstOrDefault(x => x.AppId == appID && x.AppToken == appToken);

                if (dbServer != null)
                {
                    var dbCall = (from job in dbServer.Jobs
                                    join cal in _context.Calls on job.JobID equals cal.JobID
                                    where job.Key == jobKey
                                    && cal.FireInstenceID == call.FireInstenceID
                                    select cal
                                    ).FirstOrDefault();
                    if (dbCall != null)
                    {
                        dbCall.EndedAt = call.EndedAt;
                        dbCall.Error = call.Error;
                        dbCall.Job.Executing = false;
                        _context.SaveChanges();
                        response.data.result = true;
                        response.Complete(ResultCode.OK, "");
                        try
                        {
                            WebSocketClient.SendMessage(new SocketMessage
                            {
                                Title = "Job " + dbCall.Job?.Key + " terminato con instance ID : " + call.FireInstenceID,
                                DataType = call.GetType(),
                                IsImportant = call.Error != null,
                                Data = call

                            });
                        }
                        catch (Exception ex) { LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex); }
                    }
                    else throw new EntityNotExistsException("Call with fire instance id " + call.FireInstenceID + " not exists");
                }
                else throw new EntityNotExistsException("Client with key " + Request.Username() + " not exists");

            }
            catch (EntityNotExistsException ex)
            {
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message);
                response.Warning(System.Net.HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
                response.Fault(ex);
            }
            return Request.CreateResponse(response.Code, response);
        }
    }
}