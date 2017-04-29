using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PartyWifi.Server.Components;
using PartyWifi.Server.Models;

namespace PartyWifi.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly IImageManager _imageManager;

        public HomeController(IImageManager imageManager)
        {
            _imageManager = imageManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
