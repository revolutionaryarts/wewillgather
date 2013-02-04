using System;
using Gather.Core.Domain.Users;

namespace Gather.Core.Domain.Logging
{
    /// <summary>
    /// Represents a log record
    /// </summary>
    public class Log : BaseEntity
    {
        /// <summary>
        /// Gets or sets the log level identifier
        /// </summary>
        public virtual int LogLevelId { get; set; }

        /// <summary>
        /// Gets or sets the short message
        /// </summary>
        public virtual string ShortMessage { get; set; }

        /// <summary>
        /// Gets or sets the full exception
        /// </summary>
        public virtual string FullMessage { get; set; }

        /// <summary>
        /// Gets or sets the IP address
        /// </summary>
        public virtual string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public virtual int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the page URL
        /// </summary>
        public virtual string PageUrl { get; set; }

        /// <summary>
        /// Gets or sets the referrer URL
        /// </summary>
        public virtual string ReferrerUrl { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the log level
        /// </summary>
        public virtual LogLevel LogLevel
        {
            get
            {
                return (LogLevel)LogLevelId;
            }
            set
            {
                LogLevelId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual User User { get; set; }
    }
}

