using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Curus.Service.Interfaces;

public class OtpService : IOtpService
{
    private readonly ConcurrentDictionary<int, OtpEntry> _otpStore = new ConcurrentDictionary<int, OtpEntry>();

    public async Task StoreOtpAsync(int userId, string otp, TimeSpan expiration)
    {
        var otpEntry = new OtpEntry
        {
            Otp = otp,
            ExpiryTime = DateTime.UtcNow.Add(expiration)
        };
        _otpStore[userId] = otpEntry;

        // Simulate async operation
        await Task.CompletedTask;
    }

    public async Task<bool> ValidateOtpAsync(int userId, string otp)
    {
        if (_otpStore.TryGetValue(userId, out var otpEntry))
        {
            if (otpEntry.Otp == otp && otpEntry.ExpiryTime > DateTime.UtcNow)
            {
                // OTP is valid, remove it from the store
                _otpStore.TryRemove(userId, out _);
                return await Task.FromResult(true);
            }
        }

        return await Task.FromResult(false);
    }
}



public class OtpEntry
{
    public string Otp { get; set; }
    public DateTime ExpiryTime { get; set; }
}