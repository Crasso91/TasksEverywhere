using TasksEverywhere.DataLayer.Context.Concrete;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.HttpUtilities.Enumerators;
using TasksEverywhere.HttpUtils.Models.Concrete;
using TasksEverywhere.Logging.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace TasksEverywhere.WebApi.Controllers
{
    public class AccountsVerificationController : ApiController
    {
        [HttpPost, ActionName("exists")]
        public HttpResponseMessage Exists([FromBody] Account account)
        {
            var response = new ApiHttpResponse<HttpDataSingle<Account>>();
            var _context = new QuartzExecutionDataContext();
            try
            {
                var dbAccount = _context.Accounts
                    .Where(x => x.Username == account.Username && x.Password == account.Password)
                    .SingleOrDefault();
                response.data.result = dbAccount;
                response.Complete(ResultCode.OK, string.Empty);
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