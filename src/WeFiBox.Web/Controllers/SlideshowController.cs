using System.IO;
using FileHelper = System.IO.File;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace WeFiBox.Web.Controllers
{
    public class FileId
    {
        public FileId(string file)
        {
            File = file;
        }

        public string File { get; }
    }

    public class SlideshowController : Controller
    {
        private readonly DirectoryConfigs _directories;

        public SlideshowController(IOptions<DirectoryConfigs> directories)
        {
            _directories = directories.Value;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var files = AllFiles();
            var latestFile = files[files.Length - 1];

            return View(new FileId(latestFile));
        }

        /// <summary>
        /// Get name of next image
        /// </summary>
        public IActionResult Next(string id)
        {
            var files = AllFiles();
            // Find current index in all files
            var currentIndex = 0;
            for (int index = 0; index  < files.Length; index ++)
            {
                if(files[index] != id)
                    continue;
                currentIndex = index;
                break;
            }

            // Return next in array or select random from older photos
            var nextIndex = currentIndex + 1;
            if (nextIndex >= files.Length)
            {
                var rand = new Random();
                nextIndex = rand.Next(currentIndex);
            }
            return Json(new FileId(files[nextIndex]));            
        }

        /// <summary>
        /// Get next image based on current id
        /// </summary>
        public IActionResult Image(string id)
        {           
            // Check if file has a resized clone, otherwise use original
            var filePath = Path.Combine(_directories.ResizedDir, id);
            if(!FileHelper.Exists(filePath))
            {
                filePath = Path.Combine(_directories.UploadDir, id);
            }

            // Return file stream
            var stream = new FileStream(filePath, FileMode.Open);
            return File(stream, "image/jpeg");
        }

        /// <summary>
        /// Get all files in the UploadDir in chronological order
        /// </summary>
        private string[] AllFiles()
        {
            var files = Directory.EnumerateFiles(_directories.UploadDir);
            if(_directories.AlphabeticalFileSystem)
            {
                files = files.OrderByDescending(f => f);
            }
            return files.Select(Path.GetFileName).ToArray();
        }
    }
}
