namespace PartyWifi.Server.DataModel
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}