using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PartyWifi.Server.Model
{
    public class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly IList<IRepository> _createdRepos = new List<IRepository>();
        private readonly RepositoryHolder _repositoryHolder;
        private DbContext _context;
        private bool _disposed;

        public EntityFrameworkUnitOfWork(DbContext context, RepositoryHolder repositoryHolder)
        {
            _context = context;
            _repositoryHolder = repositoryHolder;
        }

        public T GetRepository<T>() where T : class, IRepository
        {
            var repo = _createdRepos.OfType<T>().SingleOrDefault();
            if (repo != null)
                return repo;

            repo = (T)_repositoryHolder[typeof(T)]();
            
            ((IContextBasedRepository)repo).SetContext(this, _context);

            _createdRepos.Add(repo);

            return repo;
        }

        public Task Save()
        {
            return _context.SaveChangesAsync();
        }

        protected void CloseContext()
        {
            if (_context == null)
                return;

            _context.Dispose();
            _context = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CloseContext();
                    foreach (var repo in _createdRepos)
                        repo.Dispose();
                }
            }

            _disposed = true;
        }
    }
}