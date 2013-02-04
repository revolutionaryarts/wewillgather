using System;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Messages;
using Gather.Core.Infrastructure;
using Gather.Services.Logging;
using Twitterizer;

namespace Gather.Services.MessageQueues
{
    public class TwitterMessageSender : ITwitterMessageSender
    {
        /// <summary>
        /// Sends a twitter message
        /// </summary>
        /// <param name="twitterProfile">User to send to</param>
        /// <param name="twitterUsername">Twitter username</param>
        /// <param name="body">Message to send</param>
        public bool SendTwitterMessage(string twitterProfile, string twitterUsername, string body)
        {
            var ownerSettings = EngineContext.Current.Resolve<OwnerSettings>();
            var siteSettings = EngineContext.Current.Resolve<SiteSettings>();

            // Convert the profile if to a decimal
            decimal profileId;
            decimal.TryParse(twitterProfile, out profileId);

            // Build the token object required to send the tweets
            var tokens = new OAuthTokens
            {
                AccessToken = ownerSettings.TwitterAccessToken,
                AccessTokenSecret = ownerSettings.TwitterAccessTokenSecret,
                ConsumerKey = siteSettings.TwitterConsumerKey,
                ConsumerSecret = siteSettings.TwitterConsumerSecret
            };

            try
            {
                // If we have a profile id, try sending the user a direct message
                // This will fail if the user isn't following us
                if (profileId > 0)
                {
                    var directResponse = TwitterDirectMessage.Send(tokens, profileId, body);
                    if (directResponse.Result == RequestResult.Success)
                        return true;
                }

                // Direct message failed
                // Fall back to sending a tweet containing a mention
                string tweetMessage = (!string.IsNullOrEmpty(twitterUsername) ? "@" + twitterUsername + " " : "") + body;
                if (!tweetMessage.Contains(siteSettings.TwitterHashTag))
                    tweetMessage += " " + siteSettings.TwitterHashTag;

                var mentionResponse = TwitterStatus.Update(tokens, tweetMessage);

                // If the tweet was send, jump out
                if (mentionResponse.Result == RequestResult.Success)
                    return true;

                // If the tweet failed because it's duplicate, jump out 
                if (mentionResponse.Result == RequestResult.Unauthorized && mentionResponse.ErrorMessage == "Status is a duplicate.")
                    return true;

                // If we've reached this point, something has gone wrong, log the error
                var logger = EngineContext.Current.Resolve<ILogService>();
                logger.Error(string.Format("Error sending message via twitter. {0}", mentionResponse.Result + " - " + mentionResponse.ErrorMessage));
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogService>();
                logger.Error(string.Format("Error sending message via twitter. {0}", ex.Message), ex);
            }

            return false;
        }
    }
}