using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using PartyWifi.Server.Components;
using PartyWifi.Server.Models;

namespace PartyWifi.Server.Controllers
{
    public class SlideshowController : Controller
    {
        private readonly IImageManager _imageManager;
        private readonly Settings _settings;

        public SlideshowController(IOptions<Settings> settings, IImageManager imageManager)
        {
            _imageManager = imageManager;
            _settings = settings.Value;
        }

        /// <summary>
        /// Render index page of slideshow with latest picture
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Receive init values as json for non HTML clients
        /// </summary>
        public IActionResult Init()
        {
            var files = _imageManager.GetAll();
            var latestFile = files.Last();

            return Json(new SlideshowInit(latestFile.Name, _settings.ImageRotationSec * 1000));
        }

        /// <summary>
        /// Get name of next image
        /// </summary>
        public IActionResult Next(string id)
        {
            //TODO: find better way to get and do not iterate over all images
            var files = _imageManager.GetAll().Select(i => i.Name).ToArray();

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
            var fileStream = _imageManager.Get(id);

            // Return file stream
            return File(fileStream, "image/jpeg");
        }
    }
}