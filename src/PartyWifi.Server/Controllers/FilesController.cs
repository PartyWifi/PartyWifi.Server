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
            var fileList = _manager.FileList<OverviewFileList>(1, _imagesPerPage);
            return View(fileList);
        }

        public IActionResult Page(int id)
        {
            var fileList = _manager.FileList<OverviewFileList>(id, _imagesPerPage);
            return View(nameof(Index), fileList);
        }

        private class OverviewFileList : FileList
        {
            public override string PageUrlBuilder(int page)
            {
                return $"/files/page/{page}";
            }
        }
    }
}