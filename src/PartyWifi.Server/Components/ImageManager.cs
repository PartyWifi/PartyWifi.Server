using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using ImageSharp;
using Microsoft.Extensions.Options;
using System.Text;
using ImageSharp.Processing;

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
            lock(_images) 
                return _images[index];
        }

        public ImageInfo Get(string imageId)
        {
            lock(_images) 
                return _images.First(i => i.Id.Equals(imageId));
        }

        public ImageInfo[] GetRange(int start, int count)
        {
            lock(_images) 
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

            // Check orientation - iOS devices do not modify the image itself (only exif data is changed)
            RotateIfNecessary(stream);

            // Save original for customer to take home
            var original = await SaveFromStream(stream);
            info.Versions.Add(new ImageVersion(ImageVersions.Original, original));

            // Resize for the slide-show if image is too big
            var resized = original;
            if (ResizeIfNecessary(stream, _settings.MaxWidth, _settings.MaxHeight))
                resized = await SaveFromStream(stream);
  
            info.Versions.Add(new ImageVersion(ImageVersions.Resized, resized));

            // Resize for the thumbnail
            var thumbnail = original;
            if (ResizeIfNecessary(stream, 150, 150))
                thumbnail = await SaveFromStream(stream);

            info.Versions.Add(new ImageVersion(ImageVersions.Thumbnail, thumbnail));

            // Add and publish
            lock (_images)
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
        /// Resizes the image if necessary. 
        /// iOS devices do not modify the image itself (only exif data is changed)
        /// </summary>
        private static void RotateIfNecessary(Stream memoryStream)
        {
            using (var image = Image.Load(memoryStream))
            {
                var exifProfile = image.MetaData.ExifProfile;
                // Not all images include exif data
                if (exifProfile == null)
                {
                    memoryStream.Position = 0;
                    return;
                }

                var orientation = exifProfile.GetValue(ExifTag.Orientation);
                // Orientation value is not set by all devices, in that case ignore
                if(orientation?.Value == null || (ushort)orientation.Value == 1)
                {
                    memoryStream.Position = 0;
                    return;
                }

                // Apply orientation to image
                switch ((ushort)orientation.Value)
                {
                    case 3: // Rotate 180
                        image.Rotate(RotateType.Rotate180);
                        break;
                    case 6: // Rotate 90
                        image.Rotate(RotateType.Rotate90);
                        break;
                    case 8: // Rotate 270
                        image.Rotate(RotateType.Rotate270);
                        break;
                }

                exifProfile.SetValue(ExifTag.Orientation, (ushort)1);

                SaveAndReuseStream(memoryStream, image);
            }
        }

        /// <summary>
        /// Create a resized version of the image if necessary
        /// </summary>
        /// <returns>
        /// True if image was resized, otherwise false
        /// </returns>
        private static bool ResizeIfNecessary(Stream memoryStream, int width, int height)
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

                image.Resize(newWidth, newHeight);

                SaveAndReuseStream(memoryStream, image);

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

        public void Delete(string id)
        {
            lock(_images)
            {
                // Delete from list
                var img = _images.First(i => i.Id == id);
                _images.Remove(img);

                // Move file
                var removedDir = Path.Combine(_settings.Directory, "removed");
                if(!Directory.Exists(removedDir))
                    Directory.CreateDirectory(removedDir);

                var oldPath = Path.Combine(_settings.Directory, id + ImageInfo.InfoExtension);
                var newPath = Path.Combine(removedDir, id + ImageInfo.InfoExtension);
                File.Move(oldPath, newPath);
            }
        }

        private static void SaveAndReuseStream(Stream memoryStream, Image<Rgba32> image)
        {
            memoryStream.SetLength(0);
            image.Save(memoryStream);
            memoryStream.Position = 0;
        }

        public event EventHandler<ImageInfo> Added;

        private void RaiseAdded(ImageInfo info)
        {
            Added?.Invoke(this, info);
        }
    }
}