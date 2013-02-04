using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Gather.Core;
using Gather.Core.Data;

namespace Gather.Data
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        public EfRepository(IDbContext context)
        {
            _context = context;
        }

        public T GetById(object id)
        {
            return Entities.Find(id);
        }

        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentException("entity");

                Entities.Add(entity);

                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var message = BuildErrorMessage(dbEx);
                throw new Exception(message, dbEx);
            }
        }

        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentException("entity");

                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var message = BuildErrorMessage(dbEx);
                throw new Exception(message, dbEx);
            }
        }

        public void BulkUpdate(IList<T> entities)
        {
            try
            {
                if (entities.Any(entity => entity == null))
                    throw new ArgumentException("entity");

                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var message = BuildErrorMessage(dbEx);
                throw new Exception(message, dbEx);
            }
        }

        public void Delete(T entity, bool removeFromDatabase = false)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentException("entity");

                if (removeFromDatabase)
                    Entities.Remove(entity);
                else
                    entity.Deleted = true;
                    
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var message = BuildErrorMessage(dbEx);
                throw new Exception(message, dbEx);
            }
        }

        public void BulkDelete(IList<T> entities, bool persist = false)
        {
            try
            {
                foreach (var entity in entities)
                {
                    if (entity == null)
                        return;

                    if (persist)
                        Entities.Remove(entity);
                    else
                        entity.Deleted = true;
                }

                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var message = BuildErrorMessage(dbEx);
                throw new Exception(message, dbEx);
            }
        }

        public IQueryable<T> Table
        {
            get
            {
                return Entities;
            }
        }

        private IDbSet<T> Entities
        {
            get { return _entities ?? (_entities = _context.Set<T>()); }
        }

        private string BuildErrorMessage(DbEntityValidationException dbEx)
        {
            return dbEx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors).Aggregate(string.Empty, (current, validationError) => current + (string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine));
        }
    }
}