using System;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Projects;

namespace Gather.Core.Domain.ModerationQueue
{
    public class ProjectModeration : BaseEntity
    {

        /// <summary>
        /// The project that is awaiting approval.
        /// </summary>
        public virtual Project Project { get; set; }

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
        public virtual ProjectComplaintType ComplaintType
        {
            get
            {
                return (ProjectComplaintType)ComplaintId;
            }
            set
            {
                ComplaintId = (int)value;
            }
        }
    }
}
