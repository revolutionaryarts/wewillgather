using Gather.Core.Domain.Locations;

namespace Gather.Core.Domain.Projects
{
    public class ProjectLocation : BaseEntity
    {
        /// <summary>
        /// Gets or sets the location
        /// </summary>
        public virtual Location Location { get; set; }

        /// <summary>
        /// Gets or sets the location id
        /// </summary>
        public virtual int LocationId { get; set; }

        /// <summary>
        /// Gets or sets the primary location flag
        /// </summary>
        public virtual bool Primary { get; set; }

        /// <summary>
        /// Gets or sets the project
        /// </summary>
        public virtual Project Project { get; set; }

        /// <summary>
        /// Gets or sets the project id
        /// </summary>
        public virtual int ProjectId { get; set; }
    }
}