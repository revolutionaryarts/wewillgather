using Gather.Core.Configuration;

namespace Gather.Core.Domain.Common
{
    public class OwnerSettings : ISettings
    {
        /// <summary>
        /// Flag to use default mail credentials
        /// </summary>
        public bool MailDefaultCredentials { get; set; }

        /// <summary>
        /// Flag to enable SSL when sending mail
        /// </summary>
        public bool MailEnableSSL { get; set; }

        /// <summary>
        /// The display name of the sender
        /// </summary>
        public string MailFromDisplayName { get; set; }

        /// <summary>
        /// The email address of the sender
        /// </summary>
        public string MailFromEmail { get; set; }

        /// <summary>
        /// Host of the mail account to use
        /// </summary>
        public string MailHost { get; set; }

        /// <summary>
        /// Password of the mail account to use
        /// </summary>
        public string MailPassword { get; set; }

        /// <summary>
        /// Port of the mail account to use
        /// </summary>
        public string MailPort { get; set; }

        /// <summary>
        /// User of the mail account to use
        /// </summary>
        public string MailUsername { get; set; }

        /// <summary>
        /// Access token for the Twitter profile to send site tweets from
        /// </summary>
        public string TwitterAccessToken { get; set; }

        /// <summary>
        /// Access token secret for the Twitter profile to send site tweets from
        /// </summary>
        public string TwitterAccessTokenSecret { get; set; }
    }
}