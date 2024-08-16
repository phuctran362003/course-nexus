using System.IdentityModel.Tokens.Jwt;
using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Curus.Service.Services
{
    public class PayoutService : IPayoutService
{
    private readonly IInstructorPayoutRepository _payoutRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;
    private readonly IInstructorService _instructorService;
    private readonly ILogger<PayoutService> _logger;

    public PayoutService(IInstructorPayoutRepository payoutRepository,
                         IInstructorRepository instructorRepository,
                         IUserRepository userRepository,
                         IHttpContextAccessor httpContextAccessor,
                         IEmailService emailService,
                         IInstructorService instructorService,
                         ILogger<PayoutService> logger)
    {
        _payoutRepository = payoutRepository;
        _instructorRepository = instructorRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
        _instructorService = instructorService;
        _logger = logger;
    }

    private int GetUserIdFromToken()
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Authorization token is missing");
            throw new UnauthorizedAccessException("Authorization token is missing");
        }

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        if (jwtToken == null)
        {
            _logger.LogError("Invalid token");
            throw new UnauthorizedAccessException("Invalid token");
        }

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "id");
        if (userIdClaim == null)
        {
            _logger.LogError("User ID claim is missing in token");
            throw new UnauthorizedAccessException("User ID claim is missing in token");
        }

        return int.Parse(userIdClaim.Value);
    }

    public async Task<UserResponse<object>> RequestPayout(decimal amount)
    {
        var userId = GetUserIdFromToken();
        _logger.LogInformation($"RequestPayout initiated for user ID {userId} with amount {amount}");

            // Get the balance from EarningAnalytics
            var earningResponse = await _instructorService.EarningAnalytics();
            if (earningResponse == null || earningResponse.Data == null)
            {
                _logger.LogError("Unable to fetch earnings data");
                return new UserResponse<object>( "Unable to fetch earnings data", null);
            }

            var earningsData = earningResponse.Data as EarningAnalyticRespone;
            if (earningsData == null)
            {
                _logger.LogError("Earnings data is invalid");
                return new UserResponse<object>( "Earnings data is invalid", null);
            }

            if (earningsData.MaintainMoney < amount)
            {
                _logger.LogError("Insufficient balance");
                return new UserResponse<object>("Insufficient balance", null);
            }

        var payoutRequest = new InstructorPayout
        {
            InstructorId = userId,
            PayoutAmount = amount,
            RequestDate = DateTime.UtcNow,
            PayoutStatus = PayoutStatus.Pending,
            RejectionReason = string.Empty // Provide a default value for RejectionReason
        };

            await _payoutRepository.AddPayoutRequest(payoutRequest);
            _logger.LogInformation($"Payout request submitted for user ID {userId} with amount {amount}");
            return new UserResponse<object>( "Payout request submitted", null);
        }

    public async Task<UserResponse<object>> ApprovePayout(int payoutRequestId)
    {
        _logger.LogInformation($"ApprovePayout initiated for payout request ID {payoutRequestId}");

    var payoutRequest = await _payoutRepository.GetPayoutRequestById(payoutRequestId);
    if (payoutRequest == null)
    {
        _logger.LogError($"Payout request with ID {payoutRequestId} not found");
        return new UserResponse<object>("Payout request not found", null);
    }

        _logger.LogInformation($"Payout request with ID {payoutRequestId} found, fetching instructor details");

    var instructor = await _instructorRepository.GetInstructorROLEByIdAsync(payoutRequest.InstructorId);
    if (instructor == null)
    {
        _logger.LogError($"Instructor with ID {payoutRequest.InstructorId} not found");
        return new UserResponse<object>( "Instructor not found", null);
    }

        _logger.LogInformation($"Instructor with ID {payoutRequest.InstructorId} found, fetching user details");

    var user = await _userRepository.GetUserById(instructor.UserId);
    if (user == null)
    {
        _logger.LogError($"User with ID {instructor.UserId} not found");
        return new UserResponse<object>( "User not found", null);
    }

        _logger.LogInformation($"User with ID {instructor.UserId} found: {user.FullName}");

        payoutRequest.PayoutStatus = PayoutStatus.Approved;
        payoutRequest.PayoutDate = DateTime.UtcNow;
        await _payoutRepository.UpdatePayoutRequest(payoutRequest);

        // Update MaintainMoney after approving payout
        var earningResponse = await _instructorService.EarningAnalytics();
        if (earningResponse != null && earningResponse.Data != null)
        {
            var earningsData = earningResponse.Data as EarningAnalyticRespone;
            if (earningsData != null)
            {
                earningsData.MaintainMoney -= payoutRequest.PayoutAmount;
                _logger.LogInformation($"Updated MaintainMoney for user ID {user.UserId} to {earningsData.MaintainMoney}");
            }
        }

        await _emailService.SendPayoutApprovalEmail(user.Email, payoutRequest.PayoutAmount);
        _logger.LogInformation($"Payout request with ID {payoutRequestId} approved for user {user.Email}");

    return new UserResponse<object>("Payout approved and processed", null);
 }

    public async Task<UserResponse<object>> RejectPayout(int payoutRequestId, string reason)
    {
        _logger.LogInformation($"RejectPayout initiated for payout request ID {payoutRequestId}");

            var payoutRequest = await _payoutRepository.GetPayoutRequestById(payoutRequestId);
            if (payoutRequest == null)
            {
                _logger.LogError($"Payout request with ID {payoutRequestId} not found");
                return new UserResponse<object>( "Payout request not found", null);
            }

        _logger.LogInformation($"Payout request with ID {payoutRequestId} found, fetching instructor details");

            var instructor = await _instructorRepository.GetInstructorROLEByIdAsync(payoutRequest.InstructorId);
            if (instructor == null)
            {
                _logger.LogError($"Instructor with ID {payoutRequest.InstructorId} not found");
                return new UserResponse<object>( "Instructor not found", null);
            }

        _logger.LogInformation($"Instructor with ID {payoutRequest.InstructorId} found, fetching user details");

            var user = await _userRepository.GetUserById(instructor.UserId);
            if (user == null)
            {
                _logger.LogError($"User with ID {instructor.UserId} not found");
                return new UserResponse<object>("User not found", null);
            }

        _logger.LogInformation($"User with ID {instructor.UserId} found: {user.FullName}");

        payoutRequest.PayoutStatus = PayoutStatus.Rejected;
        payoutRequest.RejectionReason = reason;
        payoutRequest.RejectionDate = DateTime.UtcNow;
        await _payoutRepository.UpdatePayoutRequest(payoutRequest);

        await _emailService.SendPayoutRejectionEmail(user.Email, reason);
        _logger.LogInformation($"Payout request with ID {payoutRequestId} rejected for user {user.Email}");

            return new UserResponse<object>( "Payout rejected", null);
        }
    }



}
