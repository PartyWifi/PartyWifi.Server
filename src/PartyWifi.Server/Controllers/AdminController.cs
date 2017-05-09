using System;
using Microsoft.AspNetCore.Mvc;
using PartyWifi.Server.Components;
using PartyWifi.Server.Models;

namespace PartyWifi.Server.Controllers
{
    public class AdminController : Controller
    {
        private readonly ISlideshowHandler _slideshowHandler;

        private readonly IImageManager _imageManager;

        public AdminController(ISlideshowHandler slideshowHandler, IImageManager imageManager)
        {
            _slideshowHandler = slideshowHandler;
            _imageManager = imageManager;
        }

        // GET: /<controller>/
        public IActionResult Index([FromQuery]int? page)
        {
            var fileList = _imageManager.FileList<AdminModel>(page ?? 1, 10);
            fileList.RotationTime = _slideshowHandler.RotationMs;
            return View(fileList);
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            _imageManager.Delete(id);
            return Ok();
        }

        [HttpPost]
        public IActionResult RotationTime([FromBody]int seconds)
        {
            _slideshowHandler.RotationMs = seconds*1000;
            return Ok();
        }

        public class AdminModel : FileList
        {
            public int RotationTime { get; set; }

            public override string PageUrlBuilder(int page)
            {
                return $"admin?page={page}";
            }
        }
    }
}