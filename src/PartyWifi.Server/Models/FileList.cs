namespace PartyWifi.Server.Models
{
    /// <summary>
    /// Model for the file list containing the current page, all pages
    /// and the images for this page.
    /// </summary>
    public class FileList
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
    }
}