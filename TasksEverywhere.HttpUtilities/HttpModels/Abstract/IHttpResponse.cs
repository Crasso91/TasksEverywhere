using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace TasksEverywhere.HttpUtils.Models.Abstract
{
    public interface IApiHttp<T>
    {
        HttpStatusCode Code { get; set; }
        T data { get; set; }
    }
}