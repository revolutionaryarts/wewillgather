using System;
using Gather.Core.Domain.Projects;

namespace Gather.Core.Domain.ModerationQueue
{
    public class ProjectWithdrawal : BaseEntity
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

    }
}
