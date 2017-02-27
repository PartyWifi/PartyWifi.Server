namespace PartyWifi.Server
{
    /// <summary>
    /// Settings of the application
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Directory for the original uploaded images
        /// </summary>
        public string OriginalsDir { get; set; }

        /// <summary>
        /// Directory for resized photos on local file system
        /// </summary>
        public string ResizedDir {get; set;}

        /// <summary>
        /// Maximum height of images
        /// </summary>
        public int MaxHeight { get; set; }

        /// <summary>
        /// Maximum width of images
        /// </summary>
        public int MaxWidth { get; set; }

        /// <summary>
        /// Image rotation time in seconds
        /// </summary>
        public double ImageRotationSec { get; set; }
    }
}