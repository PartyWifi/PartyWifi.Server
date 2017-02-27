using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileHelper = System.IO.File;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ImageSharp;

namespace PartyWifi.Server.Controllers
{
    public class UploadController : Controller
    {
        private readonly Settings _settings;

        public UploadController(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ICollection<IFormFile> files)
        {
            foreach (var file in files.Where(file => file.Length > 0))
            {
                using (var originalStream = new MemoryStream())
                {
                    // Copy to memory first
                    await file.CopyToAsync(originalStream);
                    originalStream.Position = 0;

                    // Create unambiguous filename
                    var fileName = $"{Guid.NewGuid()}.jpg";

                    // Copy to filesystem for later
                    var originalPath = Path.Combine(_settings.OriginalsDir, fileName);
                    await SaveFromStream(originalStream, originalPath);

                    // Resize for the slide-show
                    var resizedStream = ResizeIfNecessary(originalStream);

                    // Rename file to make it accessible to slideshow
                    var resizedPath = Path.Combine(_settings.ResizedDir, fileName);
                    await SaveFromStream(resizedStream, resizedPath);
                }
            }

            return View();
        }

        /// <summary>
        /// Save the given stream to a file path
        /// </summary>
        private static async Task SaveFromStream(Stream stream, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
                fileStream.Flush();
                stream.Position = 0;
            }
        }

        /// <summary>
        /// Create a resized version of the image if necessary
        /// </summary>
        private Stream ResizeIfNecessary(Stream memoryStream)
        {
            using (var image = new Image(memoryStream))
            {
                var widthScale = image.Width / (double)_settings.MaxWidth;
                var heightScale = image.Height / (double)_settings.MaxHeight;

                if (widthScale <= 1 && heightScale <= 1)
                    return memoryStream;

                // Find the dimension that needs the most adjustment and create new dimensions
                var scaling = widthScale > heightScale ? widthScale : heightScale;
                var newWidth = (int)Math.Floor(image.Width / scaling);
                var newHeight = (int)Math.Floor(image.Height / scaling);

                image.Resize(newWidth, newHeight);

                var resizedStream = new MemoryStream();

                //Resave in same stream
                image.Save(resizedStream);
                resizedStream.Position = 0;

                return resizedStream;
            }
        }
    }
}
