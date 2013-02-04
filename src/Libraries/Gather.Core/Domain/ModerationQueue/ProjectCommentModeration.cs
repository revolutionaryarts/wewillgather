using System;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Common;

namespace Gather.Core.Domain.ModerationQueue
{
    public class ProjectCommentModeration : BaseEntity
    {
        /// <summary>
        /// The project that is awaiting approval.
        /// </summary>
        public virtual Comment Comment { get; set; }

        /// <summary>
        /// Gets or sets the Moderation queue
        /// </summary>
        public virtual ModerationQueue ModerationQueue { get; set; }

        /// <summary>
        /// Gets or sets the reason for the comment complaint
        /// </summary>
        public virtual String Reason { get; set; }

        /// <summary>
        /// Gets or sets the request type identifier
        /// </summary>
        public virtual int ComplaintId { get; set; }

        /// <summary>
        /// Gets or sets the request type
        /// </summary>
        public virtual ProjectCommentComplaintType ComplaintType
        {
            get
            {
                return (ProjectCommentComplaintType)ComplaintId;
            }
            set
            {
                ComplaintId = (int)value;
            }
        }
    }
}