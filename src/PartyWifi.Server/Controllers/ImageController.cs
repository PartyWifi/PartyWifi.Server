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
            var stream = _manager.Open(image.Original);

            return File(stream, "image/jpeg");;
        }

        public ActionResult Resized(string id)
        {
            var image = _manager.Get(id);
            var stream = _manager.Open(image.Resized);

            return File(stream, "image/jpeg");;
        }

        public ActionResult Thumbnail(string id)
        {
            var image = _manager.Get(id);
            var stream = _manager.Open(image.Thumbnail);

            return File(stream, "image/jpeg");;
        }
    }
}