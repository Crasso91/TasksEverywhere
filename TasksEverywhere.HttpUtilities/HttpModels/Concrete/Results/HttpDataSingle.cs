using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksEverywhere.HttpUtils.Models.Concrete
{
    public class HttpDataSingle<T>
    {
        public T result { get; set; }
    }
}