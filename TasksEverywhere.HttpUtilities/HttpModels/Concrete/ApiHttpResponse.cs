using TasksEverywhere.HttpUtilities.Enumerators;
using TasksEverywhere.HttpUtils.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace TasksEverywhere.HttpUtils.Models.Concrete
{
    public class ApiHttpResponse<T> : IApiHttp<T> 
        where T : new()
    {
        public HttpStatusCode Code { get; set; }
        public T data { get; set; }
        public Entities.OperationResult operationResult { get; set; } = new Entities.OperationResult();

        public ApiHttpResponse()
        {
            data = new T();
        }

        public void Fault(string message)
        {
            this.Code = HttpStatusCode.BadGateway;
            operationResult.Code = ResultCode.KO;
            operationResult.Message = message;
        }

        public void Fault(Exception ex)
        {
            this.Code = HttpStatusCode.InternalServerError;
            operationResult.Code = ResultCode.KO;
            while(ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            operationResult.Message = ex.Message;
            operationResult.Stack = ex.StackTrace;
        }

        public void Complete(ResultCode resultCode, string message = "")
        {
            this.Code = HttpStatusCode.OK;
            operationResult.Code = resultCode;
            operationResult.Message = message;
        }

        public void Warning(HttpStatusCode code, string message)
        {
            this.Code = code;
            operationResult.Code = ResultCode.OK;
            operationResult.Message = message;
        }
        public void NotAuthorized(string message)
        {
            this.Code = HttpStatusCode.Unauthorized;
            operationResult.Code = ResultCode.KO;
            operationResult.Message = message;
        }
    }
}