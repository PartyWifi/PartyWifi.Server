using System;

namespace PartyWifi.Server.Models
{
    public class FileListEntry
    {
        public string Id { get; set; }

        public long Size { get; set; }

        public string PublicUrl { get; set; }

        public DateTime UploadDate { get; set; }
    }
}