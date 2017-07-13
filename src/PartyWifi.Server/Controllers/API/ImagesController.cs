using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PartyWifi.Server.Components;
using PartyWifi.Server.Models;

namespace PartyWifi.Server.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IImageManager _manager;

        /// <summary>
        /// Creates a new instance of the <see cref="ImagesController"/>
        /// </summary>
        /// <param name="manager">Injected dependency of the image manager</param>
        public ImagesController(IImageManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// GET: /images?offset=2
        /// </summary>
        [HttpGet, Route("")]
        public IActionResult List([FromQuery]int offset, [FromQuery]int limit)
        {
            if (limit == 0)
                limit = 10;

            var total = _manager.ImageCount();
            var images = _manager.GetRange(offset, limit).Select(ImageEntrySelector<ImageEntryResponse>).ToArray();

            var result = new ImageListResponse
            {
                Limit = limit,
                Offset = offset,
                Total = total,
                Images = images
            };

            return Json(result);
        }

        /// <summary>
        /// Creates a new image from the posted form file
        /// POST: images/ (multipart)
        /// </summary>
        [HttpPost, Route("")]
        public async Task<IActionResult> Create()
        {
            var file = Request.Form.Files.SingleOrDefault();
            if (file == null)
                return BadRequest();

            ImageInfo info;
            using (var memoryStream = new MemoryStream())
            {
                // Copy to memory first
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                info = await _manager.Add(memoryStream);
            }

            return CreatedAtRoute(nameof(Read), new {identifier = info.Identifier}, info);
        }

        /// <summary>
        /// Reads a image by the identifier
        /// GET: images/{identifier}
        /// </summary>
        [HttpGet, Route("{identifier}", Name = nameof(Read))]
        public IActionResult Read(string identifier)
        {
            var info = _manager.GetByIdentifier(identifier);
            if (info == null)
                return NotFound();

            var imageInfo = ImageEntrySelector<ImageInfoResponse>(info);
            imageInfo.Versions = info.Versions.Select(v => new ImageVersionResponse
            {
                Hash = v.Hash,
                Size = v.Size,
                Version = v.Version
            }).ToArray();

            return Json(imageInfo);
        }

        [HttpDelete, Route("{identifier}")]
        public async Task<IActionResult> Delete(string identifier)
        {
            await _manager.Delete(identifier);
            return NoContent();
        }

        /// <summary>
        /// Load special version of an image
        /// </summary>
        [HttpGet, Route("{identifier}/{version}")]
        public IActionResult Load(ImageVersions version, string identifier)
        {
            var image = _manager.GetByIdentifier(identifier);
            if (image == null)
                return NotFound();

            var imageVersion = image.Versions.FirstOrDefault(v => v.Version == version);
            if (imageVersion == null)
                return NotFound();

            var stream = _manager.Open(imageVersion.Hash);
            return File(stream, "image/jpeg");
        }

        private static T ImageEntrySelector<T>(ImageInfo info)
            where T : ImageEntryResponse, new()
        {
            return new T
            {
                Identifier = info.Identifier,
                UploadDate = info.UploadDate,
                IsDeleted = info.IsDeleted,
                IsApproved = info.IsApproved,
                IsHidden = info.IsHidden
            };
        }
    }
}