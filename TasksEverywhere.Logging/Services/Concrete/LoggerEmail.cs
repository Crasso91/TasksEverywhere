using ICeScheduler.Quartz.Config.Sections;
using ICeScheduler.Quartz.Enums;
using ICeScheduler.Quartz.Extensions;
using ICeScheduler.Quartz.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICeScheduler.Quartz.Services.Concrete
{
    public class LoggerEmail : BaseLoggingService<LoggerEmail>
    {
        private SmtpClient _smtpClient;
        private MailSettingsSectionGroup mailerSettings;

        public LoggerEmail()
        {
            this.Configure();
        }

        public override void Configure()
        {
            base.Configure();
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            mailerSettings = cfg.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

            if (mailerSettings != null)
            {
                try
                {
                    int port = mailerSettings.Smtp.Network.Port;
                    string host = mailerSettings.Smtp.Network.Host;
                    _smtpClient = new SmtpClient(host, port);
                    _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    NetworkCredential Credentials = new NetworkCredential(mailerSettings.Smtp.Network.UserName, mailerSettings.Smtp.Network.Password);
                    _smtpClient.Credentials = Credentials;
                }
                catch (Exception ex)
                {
                    LoggerInternal.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "InitError", ex);
                }
            }
        }

        public override void Debug(Type type, MethodBase method, string message)
        {
            if (LogLevel != LogLevelType.Debug) return;

            try
            {
                var mailMessage = this.GetMailMessage(type, "Debug");
                mailMessage.Body = GetFormattedMessage(type, method, message);
                _smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                LoggerInternal.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "InitError", ex);
            }
        }

        public override void Error(Exception ex)
        {

            try
            {
                var mailMessage = this.GetMailMessage(this.GetType(), "Error");
                mailMessage.Body = GetFormattedMessage(this.GetType(), MethodBase.GetCurrentMethod(), ex.Message);
                _smtpClient.Send(mailMessage);
            }
            catch (Exception ex1)
            {
                LoggerInternal.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "InitError", ex1);
            }
        }

        public override void Error(string msg, Exception ex)
        {
            try
            {
                var mailMessage = this.GetMailMessage(this.GetType(), "Error");
                mailMessage.Body = GetFormattedMessage(this.GetType(), MethodBase.GetCurrentMethod(), msg) + " " +  ex.Message + " " + ex.StackTrace;
                _smtpClient.Send(mailMessage);
            }
            catch (Exception ex1)
            {
                LoggerInternal.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "InitError", ex1);
            }
        }

        public override void Error(Type type, MethodBase method, string message, Exception ex)
        {
            try
            {
                var mailMessage = this.GetMailMessage(type, "Error");
                mailMessage.Body = GetFormattedMessage(type, method, message) + " " + ex.Message + " " + ex.StackTrace;
                _smtpClient.Send(mailMessage);
            }
            catch (Exception ex1)
            {
                LoggerInternal.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "InitError", ex1);
            }
        }

        public override void Info(string msg)
        {
            if (LogLevel == LogLevelType.Error) return;
            try
            {
                var mailMessage = this.GetMailMessage(this.GetType(), "Info");
                mailMessage.Body = GetFormattedMessage(this.GetType(), MethodBase.GetCurrentMethod(), msg);
                _smtpClient.Send(mailMessage);
            }
            catch (Exception ex1)
            {
                LoggerInternal.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "InitError", ex1);
            }
        }

        public override void Info(Type type, MethodBase method, string message)
        {
            if (LogLevel == LogLevelType.Error) return;

            try
            {
                var mailMessage = this.GetMailMessage(this.GetType(), "Info");
                mailMessage.Body = GetFormattedMessage(type, method, message);
                _smtpClient.Send(mailMessage);
            }
            catch (Exception ex1)
            {
                LoggerInternal.Instance.Error(this.GetType(), MethodBase.GetCurrentMethod(), "InitError", ex1);
            }
        }

        private MailMessage GetMailMessage(Type type, string Level)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(parameters["from"].Value);
            mailMessage.To.Add(parameters["to"].Value);
            var appSettings = ConfigurationManager.OpenExeConfiguration(Process.GetCurrentProcess().MainModule.FileName).AppSettings;
            if(!String.IsNullOrEmpty(parameters["bcc"].Value))
            {
                foreach (var bcc in parameters["bcc"].Value.Split(','))
                {
                    mailMessage.Bcc.Add(bcc);
                }
            }
            mailMessage.Subject = string.Format("{0} - {1} {2}", appSettings.Settings["ClientName"].Value, Level, type.Name);
            mailMessage.IsBodyHtml = false;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.SubjectEncoding = Encoding.UTF8;
            return mailMessage;
        }
    }
}
