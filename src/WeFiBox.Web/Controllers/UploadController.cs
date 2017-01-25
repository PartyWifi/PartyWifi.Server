using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeFiBox.Web.Controllers
{
    public class UploadController : Controller
    {
        private readonly IHostingEnvironment _environment;

        public UploadController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ICollection<IFormFile> files)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            foreach (var file in files)
            {
                if (file.Length <= 0)
                    continue;

                var fileName = $"{Guid.NewGuid()}.jpg";
                var filePath = Path.Combine(uploads, fileName);
                
                using (var memoryStream = new MemoryStream())
                 {
                    await file.CopyToAsync(memoryStream);


                    using (var image = new MagickImage(memoryStream))
                    {
                        if (image.BaseWidth > 1280)
                        {
                            var size = new MagickGeometry(1280, 1024);
                            image.Resize(size);
                        }

                        // Save frame as jpg
                        image.Write(filePath);
                    }
                }
            }

            return View();
        }
    }
}
