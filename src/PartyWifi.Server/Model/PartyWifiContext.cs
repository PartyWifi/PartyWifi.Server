using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PartyWifi.Server.Model
{
    public class PartyWifiContext : DbContext, IDesignTimeDbContextFactory<PartyWifiContext>
    {
        private readonly string _connectionString;

        public PartyWifiContext()
        {
            
        }

        public PartyWifiContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<ImageEntity> Images { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        PartyWifiContext IDesignTimeDbContextFactory<PartyWifiContext>.CreateDbContext(string[] args)
        {
            var conStringBuilder = new SqliteConnectionStringBuilder{DataSource = "partywifi.sqlite"};
            return new PartyWifiContext(conStringBuilder.ConnectionString);
        }
    }

}