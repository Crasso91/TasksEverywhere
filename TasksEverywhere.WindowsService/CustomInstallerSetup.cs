using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace ICeQuartzScheduler.WindowsService
{
    [RunInstaller(true)]
    public partial class CustomInstallerSetup : System.Configuration.Install.Installer
    {
        public CustomInstallerSetup()
        {
            InitializeComponent();
        }
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            try
            {
                InstallModules();
                //AddConfigurationFileDetails();
            }
            catch (Exception e)
            {
                MessageBox.Show("Impossibile continuare, esecuzione annullata " + e.Message);
                base.Rollback(savedState);
                DeleteWindowsService();
            }
        }


        public override void Rollback(IDictionary savedState)
        {
            try
            {
                base.Rollback(savedState);
                DeleteWindowsService();
            }
            catch (Exception)
            {

            }
        }


        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
                //DeleteWindowsService();
            }
            catch (Exception)
            {

            }
        }

        private void DeleteWindowsService()
        {

            RunProcess("cmd.exe", "/c sc delete IceQuartzScheduler");
        }

        private void InstallModules()
        {
            var modulesDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Context.Parameters["assemblypath"]), "modules");
            bool InstallGit = Context.Parameters["GITCHECKBOX"] == "1";
            bool InstallGitGui = Context.Parameters["GITGUICHECKBOX"] == "1";
            if (InstallGit)
            {
                
                RunProcess(System.IO.Path.Combine(modulesDir, "Git.exe"));
            }
            if (InstallGitGui)
            {
                string selectedGui = Context.Parameters["GITGUI"];
                switch (selectedGui)
                {
                    case "1":
                        RunProcess(System.IO.Path.Combine(modulesDir, "GitHubDesktop.exe"));
                        break;
                    case "2":
                        RunProcess(System.IO.Path.Combine(modulesDir, "GitAtomic.exe"));
                        break;
                    case "3":
                        if (System.Environment.Is64BitOperatingSystem)
                        {
                            RunProcess(System.IO.Path.Combine(modulesDir, "TortoiseGit64.msi"));
                        }
                        else
                        {
                            RunProcess(System.IO.Path.Combine(modulesDir, "TortoiseGit.msi"));
                        }
                        break;
                    case "4":
                        RunProcess(System.IO.Path.Combine(modulesDir, "GitExtensions.msi"));
                        break;
                    default:
                        break;
                }
            }
        }

        private void RunProcess(string filename, string args = "")
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = filename;
            if(args != "") startInfo.Arguments = args;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

        }

        private void AddConfigurationFileDetails()
        {
            try
            {
                

                //string ClientName = Context.Parameters["CLIENTNAME"];

                //string InternalLogging = Context.Parameters["INTERNALLOGGING_ACTIVE"] == "1" ? "true" : "false";
                //string EmailLogging = Context.Parameters["EMAILLOGGING_ACTIVE"] == "1" ? "true" : "false";
                //string InternalLogging_Level = Context.Parameters["INTERNALLOGGING_LEVEL"];
                //string EmailLogging_Leve = Context.Parameters["EMAILLOGGING_LEVEL"];

                //string InternalLogging_FilePath = Context.Parameters["INTERNALLOGGING_FILEPATH"];
                //string EmailLogging_Bcc = Context.Parameters["EMAILLOGGING_BCC"];
                //string EmailLogging_From = Context.Parameters["EMAILLOGGING_FROM"];
                //string EmailLogging_To = Context.Parameters["EMAILLOGGING_TO"];
                //string EmailLogging_Host = Context.Parameters["EMAILLOGGING_HOST"];
                //string EmailLogging_Username = Context.Parameters["EMAILLOGGING_USERNAME"];
                //string EmailLogging_Password = Context.Parameters["EMAILLOGGING_PASS"];


                //// Get the path to the executable file that is being installed on the target computer  
                //string assemblypath = Context.Parameters["assemblypath"];
                //string appConfigPath = assemblypath + ".config";

                //// Write the path to the app.config file  
                //XmlDocument doc = new XmlDocument();
                //doc.Load(appConfigPath);

                //XmlNode configuration = null;
                //foreach (XmlNode node in doc.ChildNodes)
                //    if (node.Name == "configuration")
                //        configuration = node;

                //if (configuration != null)
                //{
                //    var loggersSection = configuration.ChildNodes.Cast<XmlNode>().First(x => x.Name == "LoggerSection");
                //    var network = configuration.ChildNodes.Cast<XmlNode>()
                //        .First(x => x.Name == "system.net").ChildNodes.Cast<XmlNode>()
                //        .First(x => x.Name == "mailSettings").ChildNodes.Cast<XmlNode>()
                //        .First(x => x.Name == "smtp").ChildNodes.Cast<XmlNode>()
                //        .First(x => x.Name == "network");
                //    var clientNameTag = configuration.ChildNodes.Cast<XmlNode>()
                //        .First(x => x.Name == "appSettings").ChildNodes.Cast<XmlNode>()
                //        .First(x => x.Name == "add" && x.Attributes["key"].Value == "ClientName");

                //    if (loggersSection != null)
                //    {
                //        var internalLoggingConfig = loggersSection.ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "loggers").ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "logger" && x.Attributes["type"].Value == "LoggerInternal");

                //        var mailLoggingConfig = loggersSection.ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "loggers").ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "logger" && x.Attributes["type"].Value == "LoggerEmail");

                //        internalLoggingConfig.Attributes["active"].Value = InternalLogging;
                //        mailLoggingConfig.Attributes["active"].Value = EmailLogging;
                //        internalLoggingConfig.Attributes["level"].Value = InternalLogging_Level;
                //        mailLoggingConfig.Attributes["level"].Value = EmailLogging_Leve;

                //        internalLoggingConfig.ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "parameters").ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "parameter" && x.Attributes["key"]?.Value == "LogFilePath").Attributes["value"].Value = InternalLogging_FilePath;

                //        mailLoggingConfig.ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "parameters").ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "parameter" && x.Attributes["key"]?.Value == "bcc").Attributes["value"].Value = EmailLogging_Bcc;
                //        mailLoggingConfig.ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "parameters").ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "parameter" && x.Attributes["key"]?.Value == "from").Attributes["value"].Value = EmailLogging_Bcc;
                //        mailLoggingConfig.ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "parameters").ChildNodes.Cast<XmlNode>()
                //            .First(x => x.Name == "parameter" && x.Attributes["key"]?.Value == "to").Attributes["value"].Value = EmailLogging_Bcc;
                //        network.Attributes["host"].Value = EmailLogging_Host;
                //        network.Attributes["userName"].Value = EmailLogging_Username;
                //        network.Attributes["password"].Value = EmailLogging_Password;

                //        clientNameTag.Attributes["value"].Value = ClientName;
                //    }

                //     doc.Save(appConfigPath);
                //}
            }
            catch
            {
                throw;
            }
        }
    }
}
