using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace PartyWifi.Server.DataModel
{
    public abstract class EntityFrameworkRepository<T> : IRepository<T> where T : class, IEntity
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        protected DbContext Context { get; private set; }

        protected DbSet<T> DbSet { get; private set; }

        public virtual IQueryable<T> Linq => DbSet;

        public EntityFrameworkRepository(IUnitOfWork uow, DbContext context)
        {
            if (context == null) 
                throw new ArgumentNullException(nameof(context));

            if (uow == null)
                throw new ArgumentNullException(nameof(uow));

            Context = context;
            UnitOfWork = uow;
        }

        public void Create(T entity)
        {
            if (entity == null) 
                throw new ArgumentNullException(nameof(entity));
                
            Context.Set<T>().Add(entity);
        }

        public ICollection<T> GetAll()
        {
            return DbSet.ToList();
        }

        public T GetByKey(long id)
        {
            return DbSet.FirstOrDefault(e => e.Id == id);
        }

        public void Remove(T entity)
        {
            if (entity == null)
                return;

            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                return;

            DbSet.RemoveRange(entities.ToArray());
        }
        
        public virtual void Dispose()
        {

        }
    }
}