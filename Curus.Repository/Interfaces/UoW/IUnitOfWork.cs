using Azure.Core;

namespace Curus.Repository.Interfaces.UoW;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
}