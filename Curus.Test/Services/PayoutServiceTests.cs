using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Curus.Service.Services;
using Curus.Test.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Curus.Tests.Services;

public class PayoutServiceTests
    {
        private readonly Mock<IInstructorPayoutRepository> _mockPayoutRepository;
        private readonly Mock<IInstructorRepository> _mockInstructorRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IInstructorService> _mockInstructorService;
        private readonly LoggerMock<PayoutService> _loggerMock;
        private readonly PayoutService _payoutService;

        public PayoutServiceTests()
        {
            _mockPayoutRepository = new Mock<IInstructorPayoutRepository>();
            _mockInstructorRepository = new Mock<IInstructorRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockEmailService = new Mock<IEmailService>();
            _mockInstructorService = new Mock<IInstructorService>();
            _loggerMock = new LoggerMock<PayoutService>();
            _payoutService = new PayoutService(
                _mockPayoutRepository.Object,
                _mockInstructorRepository.Object,
                _mockUserRepository.Object,
                _mockHttpContextAccessor.Object,
                _mockEmailService.Object,
                _mockInstructorService.Object,
                _loggerMock
            );
        }

        private void SetupHttpContext(string token)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {token}";
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        }

        private string GenerateJwtToken(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim("id", userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("48nphXHApZZlwgUvdFHWcsGiNazbBxqbdgASO7fVzYdQKYbt"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7203",
                audience: "https://localhost:7203",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Fact]
        public async Task RequestPayout_InsufficientBalance_ReturnsError()
        {
            // Arrange
            var token = GenerateJwtToken(1, "Instructor");
            SetupHttpContext(token);

            _mockInstructorService.Setup(x => x.EarningAnalytics())
                .ReturnsAsync(new UserResponse<object>("Success", new EarningAnalyticRespone { MaintainMoney = 50 }));

            // Act
            var result = await _payoutService.RequestPayout(100);

            // Assert
            Assert.Equal("Insufficient balance", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task RequestPayout_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var token = GenerateJwtToken(1, "Instructor");
            SetupHttpContext(token);

            var earningResponse = new EarningAnalyticRespone { MaintainMoney = 200 };
            _mockInstructorService.Setup(x => x.EarningAnalytics())
                .ReturnsAsync(new UserResponse<object>("Success", earningResponse));

            _mockPayoutRepository.Setup(x => x.AddPayoutRequest(It.IsAny<InstructorPayout>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _payoutService.RequestPayout(100);

            // Assert
            Assert.Equal("Payout request submitted", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ApprovePayout_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var payoutRequest = new InstructorPayout { InstructorId = 1, PayoutAmount = 100, PayoutStatus = PayoutStatus.Pending };
            var instructorRole = new Role { Id = 3, RoleName = "Instructor" };
            var instructorData = new InstructorData { UserId = 1, TaxNumber = "12345", CardNumber = "67890", CardName = "John Doe", CardProvider = CardProviderEnum.Visa, Certification = "Cert123" };
            var user = new User
            {
                UserId = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                RoleId = 3,
                Role = instructorRole,
                InstructorData = instructorData
            };
            var earningResponse = new EarningAnalyticRespone { MaintainMoney = 200 };

            // Mock the GetPayoutRequestById to return the payoutRequest
            _mockPayoutRepository.Setup(x => x.GetPayoutRequestById(It.IsAny<int>()))
                .ReturnsAsync(payoutRequest);

            // Mock the GetInstructorROLEByIdAsync to return the user
            _mockInstructorRepository.Setup(x => x.GetInstructorROLEByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);

            // Mock the GetUserById to return the user
            _mockUserRepository.Setup(x => x.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);

            // Mock the EarningAnalytics to return the earningResponse
            _mockInstructorService.Setup(x => x.EarningAnalytics())
                .ReturnsAsync(new UserResponse<object>("Success", earningResponse));

            // Mock the UpdatePayoutRequest to complete successfully
            _mockPayoutRepository.Setup(x => x.UpdatePayoutRequest(It.IsAny<InstructorPayout>()))
                .Returns(Task.CompletedTask);

            // Mock the SendPayoutApprovalEmail to complete successfully
            _mockEmailService.Setup(x => x.SendPayoutApprovalEmail(It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _payoutService.ApprovePayout(1);

            // Assert
            Assert.Equal("Payout approved and processed", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task RejectPayout_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var payoutRequest = new InstructorPayout { InstructorId = 1, PayoutStatus = PayoutStatus.Pending };
            var instructorRole = new Role { Id = 3, RoleName = "Instructor" };
            var instructorData = new InstructorData { UserId = 1, TaxNumber = "12345", CardNumber = "67890", CardName = "John Doe", CardProvider = CardProviderEnum.Visa, Certification = "Cert123" };
            var user = new User
            {
                UserId = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                RoleId = 3,
                Role = instructorRole,
                InstructorData = instructorData
            };

            // Mock the GetPayoutRequestById to return the payoutRequest
            _mockPayoutRepository.Setup(x => x.GetPayoutRequestById(It.IsAny<int>()))
                .ReturnsAsync(payoutRequest);

            // Mock the GetInstructorROLEByIdAsync to return the user
            _mockInstructorRepository.Setup(x => x.GetInstructorROLEByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);

            // Mock the GetUserById to return the user
            _mockUserRepository.Setup(x => x.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);

            // Mock the UpdatePayoutRequest to complete successfully
            _mockPayoutRepository.Setup(x => x.UpdatePayoutRequest(It.IsAny<InstructorPayout>()))
                .Returns(Task.CompletedTask);

            // Mock the SendPayoutRejectionEmail to complete successfully
            _mockEmailService.Setup(x => x.SendPayoutRejectionEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _payoutService.RejectPayout(1, "Invalid request");

            // Assert
            Assert.Equal("Payout rejected", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task RequestPayout_MissingAuthorizationToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _payoutService.RequestPayout(100));
            Assert.Equal("Authorization token is missing", exception.Message);
        }

        [Fact]
        public async Task RequestPayout_InvalidToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer invalid-token";
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act & Assert
            await Assert.ThrowsAsync<SecurityTokenMalformedException>(() => _payoutService.RequestPayout(100));
        }




        [Fact]
        public async Task ApprovePayout_NonExistentPayoutRequest_ReturnsError()
        {
            // Arrange
            _mockPayoutRepository.Setup(x => x.GetPayoutRequestById(It.IsAny<int>()))
                .ReturnsAsync((InstructorPayout)null);

            // Act
            var result = await _payoutService.ApprovePayout(1);

            // Assert
            Assert.Equal("Payout request not found", result.Message);
            Assert.Null(result.Data);
        }
    }

   