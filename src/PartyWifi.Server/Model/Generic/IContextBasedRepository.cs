using Microsoft.EntityFrameworkCore;

namespace PartyWifi.Server.Model
{
    internal interface IContextBasedRepository
    {
        void SetContext(IUnitOfWork uow, DbContext context);
    }
}