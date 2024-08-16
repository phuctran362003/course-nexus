using Curus.API.Controllers;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Curus.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IInstructorService> _mockInstructorService;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IBlobService> _mockBlobService;
    private readonly Mock<ICourseService> _mockCourseService;
    private readonly Mock<IChapterService> _mockChapterService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<UserController>> _mockLogger;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _mockInstructorService = new Mock<IInstructorService>();
        _mockAuthService = new Mock<IAuthService>();
        _mockUserService = new Mock<IUserService>();
        _mockBlobService = new Mock<IBlobService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockCourseService = new Mock<ICourseService>();
        _mockChapterService = new Mock<IChapterService>();
        _mockLogger = new Mock<ILogger<UserController>>();
        _userController = new UserController(
            _mockInstructorService.Object,
            _mockAuthService.Object,
            _mockUserService.Object,
            _mockBlobService.Object,
            _mockEmailService.Object,
            _mockLogger.Object,
            _mockCourseService.Object,
            _mockChapterService.Object
        );
    }
    
    [Fact]
    public async Task GetCardProviders_ReturnsOkResult_WithProviders()
    {
        // Arrange
        var mockProviders = new List<string> { "Provider1", "Provider2" };
        _mockAuthService.Setup(service => service.GetCardProviders()).Returns(mockProviders);

        // Act
        var result = _userController.GetCardProviders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<string>>(okResult.Value);
        Assert.Equal(mockProviders, returnValue);
    }

    [Fact]
    public async Task CreateCourseComment_ReturnsOkResult_WithResult()
    {
        // Arrange
        var commentDto = new CommentDTO { content = "Test Comment" };
        var courseId = 1;
        var mockResult = new UserResponse<object>("Success", null);
        _mockCourseService.Setup(service => service.StudentCreateCommentCourse(commentDto, courseId)).ReturnsAsync(mockResult);

        // Act
        var result = await _userController.CreateCourseComment(commentDto, courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(mockResult, returnValue);
    }

   

    [Fact]
    public async Task ToggleBookmarkCourse_ReturnsOkResult_WithResult()
    {
        // Arrange
        var courseId = 1;
        var mockResult = new UserResponse<object>("Success", null);
        _mockUserService.Setup(service => service.ToggleBookmarkCourse(courseId)).ReturnsAsync(mockResult);

        // Act
        var result = await _userController.ToggleBookmarkCourse(courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(mockResult, returnValue);
    }

    [Fact]
    public async Task ReportCourseByStudent_ReturnsOkResult_WithReport()
    {
        // Arrange
        var reportCourseDto = new ReportCourseDTO { Content = "Test Reason" };
        var courseId = 1;
        var mockReport = new UserResponse<object>("Report submitted", null);
        _mockCourseService.Setup(service => service.ReportCourseById(reportCourseDto, courseId)).ReturnsAsync(mockReport);

        // Act
        var result = await _userController.ReportCourseByStudent(reportCourseDto, courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(mockReport, returnValue);
    }

    [Fact]
    public async Task UpdateInformation_ReturnsOkResult_WithUpdatedUser()
    {
        // Arrange
        var updateUserDto = new UpdateUserDTO { FullName = "Updated User" };
        var mockResult = new UserResponse<object>("Update successful", null);
        _mockUserService.Setup(service => service.UpdateUser(updateUserDto)).ReturnsAsync(mockResult);

        // Act
        var result = await _userController.UpdateInformation(updateUserDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(mockResult, returnValue);
    }

    [Fact]
    public async Task FinishCourse_ReturnsOkResult_WithResult()
    {
        // Arrange
        var courseId = 1;
        var mockResult = new UserResponse<object>("Course finished", null);
        _mockUserService.Setup(service => service.FinishCourse(courseId)).ReturnsAsync(mockResult);

        // Act
        var result = await _userController.FinishCourse(courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(mockResult, returnValue);
    }

}