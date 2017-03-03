using System;
using System.IO;
using System.Linq;
using System.Globalization;

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
        /// Hash name of the thumbnail version
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// Save this image to the file system at given directory
        /// </summary>
        public void SaveTo(string directory)
        {
            var fileName = Path.Combine(directory, Id);
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.WriteLine($"{nameof(Size)}: {Size}");
                writer.WriteLine($"{nameof(IsApproved)}: {IsApproved}");
                writer.WriteLine($"{nameof(UploadDate)}: {UploadDate:yyyyMMdd-HHmmss}");
                writer.WriteLine($"{nameof(Original)}: {Original}");
                writer.WriteLine($"{nameof(Resized)}: {Resized}");
                writer.WriteLine($"{nameof(Thumbnail)}: {Thumbnail}");
            }
        }

        public static ImageInfo FromFile(string filePath)
        {
            var id = Path.GetFileName(filePath);
            var lines = File.ReadAllLines(filePath);

            var image = new ImageInfo { Id = id };

            // Read size
            var value = LineValue(nameof(Size), lines);
            image.Size = int.Parse(value);

            // IsApproved
            value = LineValue(nameof(IsApproved), lines);
            image.IsApproved = bool.Parse(value);

            // UploadDate
            value = LineValue(nameof(UploadDate), lines);
            image.UploadDate = DateTime.ParseExact(value, "yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);

            // Load different image hashes
            image.Original = LineValue(nameof(Original), lines);
            image.Resized = LineValue(nameof(Resized), lines);
            image.Thumbnail = LineValue(nameof(Thumbnail), lines);

            return image;
        }

        private static string LineValue(string property, string[] lines)
        {
            var line = lines.First(l => l.StartsWith(property));
            var value = line.Split(':')[1];
            return value.Trim();
        }
    }
}