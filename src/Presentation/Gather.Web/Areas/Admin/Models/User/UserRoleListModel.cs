using System.Collections.Generic;
using Gather.Web.Framework.Mvc;
using Gather.Web.Models.User;

namespace Gather.Web.Areas.Admin.Models.User
{
    public class UserRoleListModel : BaseAdminListModel
    {
        public IList<UserRoleModel> Roles { get; set; }
    }
}