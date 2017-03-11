using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PartyWifi.Server.Components;

namespace PartyWifi.Server
{
    public class ImageController : Controller
    {
        private readonly IImageManager _manager;

        public ImageController(IImageManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Load special version of an image
        /// <summary>
        public ActionResult Load(ImageVersions version, string id)
        {
            var image = _manager.Get(id);
            var imageVersion = image.Versions.First(v => v.Version == version);

            var stream = _manager.Open(imageVersion.ImageHash);
            return File(stream, "image/jpeg");
        }
    }
}