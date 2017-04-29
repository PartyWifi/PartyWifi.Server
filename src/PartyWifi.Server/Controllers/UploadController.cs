using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PartyWifi.Server.Components;

namespace PartyWifi.Server.Controllers
{
    public class UploadController : Controller
    {
        private readonly IImageManager _imageManager;

        public UploadController(IImageManager imageManager)
        {
            _imageManager = imageManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFilesAjax()
        {
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Copy to memory first
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    await _imageManager.Add(memoryStream);
                }
            }

            return Ok();
        }
    }
}