using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ImageSharp;
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
            var adminModel = _imageManager.FileList<AdminModel>(page ?? 1, 10);
            adminModel.RotationTime = _slideshowHandler.RotationMs;
            adminModel.ExportDirectories = GetDirectories();
            return View(adminModel);
        }

        ///<summary>
        /// Get possible export directories
        ///</summary>
        private string[] GetDirectories()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var mediaDir = Directory.GetDirectories("/Volumes")[0];
                return Directory.GetDirectories(mediaDir);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // This only works on Ubuntu so far
                var mediaDir = Directory.GetDirectories("/media")[0];
                return Directory.GetDirectories(mediaDir);  
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return DriveInfo.GetDrives().Select(d => d.Name).ToArray();
            }

            return new string[0];
        }

        public class ImageExportRequest
        {
            // Path to export the images to
            public string Path { get; set; }

            // Images that shall be exported. Can be null to export all
            public string[] IncludedImages { get; set; }
        }

        [HttpPost]
        public IActionResult Export([FromBody]ImageExportRequest exportRequest)
        {
            var images = _imageManager.GetRange(0, _imageManager.ImageCount);

            foreach (var image in images)
            {
                // Check if image should be exported
                if(exportRequest.IncludedImages != null && !exportRequest.IncludedImages.Contains(image.Id))
                    continue;

                var original = image.Versions.First(a => a.Version == ImageVersions.Original);
                var fileName = Path.Combine(exportRequest.Path, image.UploadDate.ToString("yyyyMMdd-hhmmss_fff") + ".jpg");
                
                using (var img = _imageManager.Open(original.ImageHash))
                using (var fs = new FileStream(fileName, FileMode.CreateNew))
                {
                    Image.Load(img).SaveAsJpeg(fs);
                }
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult RotationTime([FromBody]int seconds)
        {
            _slideshowHandler.RotationMs = seconds*1000;
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            _imageManager.Delete(id);
            return Ok();
        }

        public class AdminModel : FileList
        {
            public int RotationTime { get; set; }

            public string[] ExportDirectories { get; set; }

            public override string PageUrlBuilder(int page)
            {
                return $"admin?page={page}";
            }
        }
    }
}