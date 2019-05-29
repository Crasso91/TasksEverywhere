using TasksEverywhere.Utilities.Enums;
using Quartz;
using System;
using System.Collections.Generic;

namespace TasksEverywhere.Quartz.Context.Jobs.Abstract
{
    public interface ICustomTrigger
    {
        string Name { get; set; }
        string Group { get; set; }
        bool Active { get; set; }
        PeriodType Period { get; set; }
        DateTime StartDate { get; set; }
        int Life { get; set; }
        int Interval { get; set; }
        List<DayOfWeek> WeekDays { get; set; }
        IntervalUnit IntervalUnit { get; set; }
        IntervalUnit LifeUnit { get; set; }
        ITrigger GetQuartzTrigger(string JobName, string JobGroup);
    }
}