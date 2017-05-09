using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PartyWifi.Server.Components
{
    /// <summary>
    /// Central component responible for upload and loading
    /// of images
    /// </summary>
    public interface IImageManager
    {
        /// <summary>
        /// Load all images and prepare cache
        /// </summary>
        void Initialize();

        /// <summary>
        /// Total number of images hosted
        int ImageCount { get; }

        /// <summary>
        /// Get image at this index
        /// </summary>
        ImageInfo Get(int index);

        /// <summary>
        /// Get image by imageID
        /// </summary>
        ImageInfo Get(string imageId);

        /// <summary>
        /// Get a number of images for paging
        /// </summary>
        ImageInfo[] GetRange(int start, int count);

        /// <summary>
        /// Upload a new image
        /// </summary>
        Task Add(Stream stream);

        /// <summary>
        /// Open image stream with this hash
        /// </summary>
        Stream Open(string hash);

        /// <summary>
        /// Remove image
        /// </summary>
        void Delete(string id);

        /// <summary>
        /// Event raised when an image was Added
        /// </summary>
        event EventHandler<ImageInfo> Added;
    }
}
