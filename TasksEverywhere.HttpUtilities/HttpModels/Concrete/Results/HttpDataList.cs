using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksEverywhere.HttpUtils.Models.Concrete
{
    public class HttpDataList<T>
    {
        public List<T> result { get; set; } = new List<T>();
    }
}