using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartyWifi.Server.Model
{
    public interface IRepository : IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }

    public interface IRepository<T> : IRepository where T : IEntity
    {
        IQueryable<T> Linq { get; }

        Task<T> GetByKey(long id);

        Task<T[]> GetAll();

        T Create();

        void Remove(T entity);
    }
}