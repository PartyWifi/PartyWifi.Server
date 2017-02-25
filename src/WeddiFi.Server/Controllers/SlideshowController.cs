using System.IO;
using FileHelper = System.IO.File;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using WeddiFi.Server.Models;

namespace WeddiFi.Server.Controllers
{
    public class SlideshowController : Controller
    {
        private readonly Settings _settings;

        public SlideshowController(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(CreateInitModel());
        }

        /// <summary>
        /// Receive init values as json for non HTML clients
        /// </summary>
        public IActionResult Init()
        {
            return Json(CreateInitModel());
        }

        /// <summary>
        /// Create init model for slideshow client
        /// </summary>
        private SlideshowInit CreateInitModel()
        {
            var files = AllFiles();
            var latestFile = files[files.Length - 1];

            return new SlideshowInit(latestFile, _settings.ImageRotationSec * 1000);
        }

        /// <summary>
        /// Get name of next image
        /// </summary>
        public IActionResult Next(string id)
        {
            var files = AllFiles();
            // Find current index in all files starting from the back
            var currentIndex = 0;
            for (var index = files.Length - 1; index  >= 0; index--)
            {
                if(files[index] != id)
                    continue;
                currentIndex = index;
                break;
            }

            // Return next in array or select random from older photos
            var nextIndex = currentIndex + 1;
            if (nextIndex >= files.Length)
            {
                var rand = new Random();
                nextIndex = rand.Next(files.Length);
            }
            return Json(new { file = files[nextIndex] });            
        }

        /// <summary>
        /// Get next image based on current id
        /// </summary>
        public IActionResult Image(string id)
        {           
            // Check if file has a resized clone, otherwise use original
            var filePath = Path.Combine(_settings.ResizedDir, id);
            if(!FileHelper.Exists(filePath))
            {
                filePath = Path.Combine(_settings.UploadDir, id);
            }

            // Return file stream
            var stream = new FileStream(filePath, FileMode.Open);
            return File(stream, "image/jpeg");
        }

        /// <summary>
        /// Get all files in the UploadDir in chronological order
        /// </summary>
        private string[] AllFiles()
        {
            return Directory
                .EnumerateFiles(_settings.UploadDir, "*.jpg") // Get all files excluding '.tmp' files
                .Select(Path.GetFileName) // Extract file name
                .OrderBy(fn => fn) // Order them
                .ToArray();
        }
    }
}
