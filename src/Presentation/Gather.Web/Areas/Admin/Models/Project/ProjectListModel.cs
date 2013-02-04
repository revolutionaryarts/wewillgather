using System.Collections.Generic;
using Gather.Web.Framework.Mvc;
using Gather.Web.Models.Project;

namespace Gather.Web.Areas.Admin.Models.Project
{
    public class ProjectListModel : BaseAdminListModel
    {
        public IList<ProjectModel> Projects { get; set; }
    }
}