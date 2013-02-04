using System;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.User;

namespace Gather.Web.Models.MessageQueues
{
    public class MessageQueueModel : BaseModel
    {

        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the user to receive the message
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user to receive the message
        /// </summary>
        public UserModel User { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the short message which is sent by email/twitter
        /// </summary>
        public string ShortBody { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOn { get; set; }

    }
}