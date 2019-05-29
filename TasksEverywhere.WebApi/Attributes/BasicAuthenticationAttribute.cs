using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.DataLayer.Context.Concrete;
using TasksEverywhere.DataLayer.Enumerators;
using TasksEverywhere.HttpUtilities.Extensions;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Utilities.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http.Filters;

namespace TasksEverywhere.WebApi.Attributes
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        private List<string> AuthorizedIps = new List<string>();
        private bool dbAuth = false;

        public BasicAuthenticationAttribute()
        {
            
        }

        public BasicAuthenticationAttribute(bool _dbAuth = true, string _ipListConfigName = "")
        {
            AuthorizedIps = ConfigurationManager.AppSettings[_ipListConfigName]?.Split('|').ToList();
            dbAuth = _dbAuth;
        }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //Check if ip is in configured list, unauthorize if is not
            if(AuthorizedIps != null && AuthorizedIps.Count > 0 && AuthorizedIps.FirstOrDefault(x=> actionContext.Request.RequestUri.AbsoluteUri.Contains(x)) == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            //check if has autorization header, unathorize if has not
            if (actionContext.Request.Headers.Authorization != null)
            {
                // Gets header parameters  
                string authenticationString = actionContext.Request.Headers.Authorization.Parameter;
                string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));

                // Gets username and password  
                var username = actionContext.Request.Username();
                var password = actionContext.Request.Password();
                //var appId = actionContext.Request.AppID();
                //var appToken = actionContext.Request.AppToken();
                string ip = HttpContext.Current.Request.UserHostAddress;
                bool isAuthorized = false; 

                //Check if is authorized via DB
                if (dbAuth)
                {
                    //DbContext initilization
                    var _context = CastleWindsorService.Resolve<QuartzExecutionDataService>();

                    //Validate username password and appid apptoken or admin
                    isAuthorized = _context.data.Accounts
                        .Where(x => x.Username == username && x.Password == password)
                        .SingleOrDefault() != null;

                    if(!isAuthorized) isAuthorized = _context.data.Servers
                            .Where(x => x.AppId == username && x.AppToken == password)
                            .SingleOrDefault() != null;
                    
                    CastleWindsorService.Release(_context);
                }
                if(!isAuthorized)
                {
                    var configUsername = ConfigurationManager.AppSettings["AuthorizedUsername"];
                    var configPassword = ConfigurationManager.AppSettings["AuthorizedPassword"];

                    if(!username.ToLower().Equals(configUsername) && !password.Equals(configPassword))
                    {
                        // returns unauthorized error
                        isAuthorized = false;
                    }
                    else isAuthorized = true;
                }
                if (!isAuthorized)
                {
                    LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Not Authotirzed username: " + username);
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            else
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

            base.OnAuthorization(actionContext);
        }
    }
}