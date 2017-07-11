using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PartyWifi.Server.Model
{
    public interface IImageRepository : IRepository<ImageEntity>
    {
        Task<ImageEntity> GetByIdentifier(string identifier);
    }

    public class ImageRepository : EntityFrameworkRepository<ImageEntity>, IImageRepository
    {
        public Task<ImageEntity> GetByIdentifier(string identifier)
        {
            return DbSet.SingleOrDefaultAsync(e => e.Identifier.Equals(identifier));
        }
    }
}