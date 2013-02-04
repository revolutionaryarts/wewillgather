using System;
using Gather.Core.Domain.Projects;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.User;

namespace Gather.Web.Models.Project
{
    public class ProjectUserHistoryModel : BaseModel
    {
        public DateTime CreatedDate { get; set; }

        public ProjectModel Project { get; set; }

        public UserModel CommittingUser { get; set; }

        public UserModel AffectedUser { get; set; }

        public ProjectUserAction ProjectUserAction { get; set; }

        public int ProjectUserActionId { get; set; }
    }
}