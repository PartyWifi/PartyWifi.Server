using System;

namespace PartyWifi.Server.Components
{
    /// <summary>
    /// Data access class for managing images
    /// </summary>
    public class ImageInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public bool IsApproved { get; set; }

        public string OriginalPath { get; set; }

        public string ResizedPath { get; set; }

        public DateTime UploadDate { get; set; }
    }
}