using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TasksEverywhere.HttpUtils.Models.Abstract
{
    public interface IHttpResult<T>
    {
        T data { get; set; }
    }
}
