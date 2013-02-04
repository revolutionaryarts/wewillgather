namespace Gather.Services.MessageQueues
{
    public interface ITwitterMessageSender
    {
        /// <summary>
        /// Sends a twitter message
        /// </summary>
        /// <param name="twitterProfile">User to send to</param>
        /// <param name="twitterUsername">Twitter username</param>
        /// <param name="body">Message to send</param>
        bool SendTwitterMessage(string twitterProfile,string twitterUsername, string body);
    }
}