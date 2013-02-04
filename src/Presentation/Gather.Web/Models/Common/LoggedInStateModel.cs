using Gather.Web.Models.User;

namespace Gather.Web.Models.Common
{
    public class LoggedInStateModel
    {
        public UserModel CurrentUser { get; set; }
        public bool IsAdmin { get; set; }
    }
}