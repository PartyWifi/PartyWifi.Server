using PartyWifi.Server.Components;

namespace PartyWifi.Server.Models
{
    public class ImageVersionResponse
    {
        public ImageVersions Version { get; set; }

        public long Size { get; set; }

        public string Hash { get; set; }
    }
}