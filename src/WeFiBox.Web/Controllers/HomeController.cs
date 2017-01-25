using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace WeFiBox.Web.Controllers
{
    public class FileListEntry
    {
        public string Name { get; set; }

        public long Size { get; set; }

        public DateTime UploadDate { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _environment;

        public HomeController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FileList()
        {
            var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
            var images = Directory.GetFiles(uploadsDir, "*.jpg");

            var fileListEntries = images.Select(image => new FileInfo(image)).Select(fileInfo => new FileListEntry
            {
                Name = fileInfo.Name,
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
