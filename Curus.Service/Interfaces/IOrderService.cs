using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Request;
using Curus.Service.ResponseDTO;

namespace Curus.Service.Interfaces;

public interface IOrderService
{
    Task<ServiceResponse<StudentOrderDTO>> CreateOrderAsync(int userId, List<int> courseIds);
    Task<string> GeneratePaymentUrlAsync(int orderId);

    Task<bool> HandleVnPayPaymentReturnAsync(VnPayIPNRequest request, IDictionary<string, string> queryParams);
}