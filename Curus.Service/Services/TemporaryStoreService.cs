using System;
using System.Threading.Tasks;
using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;

public class TemporaryStoreService : ITemporaryStoreService
{
    private readonly IMemoryCache _cache;

    public TemporaryStoreService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task StoreInstructorRegistrationAsync(int userId, InstructorRegistrationDTO registrationDto, TimeSpan expiration)
    {
        _cache.Set(userId, registrationDto, expiration);
        await Task.CompletedTask;
    }

    public async Task<InstructorRegistrationDTO> GetInstructorRegistrationAsync(int userId)
    {
        _cache.TryGetValue(userId, out InstructorRegistrationDTO registrationDto);
        return await Task.FromResult(registrationDto);
    }
}