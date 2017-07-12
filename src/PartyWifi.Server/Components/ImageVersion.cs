namespace PartyWifi.Server.Components
{
    ///<summary>
    /// Versions of an image
    ///</summary>
    public enum ImageVersions
    {
        ///<summary>
        /// Original version of the image
        ///</summary>
        Original = 1 << 8,

        ///<summary>
        /// Resized version of the image
        ///</summary>
        Resized = 1 << 16,

        ///<summary>
        /// Thumbnail version of the image
        ///</summary>
        Thumbnail = 1 << 24  
    }

    ///<summary>
    /// Versions of the image
    ///</summary>
    public class ImageVersion
    {
        ///<summary>
        /// Create new image version
        ///</summary>
        public ImageVersion(ImageVersions version, long size, string hash)
        {
            Version = version;
            Size = size;
            Hash = hash;
        }

        ///<summary>
        /// Hashcode of this version
        ///</summary>
        public ImageVersions Version { get; }

        ///<summary>
        /// Hashcode of this version
        ///</summary>
        public string Hash { get; }

        /// <summary>
        /// Size of the version
        /// </summary>
        public long Size { get; }
    }
}