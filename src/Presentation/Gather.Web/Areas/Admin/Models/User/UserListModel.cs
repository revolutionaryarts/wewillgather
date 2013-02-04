using System.Collections.Generic;
using Gather.Web.Framework.Mvc;
using Gather.Web.Models.User;

namespace Gather.Web.Areas.Admin.Models.User
{
    public class UserListModel : BaseAdminListModel
    {
        public IList<UserModel> Users { get; set; }
    }
}