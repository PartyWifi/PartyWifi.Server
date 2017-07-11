using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        /// </summary>
        public ActionResult Load(ImageVersions version, string id)
        {
            var image = _manager.Get(id);
            var imageVersion = image.Versions.First(v => v.Version == version);

            var stream = _manager.Open(imageVersion.Hash);
            return File(stream, "image/jpeg");
        }

        /// <summary>
        /// Adds a new image from the post form files
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add()
        {
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Copy to memory first
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    await _manager.Add(memoryStream);
                }
            }

            return Ok();
        }
    }
}