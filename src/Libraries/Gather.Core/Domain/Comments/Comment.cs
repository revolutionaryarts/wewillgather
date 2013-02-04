using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Users;

namespace Gather.Core.Domain.Comments
{
    public class Comment : BaseEntity
    {
        private ICollection<Comment> _responses; 

        /// <summary>
        /// Gets or sets the Author of the comment
        /// </summary>
        public virtual User Author { get; set; }

        /// <summary>
        /// Created by using user id
        /// </summary>
        public virtual int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the user creation date
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the response id
        /// </summary>
        public virtual Comment InResponseTo { get; set; }

        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public virtual int? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public virtual int? ModeratedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime? ModeratedDate { get; set; }

        /// <summary>
        /// The number of times that the comment has been asked to be moderator
        /// </summary>
        public virtual int ModerationRequestCount { get; set; }

        /// <summary>
        /// The project that is attached to the comment.
        /// </summary>
        public virtual Project Project { get; set; }

        /// <summary>
        /// Gets or sets the comment responses
        /// </summary>
        public virtual ICollection<Comment> Responses
        {
            get { return _responses ?? (_responses = new Collection<Comment>()); }
            set { _responses = value; }
        }

        /// <summary>
        /// Gets or sets the comment
        /// </summary>
        public virtual string UserComment { get; set; }

    }
}