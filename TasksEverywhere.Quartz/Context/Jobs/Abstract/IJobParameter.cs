using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksEverywhere.Utilities.Enums;

namespace TasksEverywhere.Quartz.Context.Jobs.Abstract
{
    public interface IJobParameter
    {
        JobDataMapKeys Key { get; set; }
        object Value { get; set; }
    }
}
