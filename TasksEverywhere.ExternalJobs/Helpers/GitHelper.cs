using TasksEverywhere.Logging.Services.Concrete;
using TasksEverywhere.Utilities.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.ExternalJobs.Helpers
{
    public class GitHelper
    {
        public string WorkingPath { get; set; }

        public GitHelper(string workingPath)
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "GitHelper instanced with working path: " + workingPath, null);
            WorkingPath = workingPath ?? throw new ArgumentNullException(nameof(workingPath));
        }

        public void Init()
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(WorkingPath, ".git"))) return;
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),"Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "init";
            process.Start();
            process.WaitForExit();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), 
                "Git init " + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();
            
        }

        public void Origin(string url)
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "remote add origin " + url;
            process.Start();
            process.WaitForExit();
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git origin " + url +  GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if(!process.HasExited) process.Kill();

        }

        internal void Config(string v)
        {
            var process = GetGitProcess();
            process.StartInfo.Arguments = "config " + v;
            process.Start();
            process.WaitForExit();
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git config " + v +  GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();

        }

        internal void Pull()
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "git pull origin master";
            process.Start();
            process.WaitForExit();
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git pull origin " + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();

        }

        public bool ExistsMaster()
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "show-branch";
            process.Start();
            process.WaitForExit();
            var output = process.StandardOutput.ReadToEnd();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git show-branch " + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();


            return string.IsNullOrEmpty(output);
        }

        public string GetBranches()
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "show-branch";
            process.Start();
            process.WaitForExit();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git show-branch " + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();


            return process.StandardOutput.ReadToEnd();
        }

        public void AddFile(string filepath)
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "add " + filepath;
            process.Start();
            process.WaitForExit();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git add " + filepath + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();

        }

        public void AddFolder(string folder)
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "add " + folder;
            process.Start();
            process.WaitForExit();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git add " + folder + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();


        }

        public void AddAllModification()
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "add -u";
            process.Start();
            process.WaitForExit();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git add - u " + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();

        }

        public void Stage(string _string)
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "stage -v " + _string;
            process.Start();
            process.WaitForExit();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git stage " + _string  + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();

        }

        public void Branch(string branch)
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "branch --track -f " + branch;
            process.Start();
            process.WaitForExit();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git branch--track - f " + branch  + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();

        }

        public void SetHead(string branch)
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "remote set-head origin " + branch;
            process.Start();
            process.WaitForExit();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git remote set-head origin " + branch +  GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();

        }

        public void Commit(string message)
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "commit -m \"" + message + "\"";
            process.Start();
            process.WaitForExit();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git commit -m \"" + message + "\" " + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();

        }

        public void Push(string branch)
        {
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var process = GetGitProcess();
            process.StartInfo.Arguments = "push --progress -u origin " + branch;
            process.Start();
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            process.WaitForExit();

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "Git push -u origin " + branch + GetFormattedOutputMessage(process)
                , null);
            if (!process.HasExited) process.CloseMainWindow();
            if (!process.HasExited) process.Kill();

        }

        public void Ignore (List<string> ignoreList)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(WorkingPath,".gitignore"))) return;
            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(), "Start", null);

            var ignored = new List<string>();
            var gitIgnorePath = Path.Combine(WorkingPath, ".gitignore");
            if (File.Exists(gitIgnorePath)) ignored = File.ReadAllLines(gitIgnorePath).ToList();

            //check if i need to remove any element from ignored file
            foreach(var ignore in ignored)
            {
                var exists = ignoreList.SingleOrDefault(x => x == ignore) != null;
                if (!exists) ignored.Remove(ignore);
            }
            //add all elemente to ignored file (if not exists)
            foreach(var ignore in ignoreList)
            {
                var exists = ignored.FirstOrDefault(x => x == ignore) != null;
                if (!exists) ignored.Add(ignore);
            }
            File.WriteAllLines(gitIgnorePath, ignored);

            LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                "GitHelper initialized. added " + ignored.Count + " ignores to file .gitignore : " + 
                string.Join(Environment.NewLine, ignored)
                , null);
        }

        private  string GetGitDirectory()
        {
            foreach (DictionaryEntry key in Environment.GetEnvironmentVariables())
            {
                if (key.Key.Equals("Path")) return ((string)key.Value).Split(';').First(x => x.Contains("Git"));
            }
            throw new Exception("Git Enviroment variable not found in Path");
        }

        private Process GetGitProcess()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.UseShellExecute = true;
            //startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.CreateNoWindow = false;
            startInfo.WorkingDirectory = WorkingPath;
            startInfo.FileName = Path.Combine(GetGitDirectory(), "git.exe");
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            //process.Exited += (p, args) =>
            //{
            //    LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Debug, this.GetType(), MethodBase.GetCurrentMethod(),
            //        "Process Exited", null);
            //    try
            //    {
            //        var pr = (Process)p;
            //        if (!pr.HasExited) pr.Kill();
            //    }
            //    catch (Exception ex)
            //    {
            //        LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "", ex);
            //    }
            //};
            process.OutputDataReceived += (p, args) =>
            {
                if (!String.IsNullOrEmpty(args.Data))
                {
                    LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                   "output " + args.Data.ToString(), null);
                }
            };
            process.ErrorDataReceived += (p, args) =>
            {
                if (!String.IsNullOrEmpty(args.Data))
                {
                    LogsAppendersManager.Instance.ConfiguredLog(LogLevelType.Info, this.GetType(), MethodBase.GetCurrentMethod(),
                   "output error " + args.Data.ToString(), null);
                }
            };
            return process;
        }

        private string GetFormattedOutputMessage(Process process)
        {
            return string.Empty;
            //return " exited with:" + Environment.NewLine + "- output: " + /*process.StandardOutput.ReadToEnd() +*/ Environment.NewLine + "- error: " /*+ process.StandardError.ReadToEnd()*/;
        }
    }
}
