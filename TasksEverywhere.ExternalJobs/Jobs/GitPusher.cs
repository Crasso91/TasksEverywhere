using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.ExternalJobs.Helpers;
using TasksEverywhere.Logging.Services.Concrete;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.ExternalJobs.Jobs
{
    public class GitPusher
    {
        public string ServerHost { get; set; }
        public string GitRepoName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        private string RepoRoot { get { return ServerHost + "/" + GitRepoName; } }

        public GitPusher(string serverHost, string gitRepoName, string username, string password)
        {
            ServerHost = serverHost ?? throw new ArgumentNullException(nameof(serverHost));
            GitRepoName = gitRepoName ?? throw new ArgumentNullException(nameof(gitRepoName));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public void Commit(string rootFolder, string exclusionExtension)
        {
            try
            {
                var exclusionExtensionList = (exclusionExtension.Deserialize<JArray>()).ToObject<List<string>>();

                var gitHelper = new GitHelper(rootFolder);
                gitHelper.Init();
                gitHelper.Config("--global user.name \"" + Username + "\"");
                gitHelper.Config("--global user.email \"" + Username + "@srvwhere.it\"");
                gitHelper.Ignore(exclusionExtensionList);
                gitHelper.Origin(String.Format("http://{0}:{1}@{2}", Username, Password, RepoRoot));
                gitHelper.Stage("*");
                gitHelper.AddAllModification();
                gitHelper.Commit("Commit of " + DateTime.Now.ToString("dd-mm-yyyy hh:mm"));
                gitHelper.Push("master");
            }
            catch (Exception ex)
            {
                LogsAppendersManager.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "", ex);
            }
        }


        
    }
}
