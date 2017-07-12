using System;
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
        /// Get image by current index
        /// </summary>
        ImageInfo Get(int index);

        /// <summary>
        /// Returns the number of managed images 
        /// </summary>
        /// <returns></returns>
        int ImageCount();

        /// <summary>
        /// Get image by identifier
        /// </summary>
        ImageInfo GetByIdentifier(string identifier);

        /// <summary>
        /// Get a number of images for paging
        /// </summary>
        ImageInfo[] GetRange(int start, int limit);

        /// <summary>
        /// Upload a new image
        /// </summary>
        Task<ImageInfo> Add(Stream stream);

        /// <summary>
        /// Open image stream with this hash
        /// </summary>
        Stream Open(string hash);

        /// <summary>
        /// Remove image
        /// </summary>
        Task Delete(string identifier);

        /// <summary>
        /// Event raised when an image was Added
        /// </summary>
        event EventHandler<ImageInfo> Added;

        /// <summary>
        /// Event raised when an image was Updated
        /// </summary>
        event EventHandler<ImageInfo> Updated;
    }
}