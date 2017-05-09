namespace PartyWifi.Server.Models
{
    /// <summary>
    /// Model for the file list containing the current page, all pages
    /// and the images for this page. This base class needs to be derived
    /// by each controller to set the url builder.
    /// </summary>
    public abstract class FileList
    {
        /// <summary>
        /// Current page
        /// </summary>
        public int CurrentPage { get; set; }
      
        /// <summary>
        /// All pages
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// Files on this page
        /// </summary>
        public FileListEntry[] Files { get; set; }

        /// <summary>
        /// Build the url to navigate to a page
        /// </summary>
        public abstract string PageUrlBuilder(int page);
    }
}