using TasksEverywhere.HttpUtilities.Enumerators;
using TasksEverywhere.HttpUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TasksEverywhere.HttpUtils.Entities
{
    public class OperationResult
    {
        public ResultCode Code {get;set;}
        public string Message { get; set; }
        public string Stack { get; set; }
    }
}
