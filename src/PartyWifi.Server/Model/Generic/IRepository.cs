using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyWifi.Server.Model
{
    public interface IRepository : IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }

    public interface IRepository<T> : IRepository where T : IEntity
    {
        IQueryable<T> Linq { get; }

        T GetByKey(long id);

        ICollection<T> GetAll();

        T Create();

        void Remove(T entity);
    }
}