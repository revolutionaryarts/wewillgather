using System.Collections.Generic;
using Gather.Core.Domain.Security;

namespace Gather.Web.Areas.Admin.Models.Common
{
    public class NavigationModel
    {
        public NavigationModel()
        {
            Sections = new List<NavigationSectionModel>();
        }

        public IList<NavigationSectionModel> Sections { get; set; }
    }

    public class NavigationSectionModel
    {
        public NavigationSectionModel()
        {
            Icon = "/Areas/Admin/Content/Images/menu-icon-settings.png";
            Items = new List<NavigationItemModel>();
            RequiredPermissions = new List<PermissionRecord>();
        }

        public bool Active { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public string Target { get; set; }
        public string Title { get; set; }

        public IList<NavigationItemModel> Items { get; set; }
        public IList<PermissionRecord> RequiredPermissions { get; set; }
    }

    public class NavigationItemModel
    {
        public NavigationItemModel()
        {
            RequiredPermissions = new List<PermissionRecord>();
        }

        public int Position { get; set; }
        public string Target { get; set; }
        public string Title { get; set; }

        public IList<PermissionRecord> RequiredPermissions { get; set; }
    }
}