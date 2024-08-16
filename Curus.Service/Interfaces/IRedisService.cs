namespace Curus.Service.Interfaces;

public interface IRedisService
{
    Task SetStringAsync(string key, string value, TimeSpan expiry);
    Task<string> GetStringAsync(string key);
    Task<bool> DeleteKeyAsync(string key);
}
