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
            var imageInfos = new ImageInfo[images.Length];

            for (var index = 0; index < images.Length; index++)
            {
                var imageName = images[index];
                var model = new ImageInfo
                {
                    Name = imageName,
                    OriginalPath = Path.Combine(_settings.OriginalsDir, imageName),
                    ResizedPath = Path.Combine(_settings.ResizedDir, imageName),
                };
                model.Size = new FileInfo(model.ResizedPath).Length;
                model.UploadDate = File.GetCreationTime(model.OriginalPath);

                imageInfos[index] = model;
            }

            _images = new List<ImageInfo>(imageInfos);
        }

        public IEnumerable<ImageInfo> GetAll()
        {
            return _images.OrderBy(i => i.UploadDate);
        }

        public Stream Get(string name)
        {
            var info = _images.First(i => i.Name.Equals(name));
            return Get(info);
        }

        public Stream Get(ImageInfo info)
        {
            return new FileStream(info.ResizedPath, FileMode.Open);
        }

        public async Task Add(Stream stream)
        {
            // Create unambiguous filename
            var fileName = $"{Guid.NewGuid()}.jpg";

            var now = DateTime.Now;

            // Copy to filesystem for later
            var originalPath = Path.Combine(_settings.OriginalsDir, fileName);
            await SaveFromStream(stream, originalPath);
            File.SetLastWriteTime(originalPath, now);

            // Resize for the slide-show
            var resizedStream = ResizeIfNecessary(stream);

            // Rename file to make it accessible to slideshow
            var resizedPath = Path.Combine(_settings.ResizedDir, fileName);
            await SaveFromStream(resizedStream, resizedPath);
            File.SetLastWriteTime(resizedPath, now);

            _images.Add(new ImageInfo
            {
                Name = fileName,
                OriginalPath = originalPath,
                ResizedPath = resizedPath,
                UploadDate = now,
                Size = resizedStream.Length
            });
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
    }
}