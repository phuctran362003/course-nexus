using Curus.Repository.Interfaces;
using Curus.Repository.Interfaces.UoW;

namespace Curus.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly IUserRepository _userRepository;


    public UnitOfWork(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public IUserRepository UserRepository
    {
        get { return _userRepository; }
    }
}