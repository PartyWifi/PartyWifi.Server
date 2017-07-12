namespace PartyWifi.Server.Models
{
    public class ImageListResponse
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public int Total { get; set; }

        public ImageEntryResponse[] Images { get; set; }
    }
}