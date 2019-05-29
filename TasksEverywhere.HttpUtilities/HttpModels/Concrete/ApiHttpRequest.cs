using TasksEverywhere.HttpUtils.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.HttpUtilities.HttpModels.Concrete
{
    public class ApiHttpRequest<T> : IApiHttp<T>
    {
        public HttpStatusCode Code { get; set; }
        public string AppID { get; set; }
        public string AppToken { get; set; }
        public T data { get; set; }
    }
}
