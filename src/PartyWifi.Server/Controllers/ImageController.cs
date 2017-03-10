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

        public ActionResult Original(string id)
        {
            var image = _manager.Get(id);
            // Find none resized version
            var version = image.Versions.First(v => v.Version == ImageVersions.Original);
            var stream = _manager.Open(version.ImageHash);

            return File(stream, "image/jpeg");;
        }

        public ActionResult Resized(string id)
        {
            var image = _manager.Get(id);
            // Find biggest resized version
            var version = image.Versions.First(v => v.Version == ImageVersions.Resized);
            var stream = _manager.Open(version.ImageHash);

            return File(stream, "image/jpeg");;
        }

        public ActionResult Thumbnail(string id)
        {
            var image = _manager.Get(id);
            // Find smallest resized version
            var version = image.Versions.First(v => v.Version == ImageVersions.Thumbnail);
            var stream = _manager.Open(version.ImageHash);

            return File(stream, "image/jpeg");;
        }
    }
}