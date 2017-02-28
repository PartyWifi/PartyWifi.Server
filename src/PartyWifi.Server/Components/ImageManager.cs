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
            _images = new List<ImageInfo>();

            foreach (var resizedPath in Directory.EnumerateFiles(_settings.ResizedDir))
            {
                var imageName = Path.GetFileName(resizedPath);
                var imageId = Path.GetFileNameWithoutExtension(imageName);

                var originalPath = Path.Combine(_settings.OriginalsDir, imageName);
                var fileInfo = new FileInfo(resizedPath);

                var info = new ImageInfo
                {
                    Id = imageId,
                    Name = imageName,
                    OriginalPath = originalPath,
                    ResizedPath = resizedPath,
                    Size = fileInfo.Length,
                    UploadDate = fileInfo.CreationTime,
                };

                _images.Add(info);
            }
        }

        public int ImageCount => _images.Count;

        public ImageInfo Get(int index)
        {
            var info = _images[index];
            return info;
        }

        public ImageInfo Get(string imageId)
        {
            var info = _images.First(i => i.Id.Equals(imageId));
            return info;
        }

        public ImageInfo[] GetRange(int start, int count)
        {
            return _images.Skip(start).Take(count).ToArray();
        }

        public async Task Add(Stream stream)
        {
            // Create unambiguous filename
            var imageId = Guid.NewGuid().ToString();
            var imageName = $"{imageId}.jpg";

            var now = DateTime.Now;

            // Copy to originals for the customer to take home
            var originalPath = Path.Combine(_settings.OriginalsDir, imageName);
            await SaveFromStream(stream, originalPath);
            File.SetLastWriteTime(originalPath, now);

            // Resize for the slide-show
            var resizedStream = ResizeIfNecessary(stream);

            // Save to our hidden 'resized' directory for the slideshow
            var resizedPath = Path.Combine(_settings.ResizedDir, imageName);
            await SaveFromStream(resizedStream, resizedPath);
            File.SetLastWriteTime(resizedPath, now);

            // Create image model
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
                {   
                    // Reset and return current stream
                    memoryStream.Position = 0;
                    return memoryStream;
                }

                // Find the dimension that needs the most adjustment and create new dimensions
                var scaling = widthScale > heightScale ? widthScale : heightScale;
                var newWidth = (int)Math.Floor(image.Width / scaling);
                var newHeight = (int)Math.Floor(image.Height / scaling);

                // Reset stream and reuse it
                memoryStream.SetLength(0);
                image.Resize(newWidth, newHeight)
                     .Save(memoryStream);
                memoryStream.Position = 0;

                return memoryStream;
            }
        }

        public event EventHandler<ImageInfo> Added;

        private void RaiseAdded(ImageInfo info)
        {
            Added?.Invoke(this, info);
        }
    }
}