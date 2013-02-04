using System;
using System.Collections.Generic;
using Gather.Core.Domain.Categories;
using Gather.Core.Domain.Locations;
using Gather.Core.Domain.Users;

namespace Gather.Core.Domain.Projects
{
    public class ProjectUserHistory : BaseEntity
    {
        /// <summary>
        /// The date the history entry was created
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// The project the history action is being created on
        /// </summary>
        public virtual Project Project { get; set; }

        /// <summary>
        /// Gets or sets the user that is making the change
        /// </summary>
        public virtual User CommittingUser { get; set; }

        /// <summary>
        /// Gets or sets the affected user
        /// </summary>
        public virtual User AffectedUser { get; set; }

        /// <summary>
        /// Gets or sets the action that happened to the user
        /// </summary>
        public virtual ProjectUserAction ProjectUserAction
        {
            get
            {
                return (ProjectUserAction)ProjectUserActionId;
            }
            set
            {
                ProjectUserActionId = (int)value;
            }
        }

        /// <summary>
        /// Value for project user action
        /// </summary>
        public virtual int ProjectUserActionId { get; set; }
    }
}