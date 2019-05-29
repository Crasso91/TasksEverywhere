using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksEverywhere.Api.Dashboard.Models
{
    public class CallPerHour
    {
        public int hour { get; set; }
        public int calls { get; set; }
    }
}