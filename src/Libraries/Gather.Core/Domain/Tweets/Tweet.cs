using System;
namespace Gather.Core.Domain.Tweets
{
    /// <summary>
    /// Represents a Twitter object
    /// </summary>
    public class Tweet : BaseEntity
    {
        /// <summary>
        /// The Date the tweet was created
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the tweet text
        /// </summary>
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the twitter id
        /// </summary>
        public virtual long TwitterId { get; set; }

        /// <summary>
        /// Gets or sets the twitter user id
        /// </summary>
        public virtual string TwitterProfile { get; set; }

        /// <summary>
        /// Gets or sets the twitter name
        /// </summary>
        public virtual string TwitterName { get; set; }
        
        /// <summary>
        /// Gets or set the user name of the person who tweeted
        /// </summary>
        public virtual string UserName { get; set; }
    }
}
