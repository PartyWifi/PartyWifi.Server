namespace PartyWifi.Server
{
    /// <summary>
    /// Settings of the application
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Base Directory of our application
        /// </summary>
        public string Directory { get; set; }

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
        public int ImageRotationSec { get; set; }

        /// <summary>
        /// Number of images per page in the file list
        /// </summary>
        public int ImagesPerPage {get; set;}

        /// <summary>
        /// Automatically approve images
        /// </summary>
        public bool AutoApprove { get; set; }
    }
}