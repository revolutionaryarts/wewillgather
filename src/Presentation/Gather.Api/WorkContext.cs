using Gather.Core;
using Gather.Core.Domain.Users;

namespace Gather.Api
{
    public class WorkContext : IWorkContext
    {
        public User CurrentUser
        {
            get
            {
                return null;
            }
            set
            {
                // Do nothing
            }
        }
    }
}