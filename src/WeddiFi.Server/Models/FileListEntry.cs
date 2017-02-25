using System;

namespace WeddiFi.Server.Models
{
    public class FileListEntry
    {
        public string Name { get; set; }

        public long Size { get; set; }

        public string PublicUrl { get; set; }

        public DateTime UploadDate { get; set; }
    }
}