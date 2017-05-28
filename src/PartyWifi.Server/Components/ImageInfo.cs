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
        public bool IsApproved { get; set; }

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

        /// <summary>
        /// Save this image to the file system at given directory
        /// </summary>
        public void SaveTo(string directory)
        {
            var fileName = Path.Combine(directory, $"{Id}{InfoExtension}");
            var json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
            File.WriteAllText(fileName, json);
        }

        ///<summary>
        /// Load <see cref="ImageInfo" /> from given file
        ///</summary>
        public static ImageInfo FromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<ImageInfo>(json);
        }
    }
}