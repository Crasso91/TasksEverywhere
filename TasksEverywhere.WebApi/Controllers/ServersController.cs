using TasksEverywhere.DataLayer.Context.Concrete;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.HttpUtilities.Enumerators;
using TasksEverywhere.HttpUtils.Attributes;
using TasksEverywhere.HttpUtils.Models.Concrete;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Utilities.Exceptions;
using TasksEverywhere.WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using TasksEverywhere.HttpUtilities.HttpModels.Concrete;
using TasksEverywhere.HttpUtilities.Extensions;
using TasksEverywhere.DataLayer.Enumerators;
using TasksEverywhere.Extensions.Extensions;

namespace TasksEverywhere.WebApi.Controllers
{
    [BasicAuthentication(true)]
    public class ServersController : ApiController
    {
        [HttpPost, ActionName("List"), LogActionFilter]
        public HttpResponseMessage List([FromBody] ApiHttpRequest<Dictionary<string, object>> request)
        {
            //var filtersString = HttpUtility.ParseQueryString(Request.RequestUri.Query)["filters"];
            //var filters = !string.IsNullOrEmpty(filtersString) ? filtersString.Deserialize<Dictionary<string, object>>() : new Dictionary<string, object>();
            var filters = request?.data;
            var response = new ApiHttpResponse<HttpDataList<dynamic>>();
            var username = Request.Username();
            var _context = new QuartzExecutionDataContext();

            try
            {
                var account = _context.Accounts
                    .FirstOrDefault(x => x.Username == username);
                var dbServers = (from ser in _context.Servers
                                 join accser in _context.AccountServers on ser.ServerID equals accser.ServerID
                                 join acc in _context.Accounts on accser.AccountID equals acc.AccountID
                                 where (acc.Username == username && account.Roles == RoleType.Client)
                                        || (account.Roles == RoleType.Admin || account.Roles == RoleType.Inspector)
                                 select new  { AccountID = ser.AccountID, AppId = ser.AppId, IP = ser.IP, Name = ser.Name, ServerID = ser.ServerID, AppToken = ser.AppToken })
                                 .Distinct()
                                .Filter(filters);
                response.data.result = dbServers.ToList<dynamic>();
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
        public HttpResponseMessage Add([FromUri] long accountID, [FromBody]  ApiHttpRequest<Server> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<Server>>();
            var server = request.data;
            var username = Request.Username();

            var _context = new QuartzExecutionDataContext();
            try
            {
                /* only admin and inspector can add clients */
                Action _canProceed = () =>
                {
                    var account = _context.Accounts
                           .SingleOrDefault(x => x.AccountID == accountID);

                    if (account != null)
                    {
                        if (_context.Servers.FirstOrDefault(x => x.Name == server.Name && x.IP == server.IP) == null)
                        {
                            /*_context.Servers.Add(server);*/
                            account.Servers.Add(server);
                            _context.SaveChanges();
                            response.data.result = _context.Servers.FirstOrDefault(x => x.Name == server.Name && x.IP == server.IP);
                            response.Complete(ResultCode.OK, "Server succefully added");
                        }
                        else throw new EntityDuplicationException("Already exists server with name: " + server.Name + " and ip: " + server.IP);
                    }
                    else throw new EntityNotExistsException("No account exists for id: " + accountID);
                };
                Action _else = () => response.Complete(ResultCode.OK, "Only admin and inspector can add servers");
                _context.Accounts
                    .Where(x => x.Username == username)
                    .SingleOrDefault()
                    .ProceedIf(x => x.IsAdminOrIspector, _canProceed, _else);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
                response.Fault(ex);
            }
            return Request.CreateResponse(response.Code, response);
        }
            
        [HttpPost, ActionName("Update"), LogActionFilter]
        public HttpResponseMessage Update([FromUri] long serverID, [FromBody] ApiHttpRequest<Server> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<Server>>();
            var server = request.data;
            var username = Request.Username();

            var _context = new QuartzExecutionDataContext();
            try
            {
                /* only admin and inspector can update clients */
                Action _canProceed = () =>
                {
                    var dbClient = _context.Servers.FirstOrDefault(x => x.ServerID == serverID);
                    if (dbClient != null)
                    {
                        dbClient.Name = server.Name;
                        dbClient.IP = server.IP;
                        dbClient.AppId = string.Empty;
                        dbClient.AppToken = string.Empty;
                        _context.SaveChanges();
                        response.data.result = dbClient;
                        response.Complete(ResultCode.OK, "Server succefully Updated");
                    }
                    else throw new EntityNotExistsException("Not exists server with Name " + server.Name + " and ip " + server.IP);
                };
                Action _else = () => response.Complete(ResultCode.OK, "Only admin and inspector update add clients");
                _context.Accounts
                    .Where(x => x.Username == username)
                    .SingleOrDefault()
                    .ProceedIf(x => x.IsAdminOrIspector, _canProceed, _else);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
                response.Fault(ex);
            }
            return Request.CreateResponse(response.Code, response);
        }

        [HttpPost, ActionName("Delete"), LogActionFilter]
        public HttpResponseMessage Delete([FromBody] ApiHttpRequest<Server> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<bool>>();
            var server = request.data;
            var username = Request.Username();

            var _context = new QuartzExecutionDataContext();
            try
            {
                /* only admin and inspector can delete clients */
                Action _canProceed = () => 
                {
                    var dbClient = _context.Servers.FirstOrDefault(x => x.Name == server.Name && x.IP == server.IP);
                    if (dbClient != null)
                    {
                        _context.Servers.Remove(dbClient);
                        _context.SaveChanges();
                        response.data.result = true;
                        response.Complete(ResultCode.OK, "Client succefully Deleted");
                    }
                    else throw new EntityNotExistsException("Not exists Client with Name " + server.Name + " and ip " + server.IP);
                };
                Action _else = () => response.Complete(ResultCode.OK, "Only admin and inspector can delete clients");
                _context.Accounts
                    .Where(x => x.Username == username)
                    .SingleOrDefault()
                    .ProceedIf(x => x.IsAdminOrIspector, _canProceed, _else);
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

        [HttpGet, ActionName("AppID"), LogActionFilter]
        public HttpResponseMessage GenerateAppID([FromUri] long serverID)
        {
            var response = new ApiHttpResponse<HttpDataSingle<string>>();
            var username = Request.Username();
            var _context = new QuartzExecutionDataContext();
            try
            {
                /* only admin and inspector can generate app id */
                Action _canProceed = () =>
                {
                    var appIdTemplate = "{0}-{1}-{2}";
                    var dbClient = _context.Servers.FirstOrDefault(x => x.ServerID == serverID);
                    if (dbClient == null)
                        throw new EntityNotExistsException("Not exists Client with id: " + serverID);
                    var appId = string.Format(appIdTemplate, dbClient.Name, dbClient.IP, Guid.NewGuid()).Replace(":", "");
                    dbClient.AppId = appId;
                    _context.SaveChanges();
                    response.data.result = appId;
                    response.Complete(ResultCode.OK, "Application Id Generated");
                };
                Action _else = () => response.Complete(ResultCode.OK, "Only admin and inspector can generate app id");
                _context.Accounts
                    .Where(x => x.Username == username)
                    .SingleOrDefault()
                    .ProceedIf(x => x.IsAdminOrIspector, _canProceed, _else);
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

        [HttpGet, ActionName("AppToken"), LogActionFilter]
        public HttpResponseMessage GenerateAppToken([FromUri] long serverID)
        {
            var response = new ApiHttpResponse<HttpDataSingle<string>>();
            var username = Request.Username();
            var _context = new QuartzExecutionDataContext();

            try
            {
                /* only admin and inspector can generate app token */
                Action _canProceed = () => {
                    var appIdTemplate = "{0}-{1}-{2}-{3}";
                    var dbClient = _context.Servers.FirstOrDefault(x => x.ServerID == serverID);
                    if (dbClient == null)
                        throw new EntityNotExistsException("Not exists Client with id: " + serverID);
                    var appToken = string.Format(appIdTemplate, dbClient.Name, dbClient.IP, dbClient.AppId, Guid.NewGuid()).Replace(":", "");
                    var appTokenBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(appToken));
                    dbClient.AppToken = appTokenBase64;
                    _context.SaveChanges();
                    response.data.result = appTokenBase64;
                    response.Complete(ResultCode.OK, "Application Token Generated");
                };
                
                Action _else = () => response.Complete(ResultCode.OK, "Only admin and inspector can generate app token");

                _context.Accounts
                    .Where(x => x.Username == username)
                    .SingleOrDefault()
                    .ProceedIf(x=> x.IsAdminOrIspector, _canProceed, _else);
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