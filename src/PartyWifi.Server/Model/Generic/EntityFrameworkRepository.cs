using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Enumerable = System.Linq.Enumerable;

namespace PartyWifi.Server.Model
{
    public abstract class EntityFrameworkRepository<T> : IRepository<T>, IContextBasedRepository
        where T : class, IEntity, new()
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        protected DbContext Context { get; private set; }

        protected DbSet<T> DbSet { get; private set; }

        public virtual IQueryable<T> Linq => DbSet;

        void IContextBasedRepository.SetContext(IUnitOfWork uow, DbContext context)
        {
            Guard.ArgumentNotNull(uow, nameof(uow));
            Guard.ArgumentNotNull(context, nameof(context));

            Context = context;
            UnitOfWork = uow;
            DbSet = Context.Set<T>();
        }

        public T Create()
        {
            var entity = new T();
            DbSet.Add(entity);

            return entity;
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

        public virtual void Dispose()
        {

        }
    }
}