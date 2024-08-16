using Curus.API.Controllers;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Curus.Tests.Controllers;

public class PayoutControllerTests
    {
        private readonly Mock<IPayoutService> _mockPayoutService;
        private readonly PayoutController _controller;

        public PayoutControllerTests()
        {
            _mockPayoutService = new Mock<IPayoutService>();
            _controller = new PayoutController(_mockPayoutService.Object);
        }

        [Fact]
        public async Task CreatePayoutRequest_ValidAmount_ReturnsOkResult()
        {
            // Arrange
            var amount = 100m;
            var response = new UserResponse<object>("Payout request submitted", null);
            _mockPayoutService.Setup(service => service.RequestPayout(amount))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreatePayoutRequest(amount);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<UserResponse<object>>(okResult.Value);
            Assert.Equal("Payout request submitted", returnedValue.Message);
            Assert.Null(returnedValue.Data);
        }

        [Fact]
        public async Task ApprovePayoutRequest_ValidRequestId_ReturnsOkResult()
        {
            // Arrange
            var payoutRequestId = 1;
            var response = new UserResponse<object>("Payout approved and processed", null);
            _mockPayoutService.Setup(service => service.ApprovePayout(payoutRequestId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ApprovePayoutRequest(payoutRequestId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<UserResponse<object>>(okResult.Value);
            Assert.Equal("Payout approved and processed", returnedValue.Message);
            Assert.Null(returnedValue.Data);
        }

        [Fact]
        public async Task RejectPayoutRequest_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new RejectPayoutRequest { PayoutRequestId = 1, Reason = "Invalid request" };
            var response = new UserResponse<object>("Payout rejected", null);
            _mockPayoutService.Setup(service => service.RejectPayout(request.PayoutRequestId, request.Reason))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.RejectPayoutRequest(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<UserResponse<object>>(okResult.Value);
            Assert.Equal("Payout rejected", returnedValue.Message);
            Assert.Null(returnedValue.Data);
        }

        [Fact]
        public async Task CreatePayoutRequest_InvalidAmount_ReturnsBadRequest()
        {
            // Arrange
            var amount = -100m;
            _mockPayoutService.Setup(service => service.RequestPayout(amount))
                .ThrowsAsync(new ArgumentException("Invalid amount"));

            // Act
            var result = await _controller.CreatePayoutRequest(amount);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid amount", badRequestResult.Value);
        }

        [Fact]
        public async Task ApprovePayoutRequest_InvalidRequestId_ReturnsNotFound()
        {
            // Arrange
            var payoutRequestId = -1;
            var response = new UserResponse<object>("Payout request not found", null);
            _mockPayoutService.Setup(service => service.ApprovePayout(payoutRequestId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ApprovePayoutRequest(payoutRequestId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnedValue = Assert.IsType<UserResponse<object>>(notFoundResult.Value);
            Assert.Equal("Payout request not found", returnedValue.Message);
            Assert.Null(returnedValue.Data);
        }

        [Fact]
        public async Task RejectPayoutRequest_InvalidRequest_ReturnsNotFound()
        {
            // Arrange
            var request = new RejectPayoutRequest { PayoutRequestId = -1, Reason = "Invalid request" };
            var response = new UserResponse<object>("Payout request not found", null);
            _mockPayoutService.Setup(service => service.RejectPayout(request.PayoutRequestId, request.Reason))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.RejectPayoutRequest(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnedValue = Assert.IsType<UserResponse<object>>(notFoundResult.Value);
            Assert.Equal("Payout request not found", returnedValue.Message);
            Assert.Null(returnedValue.Data);
        }
    }