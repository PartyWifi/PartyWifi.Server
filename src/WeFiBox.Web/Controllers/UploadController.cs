using ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
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
                    var fileName = $"{DateTime.Now.ToString("yyyyMMdd-hhmmss")}.jpg";

                    // Copy to filesystem for later
                    var filePath = Path.Combine(_settings.UploadDir, fileName);
                    using(var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await memoryStream.CopyToAsync(fileStream);
                        fileStream.Flush();
                        memoryStream.Position = 0;
                    }   
                    
                    // Resize for the slide-show
                    using(var image = new Image(memoryStream))
                    {
                        var widthScale = image.Width / (double) _settings.MaxWidth;
                        var heightScale = image.Height / (double) _settings.MaxHeight;
                        
                        if(widthScale <= 1  && heightScale <= 1)
                          continue;

                        // Find the dimension that needs the most adjustment and create new dimensions
                        var scaling = widthScale > heightScale ? widthScale : heightScale;
                        var newWidth = (int)Math.Floor(image.Width / scaling);                    
                        var newHeight = (int)Math.Floor(image.Height / scaling);

                        filePath = Path.Combine(_settings.ResizedDir, fileName);
                        image.Resize(newWidth, newHeight).Save(filePath);
                    }
                }
            }

            return View();
        }
    }
}
