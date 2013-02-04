using Gather.Core.Domain.Users;

namespace Gather.Core
{
    public interface IWorkContext
    {
        User CurrentUser { get; set; }
    }
}