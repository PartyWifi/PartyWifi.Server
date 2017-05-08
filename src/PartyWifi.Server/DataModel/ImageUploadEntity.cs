using System;
using System.Collections.Generic;

namespace PartyWifi.Server.DataModel
{
    public class ImageUploadEntity : IEntity
    {
        public long Id { get; set; }

        public string Identifier { get; set; }

        public long Size { get; set; }

        public bool IsApproved { get; set; }

        public DateTime UploadDate { get; set; }

        public List<ImageVersionEntity> Versions { get; set; }
    }
}