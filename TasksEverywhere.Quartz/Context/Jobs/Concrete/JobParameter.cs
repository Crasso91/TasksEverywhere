using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksEverywhere.Utilities.Enums;

namespace TasksEverywhere.Quartz.Context.Jobs.Concrete
{
    public class JobParameter : Abstract.IJobParameter
    {
        public JobDataMapKeys Key { get; set; }
        public object Value { get; set; }
    }
}
