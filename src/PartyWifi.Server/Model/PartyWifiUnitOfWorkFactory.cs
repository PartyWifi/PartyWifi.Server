using Microsoft.Data.Sqlite;

namespace PartyWifi.Server.Model
{
    public class PartyWifiUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly RepositoryHolder _repositories = new RepositoryHolder();
        private readonly string _connectionString;

        public PartyWifiUnitOfWorkFactory()
        {
            var conStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = "partywifi.sqlite"
            };

            _connectionString = conStringBuilder.ConnectionString;
            _repositories[typeof(IImageRepository)] = () => new ImageRepository();
            _repositories[typeof(IImageVersionRepository)] = () => new ImageVersionRepository();
        }

        public IUnitOfWork Create()
        {
            var context = new PartyWifiContext(_connectionString);

            context.Database.EnsureCreated();
            //context.Database.Migrate();

            return new EntityFrameworkUnitOfWork(context, _repositories);
        }
    }
}