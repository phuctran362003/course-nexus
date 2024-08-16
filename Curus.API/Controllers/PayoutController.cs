using Curus.Service.Interfaces;
using Curus.Repository.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Curus.Repository.ViewModels.Request;

namespace Curus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayoutController : ControllerBase
    {
        private readonly IPayoutService _payoutService;

        public PayoutController(IPayoutService payoutService)
        {
            _payoutService = payoutService;
        }

        /// <summary>
        /// Creates a new payout request.
        /// </summary>
        /// <param name="amount">The amount to request for payout.</param>
        /// <returns>The result of the payout request.</returns>
        [HttpPost("requests")]
        public async Task<IActionResult> CreatePayoutRequest([FromForm] decimal amount)
        {
            try
            {
                var result = await _payoutService.RequestPayout(amount);
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Approves a payout request.
        /// </summary>
        /// <param name="payoutRequestId">The ID of the payout request to approve.</param>
        /// <returns>The result of the approval.</returns>
        [HttpPut("requests/{payoutRequestId}/approve")]
        public async Task<IActionResult> ApprovePayoutRequest(int payoutRequestId)
        {
            try
            {
                var result = await _payoutService.ApprovePayout(payoutRequestId);
                if (result.Message == "Payout request not found")
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Rejects a payout request.
        /// </summary>
        /// <param name="request">The request containing the payout request ID and the reason for rejection.</param>
        /// <returns>The result of the rejection.</returns>
        [HttpPut("requests/{payoutRequestId}/reject")]
        public async Task<IActionResult> RejectPayoutRequest([FromBody] RejectPayoutRequest request)
        {
            try
            {
                var result = await _payoutService.RejectPayout(request.PayoutRequestId, request.Reason);
                if (result.Message == "Payout request not found")
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}