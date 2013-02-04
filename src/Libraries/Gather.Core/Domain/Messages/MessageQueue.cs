using System;
using Gather.Core.Domain.Users;

namespace Gather.Core.Domain.Messages
{
    public class MessageQueue : BaseEntity
    {
        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public virtual int Priority { get; set; }

        /// <summary>
        /// Gets or sets the user to receive the message
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Gets or sets the short message which is sent by email/twitter
        /// </summary>
        public virtual string ShortBody { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public virtual string Body { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public virtual int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public virtual DateTime? SentOn { get; set; }

        /// <summary>
        /// Gets the twitter username
        /// </summary>
        public virtual string TwitterUsername { get; set; }

        /// <summary>
        /// Gets the twitter profile
        /// </summary>
        public virtual string TwitterProfile { get; set; }
    }
}