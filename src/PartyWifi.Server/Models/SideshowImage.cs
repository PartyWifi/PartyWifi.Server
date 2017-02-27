using System;

namespace PartyWifi.Server.Models
{
    /// <summary>
    /// Initialize the slideshow UI
    /// </summary>
    public class SideshowImage
    {
        public SideshowImage(string file, int rotationMs)
        {
            File = file;
            RotationMs = rotationMs;
        }

        /// <summary>
        /// Initial file of the cast
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Rotation time between images in milliseconds
        /// </summary>
        public int RotationMs { get; }
    }
}