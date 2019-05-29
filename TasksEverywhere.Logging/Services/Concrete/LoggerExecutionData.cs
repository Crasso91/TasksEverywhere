using ICeQuartzScheduler.DataLayer.Models;
using ICeScheduler.Quartz.Context.Concrete;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICeScheduler.Quartz.Services.Concrete
{
    public class LoggerExecutionData : BaseLoggingService<LoggerExecutionData>
    {
        

        public override void Configure()
        {
            base.Configure();
        }

        public void CallStart(IJobExecutionContext jobContext)
        {
            try
            {
                var _context = CastleWindsorService.Resolve<QuartzExecutionContext>();
                var call = new Call
                {
                    FireInstenceID = jobContext.FireInstanceId,
                    StartedAt = jobContext.FireTimeUtc.DateTime,
                    NextStart = jobContext.NextFireTimeUtc.HasValue ? jobContext.NextFireTimeUtc.Value.DateTime : DateTime.Parse("01/01/1753"),
                    EndedAt = DateTime.Parse("01/01/1753")
                };

                var job = _context.data.Jobs.FirstOrDefault(x => x.Key == jobContext.JobDetail.Key.Name);
                if (job == null)
                {
                    job = new Job { Key = jobContext.JobDetail.Key.Name, Calls = new List<Call>() };
                    _context.data.Jobs.Add(job);
                }

                job.Calls.Add(call);
                job.Executing = true;
                job.StartedAt = jobContext.FireTimeUtc.DateTime;
                _context.Commit();
                CastleWindsorService.Release(_context);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "", ex);
            }
        }


        public void CallEnd(IJobExecutionContext jobContext)
        {
            try
            {
                var _context = CastleWindsorService.Resolve<QuartzExecutionContext>();
                var job = _context.data.Jobs.FirstOrDefault(x => x.Key == jobContext.JobDetail.Key.Name);
                if (job != null)
                {
                    if (job.Calls.FirstOrDefault(x => x.FireInstenceID == jobContext.FireInstanceId) != null)
                        job.Calls.FirstOrDefault(x => x.FireInstenceID == jobContext.FireInstanceId).EndedAt = DateTime.Now;
                    job.Executing = false;
                }
                _context.Commit();
                CastleWindsorService.Release(_context);
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "", ex);
            }
        }

        public void Error(IJobExecutionContext jobContext, Exception ex)
        {
            try
            {
                var _context = CastleWindsorService.Resolve<QuartzExecutionContext>();
                var job = _context.data.Jobs.FirstOrDefault(x => x.Key == jobContext.JobDetail.Key.Name);
                if (job != null)
                {
                    var err = new Error
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    };
                    if (job.Calls.FirstOrDefault(x => x.FireInstenceID == jobContext.FireInstanceId) != null)
                        job.Calls.FirstOrDefault(x => x.FireInstenceID == jobContext.FireInstanceId).Error = err;
                    job.Executing = false;
                }
                _context.Commit();
                CastleWindsorService.Release(_context);
            }
            catch (Exception ex1)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "", ex1);
            }
        }
    }
}
