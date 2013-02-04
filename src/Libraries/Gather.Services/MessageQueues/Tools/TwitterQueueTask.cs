using System;
using Gather.Core.Infrastructure;
using Gather.Services.Logging;
using Gather.Services.Tasks;

namespace Gather.Services.MessageQueues.Tools
{
    public class TwitterQueueTask : ITask
    {
        public void Execute()
        {
            // Initialize engine
            EngineContext.Initialize(false);

            // Resolve any services
            var messageQueueService = EngineContext.Current.Resolve<IMessageQueueService>();

            // Retrieve all tweets to send
            var tweetQueue = messageQueueService.GetMessageQueueForTwitterSend();

            // Loop each tweet to be sent
            foreach (var tweet in tweetQueue)
            {
                try
                {
                    var twitterSender = EngineContext.Current.Resolve<ITwitterMessageSender>();
                    if (twitterSender.SendTwitterMessage(tweet.TwitterProfile, tweet.TwitterUsername, string.Format(tweet.ShortBody, tweet.Id)))
                        tweet.SentOn = DateTime.Now;
                }
                catch (Exception ex)
                {
                    var logger = EngineContext.Current.Resolve<ILogService>();
                    logger.Error(string.Format("Error sending tweet. {0}", ex.Message), ex);
                }
                finally
                {
                    tweet.SentTries = tweet.SentTries + 1;
                    messageQueueService.UpdateQueuedMessage(tweet);
                }
            }
        }    
    }
}