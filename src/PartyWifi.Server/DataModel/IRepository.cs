using System.Linq;
using System.Collections.Generic;
using System;

namespace PartyWifi.Server.DataModel
{
    public interface IRepository : IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }

    public interface IRepository<T> : IRepository where T : IEntity
    {
        IQueryable<T> Linq { get; }

        T GetByKey(long id);

        void Create(T entity);

        ICollection<T> GetAll();

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}