namespace PartyWifi.Server.Model
{
    public interface IImageVersionRepository : IRepository<ImageVersionEntity>
    {

    }

    public class ImageVersionRepository : EntityFrameworkRepository<ImageVersionEntity>, IImageVersionRepository
    {

    }
}