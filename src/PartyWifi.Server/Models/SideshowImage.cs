using System;

namespace PartyWifi.Server.Models
{
    /// <summary>
    /// Initialize the slideshow UI
    /// </summary>
    public class SideshowImage
    {
        public SideshowImage(string imageId, int rotationMs)
        {
            ImageId = imageId;
            RotationMs = rotationMs;
        }

        /// <summary>
        /// Initial file of the cast
        /// </summary>
        public string ImageId { get; }

        /// <summary>
        /// Rotation time between images in milliseconds
        /// </summary>
        public int RotationMs { get; }
    }
}