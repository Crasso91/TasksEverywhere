using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Utilities.Enums
{
    public enum JobDataMapKeys
    {
        Programs = 0,
        LogLevel = 1,
        ReflectionJobData = 2
    }

    public enum PeriodType
    {
        Giornaliero = 1,
        Settimanale = 2,
        Mensile = 3,
        Annuale = 4
    }

    public enum LogLevelType
    {
        Info = 1,
        Debug = 2,  
        Error = 3,
        Nil = -1
    }
}
