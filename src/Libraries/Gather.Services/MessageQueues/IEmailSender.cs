using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Gather.Services.MessageQueues
{
    public interface IEmailSender
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
        void SendEmail(string subject, string body, string toAddress, string toName, IEnumerable<string> bcc = null, IEnumerable<string> cc = null, Boolean isHtml = true);

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
        void SendEmail(string subject, string body, string fromAddress, string fromName, string toAddress, string toName, IEnumerable<string> bcc = null, IEnumerable<string> cc = null, Boolean isHtml = true);

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
        void SendEmail(string subject, string body, MailAddress from, MailAddress to, IEnumerable<string> bcc = null, IEnumerable<string> cc = null, Boolean isHtml = true);
    }
}