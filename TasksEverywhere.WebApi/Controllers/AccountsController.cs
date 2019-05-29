using TasksEverywhere.DataLayer.Context.Concrete;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.HttpUtilities.Enumerators;
using TasksEverywhere.HttpUtilities.Extensions;
using TasksEverywhere.HttpUtilities.HttpModels.Concrete;
using TasksEverywhere.HttpUtils.Attributes;
using TasksEverywhere.HttpUtils.Models.Concrete;
using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Utilities.Exceptions;
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
    public class AccountsController : ApiController
    {

        [HttpPost, ActionName("List"), LogActionFilter]
        public HttpResponseMessage AccountsList([FromBody] ApiHttpRequest<Dictionary<string,object>> request)
        {
            var filters = request.data;
            var _context = new QuartzExecutionDataContext();
            var response = new ApiHttpResponse<HttpDataList<Account>>();
            var username = Request.Username();
            try
            {
                //only admin can see accounts
                Action _canProceed = () =>
                {
                    var accounts = _context.Accounts.Filter(filters);
                    response.data.result = accounts.ToList();
                    response.Complete(ResultCode.OK, "");
                };
                Action _else = () => response.Complete(ResultCode.OK, "Only admin and inspector can see Accounts"); ;

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

        [HttpPost, ActionName("Add"), LogActionFilter]
        public HttpResponseMessage Add([FromBody] ApiHttpRequest<Account> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<bool>>();
            var username = Request.Username();
            var account = request.data;
            var _context = new QuartzExecutionDataContext();
            try
            {
                Action _canProceed = () => 
                {
                    var dbAccount = _context.Accounts.Where(x => x.Username == account.Username).SingleOrDefault();
                    if(dbAccount == null)
                    {
                        _context.Accounts.Add(account);
                        _context.SaveChanges();
                    }
                    else throw new EntityDuplicationException("Account alredy exists with username " + account.Username);
                };
                Action _else = () => response.Complete(ResultCode.OK, "Only admin and inspector can see Accounts");

                _context.Accounts.
                    Where(x => x.Username == username)
                    .SingleOrDefault()
                    .ProceedIf(x => x.IsAdminOrIspector, _canProceed, _else);
            }
            catch (EntityDuplicationException ex)
            {
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message);
                response.Complete(ResultCode.OK, ex.Message);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
                response.Fault(ex);
            }
            return Request.CreateResponse(response.Code, response);
        }

        [HttpPost, ActionName("Update"), LogActionFilter]
        public HttpResponseMessage Update([FromBody] ApiHttpRequest<Account> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<bool>>();
            var username = Request.Username();
            var account = request.data;
            var _context = new QuartzExecutionDataContext();
            try
            {
                Action _canProceed = () =>
                {
                    var dbAccount = _context.Accounts.Where(x => x.Username == account.Username).SingleOrDefault();
                    if (dbAccount != null)
                    {
                        dbAccount.Password = account.Password;
                        dbAccount.Roles = account.Roles;
                        _context.SaveChanges();
                    }
                    else throw new EntityNotExistsException("No account for username: " + account.Username);
                };
                Action _else = () => response.Complete(ResultCode.OK, "Only admin and inspector can update Accounts");
                _context.Accounts
                    .Where(x => x.Username == username)
                    .SingleOrDefault()
                    .ProceedIf(x => x.IsAdminOrIspector, _canProceed, _else);
            }
            catch (EntityNotExistsException ex)
            {
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message);
                response.Complete(ResultCode.OK, ex.Message);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message, ex);
                response.Fault(ex);
            }
            return Request.CreateResponse(response.Code, response);
        }

        [HttpPost, ActionName("Delete"), LogActionFilter]
        public HttpResponseMessage Delete([FromBody] ApiHttpRequest<long> request)
        {
            var response = new ApiHttpResponse<HttpDataSingle<bool>>();
            var username = Request.Username();
            var accountID = request.data;
            var _context = new QuartzExecutionDataContext();
            try
            {
                Action _canProceed = () =>
                {
                    var dbAccount = _context.Accounts.Where(x => x.AccountID == accountID).SingleOrDefault();
                    if (dbAccount != null)
                        _context.Accounts.Remove(dbAccount);
                    else throw new EntityNotExistsException("No account for account id: " + accountID);

                    _context.SaveChanges();
                };
                Action _else = () => response.Complete(ResultCode.OK, "Only admin and inspector can delete Accounts");
                _context.Accounts
                    .Where(x => x.Username == username)
                    .SingleOrDefault()
                    .ProceedIf(x => x.IsAdminOrIspector, _canProceed, _else);
            }
            catch (EntityNotExistsException ex)
            {
                LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message);
                response.Complete(ResultCode.OK, ex.Message);
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