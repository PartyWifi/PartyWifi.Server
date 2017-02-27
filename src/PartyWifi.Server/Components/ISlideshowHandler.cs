using PartyWifi.Server.Models;

namespace PartyWifi.Server.Components
{
    public interface ISlideshowHandler
    {
        void Initialize();

        SideshowImage GetInitial();

        SideshowImage Next();

        void SetRefreshTime(int seconds);
    }
}