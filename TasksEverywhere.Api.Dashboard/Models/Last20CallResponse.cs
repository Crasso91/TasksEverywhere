using TasksEverywhere.DataLayer.Models;
using System.Collections.Generic;

namespace TasksEverywhere.Api.Dashboard.Models
{
    public class Last20CallResponse
    {
        public IEnumerable<Call> Last10Calls { get; set; }
        public IEnumerable<Call> Last10ErrorsCalls { get; set; }
    }
}