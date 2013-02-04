using Gather.Core.Domain.Projects;

namespace Gather.Core.Domain.ModerationQueue
{
    public class ProjectChangeRequest : BaseEntity
    {
        /// <summary>
        /// The project that is the target project.
        /// </summary>
        public virtual Project ParentProject { get; set; }

        /// <summary>
        /// The project that is awaiting approval.
        /// </summary>
        public virtual Project ChangeProject { get; set; }

        /// <summary>
        /// Gets or sets the Moderation queue
        /// </summary>
        public virtual ModerationQueue ModerationQueue { get; set; }
    }
}
