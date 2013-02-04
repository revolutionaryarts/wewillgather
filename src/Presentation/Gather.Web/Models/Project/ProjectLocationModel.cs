using Gather.Web.Models.Location;

namespace Gather.Web.Models.Project
{
    public class ProjectLocationModel
    {
        public LocationModel Location { get; set; }

        public int LocationId { get; set; }

        public bool Primary { get; set; }

        public ProjectModel Project { get; set; }

        public int ProjectId { get; set; }
    }
}