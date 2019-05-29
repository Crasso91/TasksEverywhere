using TasksEverywhere.DataLayer.Context.Concrete;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.HttpUtils.Models.Concrete;
using TasksEverywhere.WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TasksEverywhere.Utilities.Exceptions;
using TasksEverywhere.Logging.Services.Concrete;
using System.Reflection;
using TasksEverywhere.HttpUtilities.Enumerators;
using TasksEverywhere.HttpUtilities.HttpModels.Concrete;
using System.Text;
using TasksEverywhere.HttpUtils.Attributes;
using TasksEverywhere.HttpUtilities.Services.Concrete;
using TasksEverywhere.HttpUtilities.HttpModel;

namespace TasksEverywhere.WebApi.Controllers
{
    public class ServerGeneratorController : ApiController
    {
        [HttpGet, ActionName("Create"), LogActionFilter]
        public HttpResponseMessage Create([FromUri] string username, [FromUri] string password, [FromUri] string serverName)
        {
            var response = new ApiHttpResponse<HttpDataSingle<Dictionary<string,string>>>();
            var _context = new QuartzExecutionDataContext();
            try
            {
                //Authentication Check
                var account = _context.Accounts
                    .Where(x => x.Username == username && x.Password == password).SingleOrDefault();
                if(account != null)
                {
                    var _server = _context.Servers
                        .Where(x => x.Name == serverName)
                        .SingleOrDefault();

                    if (_server == null)
                    {
                        var server = new Server
                        {
                            Name = serverName,
                            IP = HttpContext.Current.Request.UserHostAddress
                        };
                        _context.Servers.Add(server);
                        _context.AccountServers.Add(new AccountServer { AccountID = account.AccountID, ServerID = server.ServerID });
                        _context.SaveChanges();


                        var appIdTemplate = "{0}-{1}-{2}";
                        var appTokenTemplate = "{0}-{1}-{2}-{3}";
                        if (server == null)
                            throw new EntityNotExistsException("Not found server with name: " + serverName);

                        var appId = string.Format(appIdTemplate, server.Name, server.IP, Guid.NewGuid()).Replace(":", "");
                        var appToken = string.Format(appTokenTemplate, server.Name, server.IP, appId, Guid.NewGuid()).Replace(":", "");
                        var appTokenBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(appToken));

                        server.AppId = appId;
                        server.AppToken = appTokenBase64;
                        _context.SaveChanges();


                        response.data.result = new Dictionary<string, string>();
                        response.data.result.Add("AppID", server.AppId);
                        response.data.result.Add("AppToken", server.AppToken);
                        response.Complete(ResultCode.OK, "Server Created");

                        try
                        {
                            WebSocketClient.SendMessage(new SocketMessage
                            {
                                Title = "Nuovo Server " + server.Name + " con ip " + server.IP + " creato da " + username,
                                DataType = server.GetType(),
                                IsImportant = false,
                                Data = server

                            });
                        }
                        catch (Exception ex) { LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex); }
                    }
                    else
                    {
                        var server = _context.Servers.FirstOrDefault(x => x.Name == serverName);
                        response.data.result = new Dictionary<string, string>();
                        response.data.result.Add("AppID", server.AppId);
                        response.data.result.Add("AppToken", server.AppToken);

                        throw new EntityDuplicationException("Already exists server with name: " + serverName);
                    }
                }
                else
                {
                    response.NotAuthorized("Bad credentials for: " + username);
                }

            }
            catch (EntityNotExistsException ex)
            {
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message);
                response.Warning(System.Net.HttpStatusCode.NotFound, ex.Message);
            }
            catch (EntityDuplicationException ex)
            {
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message);
                response.Complete(ResultCode.KO, ex.Message);
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