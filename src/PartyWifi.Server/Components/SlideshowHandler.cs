using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using PartyWifi.Server.Models;

namespace PartyWifi.Server.Components
{
    public class SlideshowHandler : ISlideshowHandler
    {
        private readonly Settings _settings;
        private readonly IImageManager _imageManager;

        private ImageInfo _lastImage;
        private int _rotationMs;

        public SlideshowHandler(IOptions<Settings> settings, IImageManager imageManager)
        {
            _settings = settings.Value;
            _imageManager = imageManager;
        }

        public void Initialize()
        {
            _lastImage = GetLastImage();
            _rotationMs = _settings.ImageRotationSec * 1000;
        }

        public void SetRefreshTime(int seconds)
        {
            _rotationMs = seconds * 1000;
        }

        private ImageInfo GetLastImage()
        {
            return _imageManager.GetAll().Last();
        }

        public SideshowImage GetInitial()
        {
            var last = _imageManager.GetAll().Last();
            return new SideshowImage(last.Name, _rotationMs);
        }

        public SideshowImage Next()
        {
            //TODO: use timeer to load next image

            //TODO: find better way to get and do not iterate over all images
            var files = _imageManager.GetAll().Select(i => i.Name).ToArray();

            // Find current index in all files starting from the back
            var currentIndex = 0;
            for (var index = files.Length - 1; index >= 0; index--)
            {
                if (files[index] != _lastImage.Name)
                    continue;
                currentIndex = index;
                break;
            }

            // Return next in array or select random from older photos
            var nextIndex = currentIndex + 1;
            if (nextIndex >= files.Length)
            {
                var rand = new Random();
                nextIndex = rand.Next(files.Length);
            }

            var info = _imageManager.Get(files[nextIndex]);
            return new SideshowImage(info.Name, _rotationMs);
        }
    }
}