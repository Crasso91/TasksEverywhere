using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using TasksEverywhere.Utilities.Enums;
using TasksEverywhere.Utilities.Exceptions;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Context.Jobs.Concrete
{
    public class CustomTrigger : ICustomTrigger
    {
        public PeriodType Period { get; set; }
        public DateTime StartDate { get; set; }
        public int Life { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public int Interval { get; set; }
        public bool Active { get; set; }
        public List<DayOfWeek> WeekDays { get; set; } = new List<DayOfWeek>();
        public IntervalUnit IntervalUnit { get; set; }
        public IntervalUnit LifeUnit { get; set; }

        public ITrigger GetQuartzTrigger(string JobName, string JobGroup)
        {
            var trigger = TriggerBuilder
                        .Create()
                        .WithIdentity(this.Name, this.Group)
                        .ForJob(JobName, JobGroup);
            switch (this.Period)
            {
                case PeriodType.Giornaliero:
                    var dayScheduler = DailyTimeIntervalScheduleBuilder
                        .Create()
                        .OnEveryDay()
                        .InTimeZone(TimeZoneInfo.Local)
                        .StartingDailyAt(new TimeOfDay(this.StartDate.Hour, this.StartDate.Minute, this.StartDate.Second));
                    if (this.Interval > 0)
                    {
                        dayScheduler.WithInterval(this.Interval, this.IntervalUnit);

                        if (this.LifeUnit == IntervalUnit.Hour)
                            dayScheduler.EndingDailyAt(new TimeOfDay(this.StartDate.AddHours(this.Life).Hour, this.StartDate.Minute, this.StartDate.Minute));
                        else
                            dayScheduler.EndingDailyAt(new TimeOfDay(this.StartDate.AddMinutes(this.Life).Hour, this.StartDate.AddMinutes(this.Life).Minute, this.StartDate.Minute));
                    }
                    else dayScheduler.WithInterval(24, IntervalUnit.Hour);
                    trigger.WithSchedule(dayScheduler);
                    break;
                case PeriodType.Settimanale:
                    var weekScheduler = DailyTimeIntervalScheduleBuilder
                        .Create()
                        .OnDaysOfTheWeek(this.WeekDays.ToArray())
                        .InTimeZone(TimeZoneInfo.Local)
                        .StartingDailyAt(new TimeOfDay(this.StartDate.Hour, this.StartDate.Minute, this.StartDate.Second));
                    if (this.Interval > 0)
                    {
                        if (this.IntervalUnit != IntervalUnit.Minute && this.IntervalUnit != IntervalUnit.Hour)
                            throw new InvalidIntervalUnitException("IntervalUnit (" + this.IntervalUnit.ToString() + ") non accettata per il periodo selezionato (" + this.Period + ")");

                        weekScheduler.WithInterval(this.Interval, this.IntervalUnit);


                        if (this.LifeUnit == IntervalUnit.Hour)
                            weekScheduler.EndingDailyAt(new TimeOfDay(this.StartDate.AddHours(this.Life).Hour, this.StartDate.Minute, this.StartDate.Minute));
                        else if (this.LifeUnit == IntervalUnit.Minute)
                            weekScheduler.EndingDailyAt(new TimeOfDay(this.StartDate.AddMinutes(this.Life).Hour, this.StartDate.AddMinutes(this.Life).Minute, this.StartDate.Minute));
                        else throw new InvalidLifeUnitException("LifeUnit (" + this.LifeUnit.ToString() + ") non accettata per il periodo selezinato (" + this.Period + ")");
                    }
                    else weekScheduler.WithInterval(24, IntervalUnit.Hour);
                    trigger.WithSchedule(weekScheduler);
                    break;
                case PeriodType.Mensile:
                    var monthsScheduler = CalendarIntervalScheduleBuilder
                        .Create()
                        .InTimeZone(TimeZoneInfo.Local);
                    if (this.Interval > 0)
                    {
                        if (this.IntervalUnit != IntervalUnit.Day && this.IntervalUnit != IntervalUnit.Week)
                            throw new InvalidIntervalUnitException("IntervalUnit (" + this.IntervalUnit.ToString() + ") non accettata per il periodo selezionato (" + this.Period + ")");

                        trigger.WithCalendarIntervalSchedule(x =>
                        {
                            x.WithInterval(this.Interval, this.IntervalUnit);
                        });

                        if (this.LifeUnit == IntervalUnit.Day)
                            trigger.EndAt(new DateTimeOffset(this.StartDate.AddDays(this.Life)));
                        else if (this.LifeUnit == IntervalUnit.Week)
                            trigger.EndAt(new DateTimeOffset(this.StartDate.AddDays((7 * this.Life))));
                        else throw new InvalidLifeUnitException("LifeUnit (" + this.LifeUnit.ToString() + ") non accettata per il periodo selezinato (" + this.Period + ")");

                    }
                    else monthsScheduler.WithInterval(1, IntervalUnit.Month);
                    trigger.StartAt(new DateTimeOffset(this.StartDate));
                    break;
                case PeriodType.Annuale:
                    var yearScheduler = CalendarIntervalScheduleBuilder
                        .Create()
                        .InTimeZone(TimeZoneInfo.Local);
                    if (this.Interval > 0)
                    {
                        if (this.IntervalUnit != IntervalUnit.Year && this.IntervalUnit != IntervalUnit.Month)
                            throw new InvalidIntervalUnitException("IntervalUnit (" + this.IntervalUnit.ToString() + ") non accettata per il periodo selezionato (" + this.Period + ")");

                        trigger.WithCalendarIntervalSchedule(x =>
                        {
                            x.WithInterval(this.Interval, this.IntervalUnit);
                        });

                        if (this.LifeUnit == IntervalUnit.Year)
                            trigger.EndAt(new DateTimeOffset(this.StartDate.AddDays(this.Life)));
                        else if (this.LifeUnit == IntervalUnit.Month)
                            trigger.EndAt(new DateTimeOffset(this.StartDate.AddDays((30 * this.Life))));
                        else throw new InvalidLifeUnitException("LifeUnit (" + this.LifeUnit.ToString() + ") non accettata per il periodo selezinato (" + this.Period + ")");

                    }
                    else yearScheduler.WithInterval(1, IntervalUnit.Year);
                    trigger.StartAt(new DateTimeOffset(this.StartDate));
                    break;
                default:
                    return null;
            }
            return trigger.Build();
        }
    }
}
