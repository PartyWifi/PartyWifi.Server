using System;

namespace PartyWifi.Server.Models
{
    public class ImageEntryResponse
    {
        public string Identifier { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsHidden { get; set; }

        public bool IsApproved { get; set; }

        public DateTime UploadDate { get; set; }
    }
}