using System;
using System.IO;

namespace PartyWifi.Server.Components
{
    /// <summary>
    /// Data access class for managing images
    /// </summary>
    public class ImageInfo
    {
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
        /// Hash name of the original
        /// </summary>
        public string Original { get; set; }

        /// <summary>
        /// Hash name of the resized version
        /// </summary>
        public string Resized { get; set; }

        /// <summary>
        /// Save this image to the file system at given directory
        /// </summary>
        public void SaveTo(string directory)
        {
            var fileName = Path.Combine(directory, Id);
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.WriteLine($"Size: {Size}");
                writer.WriteLine($"IsApproved: {IsApproved}");
                writer.WriteLine($"UploadDate: {UploadDate:yyyyMMdd-HHmmss}");
                writer.WriteLine($"Original: {Original}");
                writer.WriteLine($"Resized: {Resized}");
            }
        }

        public static ImageInfo FromFile(string filePath)
        {
            var id = Path.GetFileName(filePath);

            return new ImageInfo
            {
                Id = id
            };
        }
    }
}