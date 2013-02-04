using System;
using Gather.Core.Domain.Users;

namespace Gather.Core.Domain.SuccessStories
{
    public class SuccessStory : BaseEntity
    {
        /// <summary>
        /// Created by using user id
        /// </summary>
        public virtual int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the user creation date
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public virtual int LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the title of the success story
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the Author of the success story
        /// </summary>
        public virtual User Author { get; set; }

        /// <summary>
        /// Gets or sets the title of the success story
        /// </summary>
        public virtual string ShortSummary { get; set; }

        /// <summary>
        /// Gets or sets the title of the success story
        /// </summary>
        public virtual string Article { get; set; }

        /// <summary>
        /// Gets or sets the project Id that the success story is related to
        /// </summary>
        public virtual int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the meta title of the success story
        /// </summary>
        public virtual string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords of the success story
        /// </summary>
        public virtual string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description of the success story
        /// </summary>
        public virtual string MetaDescription { get; set; }
    }
}