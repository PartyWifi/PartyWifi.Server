namespace PartyWifi.Server.DataModel
{
    public class ImageVersionEntity : IEntity
    {
        public long Id { get; set; }

        public int Version { get; set; }

        public string ImageHash { get; set; }
    }
}