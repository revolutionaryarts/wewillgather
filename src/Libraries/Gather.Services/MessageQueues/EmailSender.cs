using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Hosting;
using Gather.Core.Domain.Common;
using Gather.Core.Infrastructure;

namespace Gather.Services.MessageQueues
{
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses ist</param>
        /// <param name="isHtml">Html or Plain text</param>
        public void SendEmail(string subject, string body, string toAddress, string toName, IEnumerable<string> bcc = null, IEnumerable<string> cc = null, Boolean isHtml = true)
        {
            var ownerSettings = EngineContext.Current.Resolve<OwnerSettings>();
            SendEmail(subject, body, new MailAddress(ownerSettings.MailFromEmail, ownerSettings.MailFromDisplayName), new MailAddress(toAddress, toName), bcc, cc, isHtml);
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses ist</param>
        /// <param name="isHtml">Html or Plain text</param>
        public void SendEmail(string subject, string body, string fromAddress, string fromName, string toAddress, string toName, IEnumerable<string> bcc = null, IEnumerable<string> cc = null, Boolean isHtml = true)
        {
            SendEmail(subject, body, new MailAddress(fromAddress, fromName), new MailAddress(toAddress, toName), bcc, cc, isHtml);
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="from">From address</param>
        /// <param name="to">To address</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses ist</param>
        /// <param name="isHtml">Html or Plain text</param>
        public virtual void SendEmail(string subject, string body, MailAddress from, MailAddress to, IEnumerable<string> bcc = null, IEnumerable<string> cc = null, Boolean isHtml = true)
        {
            var coreSettings = EngineContext.Current.Resolve<CoreSettings>();
            var ownerSettings = EngineContext.Current.Resolve<OwnerSettings>();

            var message = new MailMessage {From = @from};
            message.To.Add(to);
            
            if (null != bcc)
                foreach (var address in bcc.Where(bccValue => !String.IsNullOrWhiteSpace(bccValue)))
                    message.Bcc.Add(address.Trim());

            if (null != cc)
                foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
                    message.CC.Add(address.Trim());

            string emailBody;
            string siteDomain = coreSettings.Domain;
            if (!siteDomain.EndsWith("/"))
                siteDomain += "/";

            try
            {
                emailBody = LoadTemplates(HostingEnvironment.MapPath("~/Views/Templates/EmailTemplate.htm"));
                emailBody = string.Format(emailBody, siteDomain, body);
            }
            catch
            {
                emailBody = body;
            }

            message.Subject = subject;
            message.Body = emailBody;
            message.IsBodyHtml = isHtml;

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = ownerSettings.MailDefaultCredentials;
                smtpClient.Host = ownerSettings.MailHost;
                smtpClient.Port = int.Parse(ownerSettings.MailPort);
                smtpClient.EnableSsl = ownerSettings.MailEnableSSL;
                smtpClient.Credentials = ownerSettings.MailDefaultCredentials ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(ownerSettings.MailUsername, ownerSettings.MailPassword);
                smtpClient.Send(message);
            }
        }

        public static string LoadTemplates(string path)
        {
            string page;

            using (StreamReader sr = File.OpenText(path))
            {
                page = sr.ReadToEnd();
            }

            return page;
        }
    }
}