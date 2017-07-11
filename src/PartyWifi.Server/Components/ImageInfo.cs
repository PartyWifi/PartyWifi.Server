using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;

namespace PartyWifi.Server.Components
{
    /// <summary>
    /// Data access class for managing images
    /// </summary>
    public class ImageInfo
    {
        public const string InfoExtension = ".json";

        /// <summary>
        /// Prepare <see cref="ImageInfo" /> and initialize versions
        /// </summary>
        public ImageInfo()
        {
            Versions = new List<ImageVersion>();
        }

        /// <summary>
        /// Unique id of the image
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Size of the original
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Flag if image was approved for slideshow
        /// </summary>
        public bool IsApproved => ImageState.HasFlag(ImageState.Approved);

        /// <summary>
        /// Flag if the image was deleted
        /// </summary>
        public bool IsDeleted => ImageState.HasFlag(ImageState.Deleted);

        /// <summary>
        /// Time-stamp of the image upload
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// Current state of the image
        /// </summary>
        public ImageState ImageState { get; set; }

        ///<summary>
        /// All versions of this image
        ///</summary>
        public List<ImageVersion> Versions { get; set; }
    }
}