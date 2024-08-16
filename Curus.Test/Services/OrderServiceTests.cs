using Curus.Repository;
using Curus.Repository.Entities;
using Curus.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Curus.Tests.Services
{
    public class OrderServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IEmailService> _mockEmailService;
    private Mock<ILogger<OrderService>> _mockLogger;

    private void InitializeMocks()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<OrderService>>();
    }

    private OrderService CreateOrderService(DbContextOptions<CursusDbContext> options)
    {
        var context = new CursusDbContext(options);
        return new OrderService(context, _mockConfiguration.Object, _mockEmailService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_ReturnsSuccess_WhenOrderIsCreatedSuccessfully()
    {
        // Arrange
        InitializeMocks();
        var options = new DbContextOptionsBuilder<CursusDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var orderService = CreateOrderService(options);

        var userId = 1;
        var courseIds = new List<int> { 1, 2 };
        var user = new User { UserId = userId, FullName = "Test User", Email = "test@example.com" };

        var courses = new List<Course>
        {
            new Course
            {
                Id = 1, Name = "Course 1", Price = 100, Description = "Description 1", ShortSummary = "Summary 1",
                Status = "Active", Thumbnail = "Thumbnail 1", Version = "1.0"
            },
            new Course
            {
                Id = 2, Name = "Course 2", Price = 200, Description = "Description 2", ShortSummary = "Summary 2",
                Status = "Active", Thumbnail = "Thumbnail 2", Version = "1.0"
            }
        };

        await using var context = new CursusDbContext(options);
        await context.Users.AddAsync(user);
        await context.Courses.AddRangeAsync(courses);
        await context.SaveChangesAsync();

        // Act
        var result = await orderService.CreateOrderAsync(userId, courseIds);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Equal(2, result.Data.OrderDetails.Count);
    }

    [Fact]
    public async Task CreateOrderAsync_ReturnsFailure_WhenUserIsNotFound()
    {
        // Arrange
        InitializeMocks();
        var options = new DbContextOptionsBuilder<CursusDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var orderService = CreateOrderService(options);

        var userId = 1;
        var courseIds = new List<int> { 1, 2 };

        // Act
        var result = await orderService.CreateOrderAsync(userId, courseIds);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User not found", result.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_ReturnsFailure_WhenCoursesAreNotFound()
    {
        // Arrange
        InitializeMocks();
        var options = new DbContextOptionsBuilder<CursusDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var orderService = CreateOrderService(options);

        var userId = 1;
        var courseIds = new List<int> { 1, 2 };
        var user = new User { UserId = userId, FullName = "Test User", Email = "test@example.com" };

        await using var context = new CursusDbContext(options);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await orderService.CreateOrderAsync(userId, courseIds);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Some courses not found", result.Message);
    }

    [Fact]
    public async Task GeneratePaymentUrlAsync_ReturnsPaymentUrl_WhenOrderExists()
    {
        // Arrange
        InitializeMocks();
        var options = new DbContextOptionsBuilder<CursusDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var orderService = CreateOrderService(options);

        var orderId = 1;
        var user = new User { UserId = 1, FullName = "Test User", Email = "test@example.com" };
        var order = new StudentOrder
        {
            Id = orderId,
            UserId = user.UserId,
            OrderDate = DateTime.UtcNow,
            OrderStatus = "Pending",
            TotalPrice = 300,
            User = user,
            Payments = new List<Payment>()
        };

        await using var context = new CursusDbContext(options);
        await context.Users.AddAsync(user);
        await context.StudentOrders.AddAsync(order);
        await context.SaveChangesAsync();

        _mockConfiguration.SetupGet(c => c["VnpaySettings:ReturnUrl"]).Returns("http://return.url");
        _mockConfiguration.SetupGet(c => c["VnpaySettings:Url"]).Returns("http://vnpay.url");
        _mockConfiguration.SetupGet(c => c["VnpaySettings:TmnCode"]).Returns("TmnCode");
        _mockConfiguration.SetupGet(c => c["VnpaySettings:HashSecret"]).Returns("HashSecret");

        // Act
        var result = await orderService.GeneratePaymentUrlAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("http://vnpay.url", result);
    }

    [Fact]
    public async Task GeneratePaymentUrlAsync_ThrowsException_WhenOrderIsNotFound()
    {
        // Arrange
        InitializeMocks();
        var options = new DbContextOptionsBuilder<CursusDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var orderService = CreateOrderService(options);

        var orderId = 1;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => orderService.GeneratePaymentUrlAsync(orderId));
        Assert.Equal("Order not found", exception.Message);
    }
}

}
