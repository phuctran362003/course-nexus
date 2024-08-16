using Curus.Service.Interfaces;
using Curus.Repository.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Curus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Authorize(Policy = "UserPolicy")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var userIdClaim = User.FindFirst("id")?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "User not authorized" });
        }

        var result = await _orderService.CreateOrderAsync(userId, dto.CourseIds);
        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(new { message = result.Message });
        }
    }

    /// <summary>
    /// Generates a payment URL for the specified order.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>The payment URL.</returns>
    [HttpGet("payment-url/{orderId}")]
    public async Task<IActionResult> GeneratePaymentUrl(int orderId)
    {
        try
        {
            var paymentUrl = await _orderService.GeneratePaymentUrlAsync(orderId);
            return Ok(new { PaymentUrl = paymentUrl });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, $"Order with ID {orderId} not found.");
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating the payment URL.");
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("payment-return")]
    public async Task<IActionResult> PaymentReturn([FromQuery] VnPayIPNRequest request)
    {
        try
        {
            if (request == null)
            {
                _logger.LogError("VnPayIPNRequest is null");
                return BadRequest("Invalid request");
            }

            var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());

            if (queryParams == null || !queryParams.Any())
            {
                _logger.LogError("Query parameters are null or empty");
                return BadRequest("Invalid query parameters");
            }

            var result = await _orderService.HandleVnPayPaymentReturnAsync(request, queryParams);
            if (result)
            {
                return Ok("Payment successful. Please check your email for details.");
            }
            else
            {
                return BadRequest("Payment failed. Please try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling VNPay payment return");
            return StatusCode(500, ex.Message);
        }
    }
}

}