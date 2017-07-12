using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using ImageSharp;
using Microsoft.Extensions.Options;
using System.Text;
using System.Threading;
using PartyWifi.Server.Model;

namespace PartyWifi.Server.Components
{
    public class ImageManager : IImageManager
    {
        private const string ImageDirectory = "img";

        private readonly string _imgDir;
        private readonly IUnitOfWorkFactory _modelFactory;
        private readonly Settings _settings;

        private List<ImageInfo> _images;
        private readonly ReaderWriterLockSlim _imageLock = new ReaderWriterLockSlim();

        public ImageManager(IOptions<Settings> settings, IUnitOfWorkFactory modelFactory)
        {
            _modelFactory = modelFactory;
            _settings = settings.Value;
            _imgDir = Path.Combine(_settings.Directory, ImageDirectory);
        }

        public void Initialize()
        {
            // Prepare 'img' directory if it does not exist
            if (!Directory.Exists(_imgDir))
                Directory.CreateDirectory(_imgDir);

            // Restore image objects from database
            using (var uow = _modelFactory.Create())
                _images = new List<ImageInfo>(ImageStorage.GetAll(uow));
        }

        public ImageInfo Get(int index)
        {
            _imageLock.EnterReadLock();
            
            var result = _images.ElementAtOrDefault(index);

            _imageLock.ExitReadLock();

            return result;
        }

        public int ImageCount()
        {
            _imageLock.EnterReadLock();

            var result = _images.Count();

            _imageLock.ExitReadLock();

            return result;
        }

        public ImageInfo GetByIdentifier(string identifier)
        {
            _imageLock.EnterReadLock();

            var result =  _images.SingleOrDefault(i => i.Identifier.Equals(identifier));

            _imageLock.ExitReadLock();

            return result;
        }

        public ImageInfo[] GetRange(int start, int limit)
        {
            _imageLock.EnterReadLock();

            var result = _images.Skip(start).Take(limit).ToArray();

            _imageLock.ExitReadLock();

            return result;
        }

        public async Task<ImageInfo> Add(Stream stream)
        {
            // Prepare model
            var info = new ImageInfo(Guid.NewGuid().ToString())
            {
                UploadDate = DateTime.Now
            };

            // Check orientation - iOS devices do not modify the image itself (only exif data is changed)
            RotateIfNecessary(stream);

            // Save original for customer to take home
            var originalHash = await SaveFromStream(stream);
            info.Versions.Add(new ImageVersion(ImageVersions.Original, stream.Length, originalHash));

            // Resize for the slide-show if image is too big
            var resizedHash = originalHash;
            if (ResizeIfNecessary(stream, _settings.MaxWidth, _settings.MaxHeight))
                resizedHash = await SaveFromStream(stream);
  
            info.Versions.Add(new ImageVersion(ImageVersions.Resized, stream.Length, resizedHash));

            // Resize for the thumbnail
            var thumbnailHash = originalHash;
            if (ResizeIfNecessary(stream, 150, 150))
                thumbnailHash = await SaveFromStream(stream);

            info.Versions.Add(new ImageVersion(ImageVersions.Thumbnail, stream.Length, thumbnailHash));

            // Save info
            using (var uow = _modelFactory.Create())
            {
                ImageStorage.Add(uow, info);
                await uow.Save();
            }

            // Add and publish
            _imageLock.EnterWriteLock();
            _images.Add(info);
            _imageLock.ExitWriteLock();

            Added?.Invoke(this, info);

            return info;
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
                // Not all images include exif data
                var exifProfile = image.MetaData.ExifProfile;
                if (exifProfile == null)
                {
                    memoryStream.Position = 0;
                    return;
                }

                image.AutoOrient();
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

        public async Task Delete(string identifier)
        {
            _imageLock.EnterReadLock();

            // Delete from list
            var info = _images.First(i => i.Identifier == identifier);
            info.ImageState |= ImageState.Deleted;

            // Update database
            using (var uow = _modelFactory.Create())
            {
                await ImageStorage.Update(uow, info);
                await uow.Save();
            }

            _imageLock.ExitReadLock();

            Updated?.Invoke(this, info);
        }

        private static void SaveAndReuseStream(Stream memoryStream, Image<Rgba32> image)
        {
            memoryStream.SetLength(0);
            image.Save(memoryStream, ImageFormats.Jpeg);
            memoryStream.Position = 0;
        }

        public event EventHandler<ImageInfo> Added;

        public event EventHandler<ImageInfo> Updated;
    }
}