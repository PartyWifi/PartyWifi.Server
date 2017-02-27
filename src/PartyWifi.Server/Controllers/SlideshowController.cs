using System.IO;
using Microsoft.AspNetCore.Mvc;
using PartyWifi.Server.Components;

namespace PartyWifi.Server.Controllers
{
    public class SlideshowController : Controller
    {
        private readonly ISlideshowHandler _slideshowHandler;
        private readonly IImageManager _imageManager;

        public SlideshowController(ISlideshowHandler slideshowHandler, IImageManager imageManager)
        {
            _slideshowHandler = slideshowHandler;
            _imageManager = imageManager;
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
            var init = _slideshowHandler.GetInitial();
            return Json(init);
        }

        /// <summary>
        /// Get name of next image
        /// </summary>
        public IActionResult Next()
        {
            var next = _slideshowHandler.Next();
            return Json(next);            
        }
        
        /// <summary>
        /// Get next image based on current id
        /// </summary>
        public IActionResult Image(string id)
        {           
            var info = _imageManager.Get(id);

            // Return file stream
            return File(new FileStream(info.ResizedPath, FileMode.Open), "image/jpeg");
        }
    }
}