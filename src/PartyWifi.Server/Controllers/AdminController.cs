using Microsoft.AspNetCore.Mvc;
using PartyWifi.Server.Components;

namespace PartyWifi.Server.Controllers
{
    public class AdminController : Controller
    {
        private readonly ISlideshowHandler _slideshowHandler;

        public AdminController(ISlideshowHandler slideshowHandler)
        {
            _slideshowHandler = slideshowHandler;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RotationTime()
        {
            return Json(new { time = _slideshowHandler.RotationMs/1000});
        }

        [HttpPost]
        public IActionResult RotationTime([FromBody]int seconds)
        {
            _slideshowHandler.RotationMs = seconds*1000;
            return Ok();
        }
    }
}