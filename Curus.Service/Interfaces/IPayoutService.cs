using Curus.Repository.ViewModels.Response;

namespace Curus.Service.Interfaces;

public interface IPayoutService
{
    Task<UserResponse<object>> RequestPayout(decimal amount);
    Task<UserResponse<object>> ApprovePayout(int payoutRequestId);
    Task<UserResponse<object>> RejectPayout(int payoutRequestId, string reason);
}
