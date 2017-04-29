using System;

namespace PartyWifi.Server.Models
{
    /// <summary>
    /// Model that represents a single file
    /// </summary>
    public class FileListEntry
    {
        /// <summary>
        /// GUID of the image
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Id of the image in bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Timestamp of the image
        /// </summary>
        public DateTime UploadDate { get; set; }
    }
}