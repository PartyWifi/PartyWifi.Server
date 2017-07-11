using System;
using System.Collections.Generic;

namespace PartyWifi.Server.Model
{
    public class ImageEntity : IEntity
    {
        public ImageEntity()
        {
            Versions = new List<ImageVersionEntity>();
        }

        public long Id { get; set; }

        public string Identifier { get; set; }

        public long Size { get; set; }

        public int ImageState { get; set; }

        public DateTime UploadDate { get; set; }

        public List<ImageVersionEntity> Versions { get; set; }
    }
}