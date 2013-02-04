using System.Collections.Generic;
using System.Linq;

namespace Gather.Core
{
    public class PaginatedList<T> : List<T>, IPaginatedList<T>
    {

        #region Fields

        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        #endregion

        #region Constructors

        public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            if (pageSize != -1)
            {
                int total = source.Count();
                TotalCount = total;
                TotalPages = total / pageSize;

                if (total % pageSize > 0)
                    TotalPages++;

                AddRange(source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
            }
            else
            {
                TotalCount = source.Count();
                AddRange(source);
            }
        }

        #endregion

    }
}