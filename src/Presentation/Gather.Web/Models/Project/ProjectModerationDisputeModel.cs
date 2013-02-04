using System.Collections.Generic;
using Gather.Web.Models.User;
using Gather.Web.Areas.Admin.Models.Project;

namespace Gather.Web.Models.Project
{
    public class ProjectModerationDisputeModel : ProjectModerationModel
    {

        public ProjectModerationDisputeModel()
        {
            CurrentModerators = new List<UserModel>();
            ProjectUserHistory = new List<ProjectUserHistoryModel>();
        }

        public UserModel CreatedByUser { get; set; }

        public IList<UserModel> CurrentModerators { get; set; }

        public IList<ProjectUserHistoryModel> ProjectUserHistory { get; set; }

        public string TypeDescription { get; set; }

    }
}