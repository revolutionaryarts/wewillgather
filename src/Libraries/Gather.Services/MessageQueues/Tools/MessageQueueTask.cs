using System;
using System.Linq;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Users;
using Gather.Core.Infrastructure;
using Gather.Services.Logging;
using Gather.Services.Tasks;

namespace Gather.Services.MessageQueues.Tools
{
    public class MessageQueueTask : ITask
    {
        public void Execute()
        {
            // Initialize engine
            EngineContext.Initialize(false);

            // Resolve any services
            var messageQueueService = EngineContext.Current.Resolve<IMessageQueueService>();

            // Retrieve all messages to send via email or twitter
            var messageQueue = messageQueueService.GetMessageQueueForMessageSend();

            // Start processing each message in the queue
            foreach (var message in messageQueue)
            {
                try
                {
                    // If we're sending a message to the site owner, or a user whose primary authentication method is Facebook, send an email
                    if ((message.User.UserRoles.Any(i => i.SystemName == SystemUserRoleNames.SiteOwner) || message.User.PrimaryAuthMethod == AuthenticationMethod.Facebook) 
                        && !string.IsNullOrEmpty(message.User.Email))
                    {
                        var emailSender = EngineContext.Current.Resolve<IEmailSender>();
                        emailSender.SendEmail(message.Subject, message.Body, message.User.Email, message.User.DisplayName);
                        message.SentOn = DateTime.Now;
                    }
                    else if(message.User.PrimaryAuthMethod == AuthenticationMethod.Twitter || 
                        (!string.IsNullOrEmpty(message.User.TwitterProfile) && !string.IsNullOrEmpty(message.User.TwitterDisplayName))) // Otherwise, send a tweet
                    {
                        var twitterSender = EngineContext.Current.Resolve<ITwitterMessageSender>();
                        if (twitterSender.SendTwitterMessage(message.User.TwitterProfile, message.User.TwitterDisplayName, string.Format(message.ShortBody, message.Id)))
                             message.SentOn = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    var logger = EngineContext.Current.Resolve<ILogService>();
                    logger.Error(string.Format("Error sending message. {0}", ex.Message), ex);
                }
                finally
                {
                    message.SentTries = message.SentTries + 1;
                    messageQueueService.UpdateQueuedMessage(message);
                }
            }
        }
    }
}