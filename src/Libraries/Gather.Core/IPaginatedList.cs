using System.Collections.Generic;

namespace Gather.Core
{
    public interface IPaginatedList<T> : IList<T>
    {
        int TotalCount { get; }
        int TotalPages { get; }
    }
}