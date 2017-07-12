using Microsoft.Extensions.Options;

namespace PartyWifi.Server.Components
{
    public class SlideshowHandler : ISlideshowHandler
    {
        public int RotationMs { get; set; }

        private readonly Settings _settings;
        private readonly IImageManager _imageManager;

        // Most recent picture that was shown in the presentation
        private int _latestImageIndex;
        // Current image displayed by clients
        private int _currentImageIndex;

        public SlideshowHandler(IOptions<Settings> settings, IImageManager imageManager)
        {
            _settings = settings.Value;
            _imageManager = imageManager;
        }
        
        public void Initialize()
        {
            //_latestImageIndex = _imageManager.ImageCount - 1;
            RotationMs = _settings.ImageRotationSec * 1000;
        }

        public ImageInfo Next()
        {
            // First check if any new fotos were added
            var nextIndex = _latestImageIndex + 1;
            //if (nextIndex < _imageManager.ImageCount)
            //{
            //    _latestImageIndex = nextIndex;
            //    return _imageManager.Get(nextIndex);
            //}

            // Otherwise keep iterating through the older photos
            nextIndex = _currentImageIndex + 1;
            if (nextIndex > _latestImageIndex)
            {
                // We reached the end, so start again
                _currentImageIndex = nextIndex = 0;
            }
            else
            {
                _currentImageIndex = nextIndex;
            }
            return _imageManager.Get(nextIndex);
        }
    }
}