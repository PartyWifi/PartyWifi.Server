using System;
using System.Threading.Tasks;

namespace PartyWifi.Server.DataModel
{
    public interface IUnitOfWork : IDisposable
    {
        T GetRepository<T>() where T : class, IRepository;

        Task SaveAsync();
    }
}