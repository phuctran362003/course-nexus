using System.Security.Claims;
using Castle.Core.Configuration;
using Curus.API.Controllers;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Request;
using Curus.Service.Interfaces;
using Curus.Service.ResponseDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.Extensions.Primitives;
using Assert = Xunit.Assert;

namespace Curus.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ILogger<OrderService>> _mockOrderLogger;
        private readonly Mock<ILogger<OrderController>> _mockLogger;
        private readonly OrderController _orderController;
        private readonly Mock<IOrderService> _mockOrderService;

        public OrderControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockEmailService = new Mock<IEmailService>();
            _mockOrderLogger = new Mock<ILogger<OrderService>>();
            _mockLogger = new Mock<ILogger<OrderController>>();
            _mockOrderService = new Mock<IOrderService>();

            _orderController = new OrderController(_mockOrderService.Object, _mockLogger.Object);
        }

        private void SetUserClaims(string userId)
        {
            var userClaims = new List<Claim> { new Claim("id", userId) };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task CreateOrder_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var createOrderDto = new CreateOrderDto { CourseIds = new List<int> { 1, 2 } };
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // No user is set in the context
            };

            // Act
            var result = await _orderController.CreateOrder(createOrderDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var unauthorizedValue = unauthorizedResult.Value;
            var actualMessage = (unauthorizedValue.GetType().GetProperty("message")?.GetValue(unauthorizedValue, null)).ToString();
            Assert.Equal("User not authorized", actualMessage);
        }


        [Fact]
        public async Task CreateOrder_ReturnsOk_WhenOrderIsCreatedSuccessfully()
        {
            // Arrange
            SetUserClaims("1");
            var createOrderDto = new CreateOrderDto { CourseIds = new List<int> { 1, 2 } };
            var serviceResponse = new ServiceResponse<StudentOrderDTO>
                { Success = true, Data = new StudentOrderDTO { UserId = 1 } };

            _mockOrderService.Setup(s => s.CreateOrderAsync(It.IsAny<int>(), It.IsAny<List<int>>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _orderController.CreateOrder(createOrderDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(serviceResponse.Data, okResult.Value);
        }
        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenOrderCreationFails()
        {
            // Arrange
            SetUserClaims("1");
            var createOrderDto = new CreateOrderDto { CourseIds = new List<int> { 1, 2 } };
            var serviceResponse = new ServiceResponse<StudentOrderDTO>
            { 
                Success = false, 
                Message = "Order creation failed" 
            };

            _mockOrderService.Setup(s => s.CreateOrderAsync(It.IsAny<int>(), It.IsAny<List<int>>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _orderController.CreateOrder(createOrderDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestValue = badRequestResult.Value;
            var actualMessage = (badRequestValue.GetType().GetProperty("message")?.GetValue(badRequestValue, null)).ToString();
            Assert.Equal("Order creation failed", actualMessage);
        }



        [Fact]
        public async Task GeneratePaymentUrl_ReturnsOk_WithPaymentUrl()
        {
            // Arrange
            var orderId = 1;
            var paymentUrl = "http://payment.url";

            _mockOrderService.Setup(s => s.GeneratePaymentUrlAsync(It.IsAny<int>()))
                .ReturnsAsync(paymentUrl);

            // Act
            var result = await _orderController.GeneratePaymentUrl(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualValue = okResult.Value;
            var actualPaymentUrl = actualValue.GetType().GetProperty("PaymentUrl")?.GetValue(actualValue, null).ToString();
            Assert.Equal(paymentUrl, actualPaymentUrl);
        }


        [Fact]
        public async Task PaymentReturn_ReturnsOk_WhenPaymentIsSuccessful()
        {
            // Arrange
            var request = new VnPayIPNRequest { vnp_TxnRef = "test-txn" };
            var queryParams = new Dictionary<string, StringValues> { { "vnp_SecureHash", "testhash" } };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Query = new QueryCollection(queryParams);
            _orderController.ControllerContext = new ControllerContext { HttpContext = httpContext };

            _mockOrderService.Setup(s =>
                    s.HandleVnPayPaymentReturnAsync(It.IsAny<VnPayIPNRequest>(),
                        It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _orderController.PaymentReturn(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment successful. Please check your email for details.", okResult.Value);
        }

        [Fact]
        public async Task PaymentReturn_ReturnsBadRequest_WhenPaymentFails()
        {
            // Arrange
            var request = new VnPayIPNRequest { vnp_TxnRef = "test-txn" };
            var queryParams = new Dictionary<string, StringValues> { { "vnp_SecureHash", "testhash" } };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Query = new QueryCollection(queryParams);
            _orderController.ControllerContext = new ControllerContext { HttpContext = httpContext };

            _mockOrderService.Setup(s =>
                    s.HandleVnPayPaymentReturnAsync(It.IsAny<VnPayIPNRequest>(),
                        It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync(false);

            // Act
            var result = await _orderController.PaymentReturn(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Payment failed. Please try again.", badRequestResult.Value);
        }

        [Fact]
        public async Task PaymentReturn_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Arrange
            VnPayIPNRequest request = null;

            // Act
            var result = await _orderController.PaymentReturn(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request", badRequestResult.Value);
        }

        [Fact]
        public async Task PaymentReturn_ReturnsBadRequest_WhenQueryParamsAreInvalid()
        {
            // Arrange
            var request = new VnPayIPNRequest { vnp_TxnRef = "test-txn" };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Query = new QueryCollection();
            _orderController.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _orderController.PaymentReturn(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid query parameters", badRequestResult.Value);
        }

        [Fact]
        public async Task PaymentReturn_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            var request = new VnPayIPNRequest { vnp_TxnRef = "test-txn" };
            var queryParams = new Dictionary<string, StringValues> { { "vnp_SecureHash", "testhash" } };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Query = new QueryCollection(queryParams);
            _orderController.ControllerContext = new ControllerContext { HttpContext = httpContext };

            _mockOrderService.Setup(s =>
                    s.HandleVnPayPaymentReturnAsync(It.IsAny<VnPayIPNRequest>(),
                        It.IsAny<IDictionary<string, string>>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _orderController.PaymentReturn(request);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            Assert.Equal("Test exception", serverErrorResult.Value);
        }
    }
}