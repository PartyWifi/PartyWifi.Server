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

        private int _latestImageIndex;

        public SlideshowHandler(IOptions<Settings> settings, IImageManager imageManager)
        {
            _settings = settings.Value;
            _imageManager = imageManager;
        }
        
        public void Initialize()
        {
            _latestImageIndex = _imageManager.ImageCount - 1;
            RotationMs = _settings.ImageRotationSec * 1000;
        }

        public ImageInfo Next()
        {
            // Return next in array or select random from older photos
            var nextIndex = _latestImageIndex + 1;
            if (nextIndex >= _imageManager.ImageCount)
            {
                var rand = new Random();
                nextIndex = rand.Next(_imageManager.ImageCount);
            }
            else
            {
                _latestImageIndex = nextIndex;
            }

            var info = _imageManager.Get(nextIndex);
            return info;
        }
    }
}