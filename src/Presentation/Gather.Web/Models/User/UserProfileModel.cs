using System.Collections.Generic;
using Gather.Web.Models.Project;

namespace Gather.Web.Models.User
{
    public class UserProfileModel
    {
        public bool VisitorIsVolunteer { get; set; }

        public IList<ProjectModel> FinishedProjects { get; set; }

        public bool IsOwnProfile { get; set; }

        public IList<ProjectModel> OrganisedProjects { get; set; }

        public bool ShowEditProfile { get; set; }

        public IList<ProjectModel> UpcomingProjects { get; set; }

        public UserModel User { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }
    }
}