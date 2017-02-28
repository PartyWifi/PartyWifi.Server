using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PartyWifi.Server.Components
{
    public interface IImageManager
    {
        void Initialize();

        IEnumerable<ImageInfo> GetAll();

        ImageInfo Get(string imageId);

        Task Add(Stream stream);

        event EventHandler<ImageInfo> Added;
    }
}
