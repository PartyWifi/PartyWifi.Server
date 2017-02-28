using System;
using System.Linq;
using Microsoft.Extensions.Options;

namespace PartyWifi.Server.Components
{
    public class SlideshowHandler : ISlideshowHandler
    {
        public int RotationMs { get; set; }

        private readonly Settings _settings;
        private readonly IImageManager _imageManager;

        private ImageInfo _lastImage;

        public SlideshowHandler(IOptions<Settings> settings, IImageManager imageManager)
        {
            _settings = settings.Value;
            _imageManager = imageManager;
        }
        
        public void Initialize()
        {
            _lastImage = _imageManager.GetAll().Last();
            RotationMs = _settings.ImageRotationSec * 1000;
        }

        public ImageInfo Next()
        {
            //TODO: use timer to load next image

            //TODO: find better way to get and do not iterate over all images
            var files = _imageManager.GetAll().Select(i => i.Id).ToArray();

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
            return info;
        }
    }
}