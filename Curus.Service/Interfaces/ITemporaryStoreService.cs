using Curus.Repository.ViewModels;

namespace Curus.Service.Interfaces;

public interface ITemporaryStoreService
{
    Task StoreInstructorRegistrationAsync(int userId, InstructorRegistrationDTO registrationDto, TimeSpan expiration);
    Task<InstructorRegistrationDTO> GetInstructorRegistrationAsync(int userId);
}