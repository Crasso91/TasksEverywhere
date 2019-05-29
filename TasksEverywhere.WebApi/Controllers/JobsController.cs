using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.DataLayer.Context.Concrete;
using TasksEverywhere.DataLayer.Enumerators;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.HttpUtilities.Enumerators;
using TasksEverywhere.HttpUtilities.Extensions;
using TasksEverywhere.HttpUtilities.HttpModel;
using TasksEverywhere.HttpUtilities.HttpModels.Concrete;
using TasksEverywhere.HttpUtilities.Services.Concrete;
using TasksEverywhere.HttpUtils.Attributes;
using TasksEverywhere.HttpUtils.Models.Concrete;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Utilities.Exceptions;
using TasksEverywhere.Utilities.Services;
using TasksEverywhere.WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace TasksEverywhere.WebApi.Controllers
{
    [BasicAuthentication(true)]
    public class JobsController : ApiController
    {
        [HttpPost, ActionName("List"), LogActionFilter]
        public HttpResponseMessage JobsList([FromBody] ApiHttpRequest<Dictionary<string, object>> request)
        {
            //var filtersString = HttpUtility.ParseQueryString(Request.RequestUri.Query)["filters"];
            //var filters = !string.IsNullOrEmpty(filtersString) ? filtersString.Deserialize<Dictionary<string, object>>() : new Dictionary<string, object>();
            var filters = request.data;

            var response = new ApiHttpResponse<HttpDataList<dynamic>>();
            var username = Request.Username();
            var _context = new QuartzExecutionDataContext();
            try
            {
                var account = _context.Accounts
                    .FirstOrDefault(x => x.Username == username);
                var jobs = (from ser in _context.Servers
                            join job in _context.Jobs on ser.ServerID equals job.ServerID
                            join accser in _context.AccountServers on ser.ServerID equals accser.ServerID
                            join acc in _context.Accounts on accser.AccountID equals acc.AccountID
                            where (acc.Username == username && account.Roles == RoleType.Client)
                                   || (account.Roles == RoleType.Admin || account.Roles == RoleType.Inspector)
                            select new {
                                JobID = job.JobID,
                                Key = job.Key,
                                Description = job.Description,
                                Executing = job.Executing,
                                ServerID = job.ServerID,
                                StartedAt = job.StartedAt,
                                IsExecutingCorrectly = DateTime.Now <= DbFunctions.AddMinutes(job.Calls.OrderByDescending(x => x.EndedAt).FirstOrDefault().NextStart, 1),
                                LastCall = job.Calls.OrderByDescending(x=>x.EndedAt).FirstOrDefault()
                            })
                            .Distinct()
                            .Filter(filters)
                            .OrderByDescending(x=>x.LastCall.StartedAt);


                //var jobList = jobs?
                //    .OrderByDescending(x => x.LastCall.StartedAt);
                //jobList.ForEach(x => x.Calls.Take(1));
                response.data.result = jobs.ToList<dynamic>();
                response.Complete(ResultCode.OK, "");
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
                response.Fault(ex);
            }
            return Request.CreateResponse(response.Code, response);
        }

        [HttpPost, ActionName("Add"), LogActionFilter]
        public HttpResponseMessage AddJob([FromBody] ApiHttpRequest<Job> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<long>>();
            var job = request.data;
            var _context = new QuartzExecutionDataContext();
            try
            {
                var appID = Request.Username();
                var appToken = Request.Password();
                var dbServer = _context.Servers
                               .FirstOrDefault(x=>x.AppId == appID && x.AppToken == appToken);

                if (dbServer != null)
                {
                    if (dbServer.Jobs.FirstOrDefault(x => x.Key.ToLower() == job.Key.ToLower()) == null)
                    {
                        job.Server = dbServer;
                        job.ServerID = dbServer.ServerID;
                        _context.Jobs.Add(job);
                        _context.SaveChanges();
                        response.Complete(ResultCode.OK, "");

                        try
                        {
                            WebSocketClient.SendMessage(new SocketMessage
                            {
                                Title = "Nuovo Job " + job.Key + " per server " + job.Server?.Name,
                                DataType = job.GetType(),
                                IsImportant = false,
                                Data = job

                            });
                        }
                        catch (Exception ex) { LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex); }
                    }
                    else throw new EntityDuplicationException("Job alredy exists with key " + job.Key);
                    response.data.result = dbServer.Jobs.FirstOrDefault(x => x.Key == job.Key).JobID;
                }
                else throw new EntityNotExistsException("Client with key " + Request.Username() + " not exists");

            }
            catch (EntityDuplicationException ex)
            {
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message);
                response.Complete(ResultCode.OK, ex.Message);
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

        [HttpPost, ActionName("Update"), LogActionFilter]
        public HttpResponseMessage UpdateJob([FromBody] ApiHttpRequest<Job> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<Job>>();
            var job = request.data;
            var username = Request.Username();
            var _context = new QuartzExecutionDataContext();
            try
            {
                var appID = Request.Username();
                var appToken = Request.Password();
                var dbServer = _context.Servers
                               .FirstOrDefault(x => x.AppId == appID && x.AppToken == appToken);

                if (dbServer != null)
                {
                    var dbJob = dbServer.Jobs.FirstOrDefault(x => x.Key == job.Key);
                    if (dbJob != null)
                    {
                        dbJob.StartedAt = job.StartedAt;
                        dbJob.Executing = job.Executing;
                        _context.SaveChanges();
                        response.Complete(ResultCode.OK, "");
                    }
                    else throw new EntityNotExistsException(job.Key);
                    response.data.result = dbServer.Jobs.FirstOrDefault(x => x.Key == job.Key);
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

        [HttpPost, ActionName("Delete"), LogActionFilter]
        public HttpResponseMessage DeleteJob([FromBody] ApiHttpRequest<Job> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<bool>>();
            var job = request.data;
            var _context = new QuartzExecutionDataContext();
            try
            {
                var appID = Request.Username();
                var appToken = Request.Password();
                var dbServer = _context.Servers
                               .FirstOrDefault(x => x.AppId == appID && x.AppToken == appToken);
                if (dbServer != null)
                {
                    var dbJob = dbServer.Jobs.FirstOrDefault(x => x.Key == job.Key);
                    if (dbJob != null)
                    {
                        _context.Jobs.Remove(dbJob);
                        _context.SaveChanges();
                        response.data.result = true;
                        response.Complete(ResultCode.OK, "");
                    }
                    else throw new EntityNotExistsException("Entity not found for job key : " + job.Key);
                }
                else throw new EntityNotExistsException("Client with key " + Request.Username() + " not exists");

            }
            catch (EntityNotExistsException ex)
            {
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message);
                response.Warning(System.Net.HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex1)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex1.Message, ex1);
                response.Fault(ex1);
            }
            return Request.CreateResponse(response.Code, response);
        }
    }
}