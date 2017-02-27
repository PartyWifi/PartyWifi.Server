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

        public IActionResult FileList()
        {
            var files = _imageManager.GetAll().Select(imageInfo => new FileListEntry
            {
                Name = imageInfo.Name,
                PublicUrl = $"/uploads/resized/{imageInfo.Name}",
                Size = imageInfo.Size,
                UploadDate = imageInfo.UploadDate
            }).ToList();

            return View(files);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
