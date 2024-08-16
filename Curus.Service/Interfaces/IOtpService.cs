namespace Curus.Service.Interfaces;

public interface IOtpService
{
    Task StoreOtpAsync(int userId, string otp, TimeSpan expiration);
    Task<bool> ValidateOtpAsync(int userId, string otp);
}