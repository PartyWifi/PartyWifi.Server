using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PartyWifi.Server.Model
{
    public abstract class EntityFrameworkRepository<T> : IRepository<T>, IContextBasedRepository
        where T : class, IEntity, new()
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        protected DbContext Context { get; private set; }

        protected DbSet<T> DbSet { get; private set; }

        public virtual IQueryable<T> Linq => DbSet;

        public Task<T[]> GetAll()
        {
            return DbSet.ToArrayAsync();
        }

        public Task<T> GetByKey(long id)
        {
            return DbSet.FirstOrDefaultAsync(e => e.Id == id);
        }

        public T Create()
        {
            var entity = new T();
            DbSet.Add(entity);

            return entity;
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

        void IContextBasedRepository.SetContext(IUnitOfWork uow, DbContext context)
        {
            Guard.ArgumentNotNull(uow, nameof(uow));
            Guard.ArgumentNotNull(context, nameof(context));

            Context = context;
            UnitOfWork = uow;
            DbSet = Context.Set<T>();
        }
    }
}