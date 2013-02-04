using System.Collections.Generic;
using System.Linq;

namespace Gather.Core.Data
{
    public interface IRepository<T> where T : BaseEntity
    {
        T GetById(object id);
        void Insert(T entity);
        void Update(T entity);
        void BulkUpdate(IList<T> entities);
        void Delete(T entity, bool persist = false);
        void BulkDelete(IList<T> entities, bool persist = false);
        IQueryable<T> Table { get; }
    }
}