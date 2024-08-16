using AutoMapper;
using Curus.Repository.Entities;
using Curus.Repository.ViewModels;

namespace Curus.Repository.Helper;

public class ApplicationMapping : Profile
{
    public ApplicationMapping()
    {
        CreateMap<User, UserDTO>().ReverseMap();
    }
}