using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp;
using Microsoft.Extensions.Options;

namespace PartyWifi.Server.Components
{
    public class ImageManager : IImageManager
    {
        private readonly Settings _settings;
        private List<ImageInfo> _images;

        public ImageManager(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public void Initialize()
        {
            var images = Directory.EnumerateFiles(_settings.ResizedDir).Select(Path.GetFileName).ToArray();
            var imageInfos = new List<ImageInfo>();

            foreach (var imageName in images)
            {
                var imageId = Path.GetFileNameWithoutExtension(imageName);

                var resizedPath = Path.Combine(_settings.ResizedDir, imageName);
                var fileInfo = new FileInfo(resizedPath);

                var info = new ImageInfo
                {
                    Id = imageId,
                    Name = imageName,
                    OriginalPath = Path.Combine(_settings.OriginalsDir, imageName),
                    ResizedPath = resizedPath,
                    Size = fileInfo.Length,
                    UploadDate = fileInfo.CreationTime,
                };

                imageInfos.Add(info);
            }

            _images = new List<ImageInfo>(imageInfos);
        }

        public IEnumerable<ImageInfo> GetAll()
        {
            return _images.OrderBy(i => i.UploadDate);
        }

        public ImageInfo Get(string imageId)
        {
            var info = _images.First(i => i.Id.Equals(imageId));
            return info;
        }

        public async Task Add(Stream stream)
        {
            // Create unambiguous filename
            var imageId = Guid.NewGuid().ToString();
            var imageName = $"{imageId}.jpg";

            var now = DateTime.Now;

            // Copy to filesystem for later
            var originalPath = Path.Combine(_settings.OriginalsDir, imageName);
            await SaveFromStream(stream, originalPath);
            File.SetLastWriteTime(originalPath, now);

            // Resize for the slide-show
            var resizedStream = ResizeIfNecessary(stream);

            // Rename file to make it accessible to slideshow
            var resizedPath = Path.Combine(_settings.ResizedDir, imageName);
            await SaveFromStream(resizedStream, resizedPath);
            File.SetLastWriteTime(resizedPath, now);

            var info = new ImageInfo
            {
                Id = imageId,
                Name = imageName,
                OriginalPath = originalPath,
                ResizedPath = resizedPath,
                UploadDate = now,
                Size = resizedStream.Length
            };

            _images.Add(info);
            RaiseAdded(info);
        }

        /// <summary>
        /// Save the given stream to a file path
        /// </summary>
        private static async Task SaveFromStream(Stream stream, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
                fileStream.Flush();
                stream.Position = 0;
            }
        }

        /// <summary>
        /// Create a resized version of the image if necessary
        /// </summary>
        private Stream ResizeIfNecessary(Stream memoryStream)
        {
            using (var image = new Image(memoryStream))
            {
                var widthScale = image.Width / (double)_settings.MaxWidth;
                var heightScale = image.Height / (double)_settings.MaxHeight;

                if (widthScale <= 1 && heightScale <= 1)
                    return memoryStream;

                // Find the dimension that needs the most adjustment and create new dimensions
                var scaling = widthScale > heightScale ? widthScale : heightScale;
                var newWidth = (int)Math.Floor(image.Width / scaling);
                var newHeight = (int)Math.Floor(image.Height / scaling);

                image.Resize(newWidth, newHeight);

                var resizedStream = new MemoryStream();

                //Resave in same stream
                image.Save(resizedStream);
                resizedStream.Position = 0;

                return resizedStream;
            }
        }

        public event EventHandler<ImageInfo> Added;
        private void RaiseAdded(ImageInfo info)
        {
            Added?.Invoke(this, info);
        }
    }
}