using System.Collections.Generic;
using System.Web.Routing;

namespace Gather.Web.Models.Project
{
    public class ProjectListPagedModel
    {
        public ProjectListPagedModel()
        {
            Projects = new List<BaseProjectModel>();
        }

        public RouteValueDictionary AdditionalPagerValues { get; set; }

        public int MobileNextResultCount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public IList<BaseProjectModel> Projects { get; set; }

        public int TotalCount { get; set; }
    }
}