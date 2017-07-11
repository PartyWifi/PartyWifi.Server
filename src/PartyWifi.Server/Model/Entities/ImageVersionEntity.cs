namespace PartyWifi.Server.Model
{
    public class ImageVersionEntity : IEntity
    {
        public long Id { get; set; }

        public int Version { get; set; }

        public string Hash { get; set; }
    }
}