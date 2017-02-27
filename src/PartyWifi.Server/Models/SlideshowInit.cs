using System;

namespace PartyWifi.Server.Models
{
    /// <summary>
    /// Initialize the slideshow UI
    /// </summary>
    public class SlideshowInit
    {
        public SlideshowInit(string file, double rotationMs)
        {
            File = file;
            RotationMs = (int)Math.Ceiling(rotationMs);
        }

        /// <summary>
        /// Initial file of the cast
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Rotation time between images in milliseconds
        public int RotationMs { get; }
    }
}