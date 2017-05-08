using Microsoft.EntityFrameworkCore;

namespace PartyWifi.Server.DataModel
{
    public class PartyWifiContext : DbContext
    {
        public DbSet<ImageUploadEntity> ImageUploads { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=partywifi.sqlite");
        }
    }
}