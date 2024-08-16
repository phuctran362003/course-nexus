using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Curus.Repository.Interfaces;
using Curus.Repository.Entities;
using Curus.Repository.Helper;
using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using Curus.Service.Services;
using StackExchange.Redis; // Ensure you have the appropriate using directive for Redis
using Assert = Xunit.Assert; // Ensure you have the appropriate using directive for Redis

namespace Curus.Tests.Services
{
      public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IAuthRepository> _mockAuthRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IInstructorRepository> _mockInstructorRepository;
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly Mock<IStudentInCourseRepository> _mockStudentInCourseRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserService _userService;
        private readonly TokenGenerators _tokenGenerators;
        private readonly Mock<IRedisService> _mockRedisService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockAuthRepository = new Mock<IAuthRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _mockBlobService = new Mock<IBlobService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockInstructorRepository = new Mock<IInstructorRepository>();
            _mockCourseRepository = new Mock<ICourseRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockStudentInCourseRepository = new Mock<IStudentInCourseRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockRedisService = new Mock<IRedisService>();

            // Setup mock configuration for TokenGenerators
            _mockConfiguration.Setup(c => c["AccessTokenSecret"]).Returns("accessTokenSecretKey");
            _mockConfiguration.Setup(c => c["RefreshTokenSecret"]).Returns("refreshTokenSecretKey");
            _mockConfiguration.Setup(c => c["Issuer"]).Returns("issuer");
            _mockConfiguration.Setup(c => c["Audience"]).Returns("audience");
            _mockConfiguration.Setup(c => c["AccessTokenExpirationMinutes"]).Returns("60");
            _mockConfiguration.Setup(c => c["refreshTokenExpirationMinutes"]).Returns("1440");

            _tokenGenerators = new TokenGenerators(_mockConfiguration.Object);

            _userService = new UserService(
                _mockUserRepository.Object,
                _mockEmailService.Object,
                _mockBlobService.Object,
                _mockConfiguration.Object,
                _mockAuthRepository.Object,
                _tokenGenerators,
                _mockHttpContextAccessor.Object,
                _mockRedisService.Object,
                _mockInstructorRepository.Object,
                _mockCourseRepository.Object,
                _mockCategoryRepository.Object,
                _mockLogger.Object,
                _mockStudentInCourseRepository.Object
            );
        }

        [Fact]
        public async Task GetUserById_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId, FullName = "Test User", Email = "test@example.com", PhoneNumber = "123456789", RoleId = 2 };
            _mockUserRepository.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Student detail", result.Message);
    
            var userDto = Assert.IsType<StudentDTO>(result.Data);
            Assert.Equal(user.FullName, userDto.FullName);
            Assert.Equal(user.Email, userDto.Email);
            Assert.Equal(user.PhoneNumber, userDto.PhoneNumber);

        }


        [Fact]
        public async Task GetUserById_UserDoesNotExist_ThrowsException()
        {
            // Arrange
            var userId = 1;
            _mockUserRepository.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.GetUserById(userId));
        }

        [Fact]
        public async Task DeleteRefreshToken_CallsAuthRepository()
        {
            // Arrange
            var userId = 1;
            _mockAuthRepository.Setup(repo => repo.DeleteRefreshToken(userId)).ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteRefreshToken(userId);

            // Assert
            Assert.True(result);
            _mockAuthRepository.Verify(repo => repo.DeleteRefreshToken(userId), Times.Once);
        }

        [Fact]
        public async Task GetByEmail_UserExists_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email };
            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByEmail(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_CallsUserRepository()
        {
            // Arrange
            var user = new User { UserId = 1, FullName = "Test User" };

            // Act
            await _userService.UpdateUserAsync(user);

            // Assert
            _mockUserRepository.Verify(repo => repo.UpdateAsync(user), Times.Once);
        }

        // Add more tests for other methods in UserService as needed
    }
}
