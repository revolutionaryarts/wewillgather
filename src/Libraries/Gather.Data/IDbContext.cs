using System.Collections.Generic;
using System.Data.Entity;
using Gather.Core;

namespace Gather.Data
{
    public interface IDbContext
    {
        int SaveChanges();
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;
        IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);
    }
}