namespace PartyWifi.Server.Models
{
    public class ImageInfoResponse : ImageEntryResponse
    {
        public ImageVersionResponse[] Versions { get; set; }
    }
}