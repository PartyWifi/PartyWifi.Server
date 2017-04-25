using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using ImageSharp;
using Microsoft.Extensions.Options;
using System.Text;

namespace PartyWifi.Server.Components
{
    public class ImageManager : IImageManager
    {
        private const string ImageDirectory = "img";

        private readonly Settings _settings;

        private string _imgDir;

        private List<ImageInfo> _images;

        public ImageManager(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public void Initialize()
        {
            // Prepare 'img' directory if it does not exist
            _imgDir = Path.Combine(_settings.Directory, ImageDirectory);
            if (!Directory.Exists(_imgDir))
            {
                Directory.CreateDirectory(_imgDir);
            }

            // Restore image objects from meta files
            _images = Directory.EnumerateFiles(_settings.Directory)
                               .Select(ImageInfo.FromFile)
                               .OrderBy(img => img.UploadDate).ToList();
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
            // Prepare model
            var id = Guid.NewGuid().ToString();
            var info = new ImageInfo
            {
                Id = id,
                Size = stream.Length,
                UploadDate = DateTime.Now
            };

            // Save original for customer to take home
            var original = await SaveFromStream(stream);
            info.Versions.Add(new ImageVersion(ImageVersions.Original, original));
            
            // Resize for the slide-show if image is too big
            var resized = original;
            if (ResizeIfNecessary(stream, _settings.MaxWidth, _settings.MaxHeight))
            {
                resized = await SaveFromStream(stream);
                info.Versions.Add(new ImageVersion(ImageVersions.Resized, resized));
            }
            else
            {
                info.Versions.Add(new ImageVersion(ImageVersions.Resized, original));
            }

            // Resize for the thumbnail
            var thumbnail = original;
            if(ResizeIfNecessary(stream, 150, 150))
            {
                thumbnail = await SaveFromStream(stream);
                info.Versions.Add(new ImageVersion(ImageVersions.Thumbnail, thumbnail));
            }

            // Add and publish
            _images.Add(info);
            RaiseAdded(info);

            // Save info as well
            info.SaveTo(_settings.Directory);
        }

        /// <summary>
        /// Save the given stream to a file path
        /// </summary>
        private async Task<string> SaveFromStream(Stream stream)
        {
            // Create hash and convert to name
            string name;
            using (var hashing = SHA1.Create())
            {
                var hash = hashing.ComputeHash(stream);
                var nameBuilder = new StringBuilder(hash.Length * 2);
                foreach(var hashByte in hash)
                {
                    nameBuilder.AppendFormat("{0:X2}", hashByte);
                }
                name = nameBuilder.ToString();

                stream.Position = 0;
            }
            
            // Subdirectory for first two characters
            var subDir = Path.Combine(_imgDir, name.Substring(0, 2));
            if (!Directory.Exists(subDir))
            {
                Directory.CreateDirectory(subDir);
            }

            // Rest is name of file
            var fileName = Path.Combine(subDir, name.Substring(2));
            if (File.Exists(fileName))
                return name;  // File already exists

            // Write to file
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
                fileStream.Flush();
                stream.Position = 0;
            }

            return name;
        }

        /// <summary>
        /// Create a resized version of the image if necessary
        /// </summary>
        /// <returns>
        /// True if image was resized, otherwise false
        /// </returns>
        private bool ResizeIfNecessary(Stream memoryStream, int width, int height)
        {
            using (var image = Image.Load(memoryStream))
            {
                var widthScale = image.Width / (double)width;
                var heightScale = image.Height / (double)height;

                if (widthScale <= 1 && heightScale <= 1)
                {   
                    // Reset and return current stream
                    memoryStream.Position = 0;
                    return false;
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

                return true;
            }
        }

        public Stream Open(string hash)
        {
            var subdir = hash.Substring(0, 2);
            var fileName = hash.Substring(2);
            var path = Path.Combine(_settings.Directory, ImageDirectory, subdir, fileName);
            
            return new FileStream(path, FileMode.Open);
        }

        public event EventHandler<ImageInfo> Added;

        private void RaiseAdded(ImageInfo info)
        {
            Added?.Invoke(this, info);
        }
    }
}