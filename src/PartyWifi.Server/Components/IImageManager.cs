using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PartyWifi.Server.Components
{
    public interface IImageManager
    {
        void Initialize();

        IEnumerable<ImageInfo> GetAll();

        Stream Get(ImageInfo info);

        Stream Get(string name);

        Task Add(Stream stream);
    }
}
