using System;
using System.Threading.Tasks;

namespace PartyWifi.Server.Model
{
    public interface IUnitOfWork : IDisposable
    {
        T GetRepository<T>() where T : class, IRepository;

        Task SaveAsync();

        void Save();
    }
}