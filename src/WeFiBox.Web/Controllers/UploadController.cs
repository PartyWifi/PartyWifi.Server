using ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using FileHelper = System.IO.File;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeFiBox.Web.Controllers
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
                 using (var memoryStream = new MemoryStream())
                 {
                    // Copy to memory first
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    // File name for time sorting
                    var fileName = $"{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.jpg";

                    // Copy to filesystem for later
                    // Append '.tmp' to exclude file from slideshow until upload was finished
                    var filePath = Path.Combine(_settings.UploadDir, fileName);
                    using(var fileStream = new FileStream(filePath + ".tmp", FileMode.Create))
                    {
                        await memoryStream.CopyToAsync(fileStream);
                        fileStream.Flush();
                        memoryStream.Position = 0;
                    }   
                    
                    // Resize for the slide-show
                    ResizeIfNecessary(memoryStream, fileName);

                    // Rename file to make it accessible to slideshow
                    FileHelper.Move(filePath + ".tmp", filePath);
                }
            }

            return View();
        }

        /// <summary>
        /// Create a resized version of the image if necessary
        /// </summary>
        private void ResizeIfNecessary(Stream memoryStream, string fileName)
        {
            using(var image = new Image(memoryStream))
            {
                var widthScale = image.Width / (double) _settings.MaxWidth;
                var heightScale = image.Height / (double) _settings.MaxHeight;
                
                if(widthScale <= 1  && heightScale <= 1)
                    return;

                // Find the dimension that needs the most adjustment and create new dimensions
                var scaling = widthScale > heightScale ? widthScale : heightScale;
                var newWidth = (int)Math.Floor(image.Width / scaling);                    
                var newHeight = (int)Math.Floor(image.Height / scaling);

                var filePath = Path.Combine(_settings.ResizedDir, fileName);
                image.Resize(newWidth, newHeight).Save(filePath);
            }
        }
    }
}
