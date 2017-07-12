using System;
using System.Collections.Generic;

namespace PartyWifi.Server.Components
{
    /// <summary>
    /// Data access class for managing images
    /// </summary>
    public class ImageInfo
    {
        /// <summary>
        /// Prepare <see cref="ImageInfo" /> and initialize versions
        /// </summary>
        public ImageInfo(string identifier)
        {
            Identifier = identifier;
            Versions = new List<ImageVersion>();
        }

        /// <summary>
        /// Unique id of the image
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Flag if image was approved
        /// </summary>
        public bool IsApproved => ImageState.HasFlag(ImageState.Approved);

        /// <summary>
        /// Flag if the image was deleted
        /// </summary>
        public bool IsDeleted => ImageState.HasFlag(ImageState.Deleted);

        /// <summary>
        /// Flag if the image was hidden
        /// </summary>
        public bool IsHidden => ImageState.HasFlag(ImageState.Hidden);

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