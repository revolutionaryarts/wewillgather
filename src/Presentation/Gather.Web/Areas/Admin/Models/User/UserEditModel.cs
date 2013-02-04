using Gather.Web.Models.User;

namespace Gather.Web.Areas.Admin.Models.User
{
    public class UserEditModel
    {
        public bool CanEditRoles { get; set; }
        public bool IsSiteOwner { get; set; }
        public UserModel User { get; set; }
    }
}