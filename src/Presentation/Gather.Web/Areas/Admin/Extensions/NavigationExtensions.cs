using System.Collections.Generic;
using Gather.Core.Domain.Security;
using Gather.Web.Areas.Admin.Models.Common;

namespace Gather.Web.Areas.Admin.Extensions
{
    public static class NavigationExtensions
    {

        public static void AddChildLink(this NavigationSectionModel section, string title, string target, List<PermissionRecord> requiredPermissions = null)
        {
            if (requiredPermissions == null)
                requiredPermissions = new List<PermissionRecord>();

            section.Items.Add(new NavigationItemModel
            {
                Target = target,
                Title = title,
                RequiredPermissions = requiredPermissions
            });
        }

    }
}