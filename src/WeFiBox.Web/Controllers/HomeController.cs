using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WeFiBox.Web.Controllers
{
    public class FileListEntry
    {
        public string Name { get; set; }

        public long Size { get; set; }

        public string PublicUrl { get; set; }

        public DateTime UploadDate { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly Settings _settings;

        public HomeController(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FileList()
        {
            var fileListEntries = (from image in Directory.GetFiles(_settings.UploadDir)
                                   let fileInfo = new FileInfo(image)
                                   select new FileListEntry
                                   {
                                       Name = fileInfo.Name,
                                       PublicUrl = $"/uploads/{fileInfo.Name}",
                                       Size = fileInfo.Length,
                                       UploadDate = fileInfo.LastWriteTime //??
                                   }).ToList();

            return View(fileListEntries);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
