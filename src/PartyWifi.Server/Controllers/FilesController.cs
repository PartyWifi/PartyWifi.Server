using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PartyWifi.Server.Components;
using PartyWifi.Server.Models;
using System;
using System.Linq;

namespace PartyWifi.Server.Controllers
{
    public class FilesController : Controller
    {
        private readonly IImageManager _manager;

        private readonly int _imagesPerPage;

        public FilesController(IImageManager manager, IOptions<Settings> settings)
        {
            _manager = manager;
            _imagesPerPage = settings.Value.ImagesPerPage;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return Page(1);
        }

        public IActionResult Page(int id)
        {
            var currentPage = id;
            var total = _manager.ImageCount;
            var start = (currentPage - 1) * _imagesPerPage;

            // Get range of files
            var files = _manager.GetRange(start, _imagesPerPage).Select(imageInfo => new FileListEntry
            {
                Id = imageInfo.Id,
                Size = imageInfo.Size,
                UploadDate = imageInfo.UploadDate
            }).ToArray();

            return View(nameof(Index), new FileList 
            {
                Files = files,
                CurrentPage = currentPage,
                Pages = (int)(Math.Ceiling((double)total / _imagesPerPage))
            });
        }
    }
}