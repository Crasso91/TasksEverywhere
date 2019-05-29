using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using TasksEverywhere.Quartz.Extensions;
using TasksEverywhere.Quartz.Services;
using TasksEverywhere.Utilities.Enums;
using log4net.Core;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Jobs.Concrete
{
    public class CustomJob : ICustomJob
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public bool Active { get; set; }
        public List<ICustomTrigger> Triggers { get; set; } = new List<ICustomTrigger>();
        public string Description { get; set; }
        public List<IJobParameter> Parameters { get; set; } = new List<IJobParameter>();


        public Task Execute(IJobExecutionContext context)
        {
            var command = string.Empty;
            var logId = "Fire Instance ID : " + context.FireInstanceId + " Task: " + context.JobDetail.Key + " --> ";

            //var program = context.JobDetail.JobDataMap.GetString(JobDataMapKeys.Program.ToString());
            var programs = (context.JobDetail.JobDataMap.Get(JobDataMapKeys.Programs.ToString()) as JArray).ToObject<List<string>>();
            var logLevel = context.GetLogLevel();

            return Task.Run(() =>
            {
                foreach (var prog in programs)
                {
                    try
                    {
                        RunProgram(logId, prog);
                        Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), logId + " Exception", ex);
                    }
                    
                }
            });
            //.ContinueWith((x) => {
                //if (x.IsFaulted)
                //{
                //    LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), logId + " Exception", x.Exception);
                //    //CastleWindsorService.Resolve<LoggerExecutionData>().Error(context, x.Exception);
                //}
                //else
                //    CastleWindsorService.Resolve<LoggerExecutionData>().CallEnd(context);
            //});
        }

        public IJobDetail GetQuartzJob()
        {
            IDictionary<string, object> _params = new Dictionary<string, object>();
            Parameters.ForEach(x => _params.Add(x.Key.ToString(), x.Value));

            return JobBuilder.Create(this.GetType())
                .SetJobData(new JobDataMap(_params))
                .WithDescription(this.Description)
                .WithIdentity(new JobKey(this.Name, this.Group))
                .Build();

        }

        public List<ITrigger> GetQuartzTriggers()
        {
            List<ITrigger> triggers = new List<ITrigger>();

            this.Triggers.Where(trigger => trigger.Active).ToList().ForEach(x => {
                var trigger = x.GetQuartzTrigger(this.Name, this.Group);
                trigger.Log();
                triggers.Add(trigger);
            });
            return triggers;
        }

        private string GetFilename(string program)
        {
            var filename = string.Empty;
            var fileext = System.IO.Path.GetExtension(program);
            switch (fileext.ToLower())
            {
                case ".vbs":
                    filename = "\"CScript.exe\"";
                    break;
                case ".bat":
                    filename = "\"cmd.exe\"";
                    break;
                case ".exe":
                    filename = "\"" + program + "\"";
                    break;
                default:
                    break;
            }
            return filename;
        }

        private string GetCommand(string program)
        {
            var command = string.Empty;
            var fileext = System.IO.Path.GetExtension(program);
            switch (fileext.ToLower())
            {
                case ".vbs":
                    command = "//B \"" + program + "\"";
                    break;
                case ".bat":
                    command = "/c \"" + program + "\"";
                    break;
                case ".exe":
                    break;
                default:
                    command = "";
                    break;
            }
            return command;
        }

        private void RunProgram(string logId, string program)
        {
            LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), logId + " Command Line : " + GetFilename(program) + " " + GetCommand(program));
            //LogsAppendersManager.Instance.Info(this.GetType(), MethodBase.GetCurrentMethod(), logId + " Started");
            //CastleWindsorService.Resolve<LoggerExecutionData>().CallStart(context);


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = GetFilename(program);
            startInfo.Arguments = GetCommand(program);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            //LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), logId + " Command Line: " + output);

            //if(String.IsNullOrEmpty(output))
            //    LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "Command output", new Exception(output));
            if (!string.IsNullOrEmpty(error))
                throw new JobExecutionException(error);
            //LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), logId + "Command error", new Exception(error));


            //LogsAppendersManager.Instance.Info(this.GetType(), MethodBase.GetCurrentMethod(), logId + " Ended");
        }

    }
}
