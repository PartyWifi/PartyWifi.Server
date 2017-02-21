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
        private readonly string _uploads;
        private readonly string _resized;

        public UploadController(IOptions<DirectoryConfigs> directories)
        {
            _uploads = directories.Value.UploadDir;
            _resized = directories.Value.ResizedDir;
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
                    var filePath = Path.Combine(_uploads, fileName);
                    using(var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await memoryStream.CopyToAsync(fileStream);
                        fileStream.Flush();
                        memoryStream.Position = 0;
                    }   
                    
                    // Resize for the slide-show
                    using(var image = new Image(memoryStream))
                    {
                        if(image.Width <= 1920 && image.Height <= 1080)
                          continue;

                        filePath = Path.Combine(_resized, fileName);
                        image.Resize(1920, 1080)
                             .Save(filePath);
                    }
                }
            }

            return View();
        }
    }
}
