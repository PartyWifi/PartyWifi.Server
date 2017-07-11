using System;
using System.Collections.Generic;

namespace PartyWifi.Server.Model
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }

    public delegate IRepository RepoConstructor();

    public class RepositoryHolder : Dictionary<Type, RepoConstructor>
    {

    }
}