using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Gather.ApplicationMonitor.Services;

namespace Gather.ApplicationMonitor
{
    class Program
    {
        
        //const string MAIL_HOST = "mail.freshegg.net";   
        //const string DEV_ENVIRONMENT = "lom";
        //const string DOMAIN_NAME = "http://wewillgather.freshegg.lom/";
        const string MAIL_HOST = "localhost";   
        const string DEV_ENVIRONMENT = "live";          
        const string DOMAIN_NAME = "http://www.wewillgather.co.uk/";

        static void Main()
        {

            Console.Write("\nApplication Health Check Starting");
            Console.Write("\n\n----------------------------------------------------------------------");

            IEnumerable<string> ccEmail = new List<string> { "andrewh@freshegg.com", "dan.ellis@freshegg.com" };

            try
            {

                var monitorLog = new StringBuilder();
                Boolean sendAlert = false;
                
                Console.Write("\n\nChecking " + DOMAIN_NAME);
                Console.Write("\n");

                // Access the site and inspect the status of the response, if we get an error, class it as as a 500.
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(DOMAIN_NAME);
                    webRequest.AllowAutoRedirect = false;

                    Console.Write("\nDownloading response from site . . . ");
                    
                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                    Console.Write(". . . returned " + response.StatusCode + " (" + (int)response.StatusCode + ")");
                    Console.Write("\n\n----------------------------------------------------------------------");

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        monitorLog.Append(DOMAIN_NAME + " - status code being returned : " + response.StatusCode + " (" + (int)response.StatusCode + ")" + "<br>");
                        sendAlert = true;
                    }

                }
                catch(Exception)
                {
                    // 500 returned, site must be down
                    Console.Write(". . . returned exception.\nAssuming that status code being returned is : " + HttpStatusCode.InternalServerError + " (" + (int)HttpStatusCode.InternalServerError + ")");
                    monitorLog.Append(DOMAIN_NAME + " - exception was returned, assuming status code : " + HttpStatusCode.InternalServerError + " (" + (int)HttpStatusCode.InternalServerError + ")" + "<br>");
                    sendAlert = true;
                }

                Console.Write("\n\nChecking scheduled tasks");
                Console.Write("\n");

                var scheduledTaskService = new ScheduledTaskService();
                var tasks = scheduledTaskService.GetAllTasks();

                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {

                        Console.Write("\nChecking . . . " + task.Name);

                        if (!task.Enabled)
                        {
                            // A scheduled task has been disabled, this needs to be reported as they are all important
                            monitorLog.Append(task.Name + " task has been disabled, all tasks are required to be running" + "<br>");
                            Console.Write("\t\tfound a problem, task has been disabled");
                            sendAlert = true;
                        }
                        else
                        {
                            // Give the tasks 3 times the actual time to ensure we don't trigger false alerts, e.g site recycles and delays a task
                            var lastStartTime = DateTime.UtcNow.AddSeconds(-(task.Seconds * 3));
                            if (lastStartTime > task.LastSuccessUtc || task.LastSuccessUtc == null)
                            {
                                monitorLog.Append(task.Name + " task appears to have stopped running." + (task.LastSuccessUtc != null ? " Last run was " + task.LastSuccessUtc + " (utc)" : "") + "<br>");
                                Console.Write("\t\tfound a problem, task has failed to run");
                                sendAlert = true;
                            }
                            else
                            {
                                Console.Write("\t\trunning ok");
                            }
                        }
                    }

                    if (sendAlert)
                    {
                        SendEmail(".:: We Will Gather - Health Monitor Problem Detected (" + DEV_ENVIRONMENT + ") ::.", monitorLog.ToString(), new MailAddress("debug@freshegg.com", "debug@freshegg.com"), new MailAddress("debug@freshegg.com", "debug@freshegg.com"), null, ccEmail);                       
                    }
                }
                else
                {
                    Console.Write("\n\nNo scheduled tasks returned, is the database connection string ok?");
                    SendEmail(".:: We Will Gather - Health Monitor Error (" + DEV_ENVIRONMENT + ") ::.", "Error running the Application health monitor. Is the database connection ok?", new MailAddress("debug@freshegg.com", "debug@freshegg.com"), new MailAddress("debug@freshegg.com", "debug@freshegg.com"), null, ccEmail, false);
                }

                Console.Write("\n\n----------------------------------------------------------------------");
                Console.Write("\n\nApplication Health Check Complete");

            }
            catch (Exception exc)
            {
                SendEmail(".:: We Will Gather - Health Monitor Error (" + DEV_ENVIRONMENT + ") ::.", exc.ToString(), new MailAddress("debug@freshegg.com", "debug@freshegg.com"), new MailAddress("debug@freshegg.com", "debug@freshegg.com"), null, ccEmail, false);
            }
        }

        public static void SendEmail(string subject, string body, MailAddress from, MailAddress to, IEnumerable<string> bcc = null, IEnumerable<string> cc = null, Boolean isHtml = true)
        {

            var message = new MailMessage { From = @from };
            message.To.Add(to);

            if (null != bcc)
            {
                foreach (var address in bcc.Where(bccValue => !String.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }
            if (null != cc)
            {
                foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
                {
                    message.CC.Add(address.Trim());
                }
            }
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = MAIL_HOST;
                smtpClient.Send(message);
            }
        }
    }
}
