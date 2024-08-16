using Curus.API.Controllers;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Curus.Tests.Controllers;

public class InstructorControllerTests
{
    private readonly Mock<IInstructorService> _mockInstructorService;
    private readonly Mock<IReportFeedbackService> _mockReportFeedbackService;
    private readonly Mock<ICourseService> _mockCourseService;
    private readonly Mock<IDiscountService> _mockDiscountService;
    private readonly InstructorController _controller;

    public InstructorControllerTests()
    {
        _mockInstructorService = new Mock<IInstructorService>();
        _mockReportFeedbackService = new Mock<IReportFeedbackService>();
        _mockCourseService = new Mock<ICourseService>();
        _mockDiscountService = new Mock<IDiscountService>();

        _controller = new InstructorController(
            _mockInstructorService.Object,
            _mockReportFeedbackService.Object,
            _mockCourseService.Object,
            _mockDiscountService.Object
        );
    }

    [Fact]
    public async Task GetInstructorDataByID_ReturnsOkResult_WithInstructorData()
    {
        // Arrange
        var instructorId = 1;
        var instructorDto = new ManageInstructorDTO { Id = instructorId, Name = "John Doe" };
        
        var userResponse = new UserResponse<object>("This is instructor data", instructorDto);
        _mockInstructorService.Setup(service => service.GetInstructorDataAsync(instructorId))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _controller.GetInstructorDataByID(instructorId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(userResponse, returnValue);
    }

    [Fact]
    public async Task GetInstructorDataByID_ReturnsNotFound_WhenExceptionThrown()
    {
        // Arrange
        var instructorId = 1;
        var exceptionMessage = "Instructor not found";
        _mockInstructorService.Setup(service => service.GetInstructorDataAsync(instructorId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.GetInstructorDataByID(instructorId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(exceptionMessage, notFoundResult.Value);
    }
    
    [Fact]
    public async Task SubmitCourse_ReturnsOkResult_WithSubmittedCourse()
    {
        // Arrange
        var courseId = 1;
        var submittedCourse = new Course { Id = courseId, Name = "Sample Course" };
        
        var userResponse = new UserResponse<object>("Submitted course successfully", submittedCourse);
        _mockInstructorService.Setup(service => service.SubmitCourse(courseId))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _controller.SubmitCourse(courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(submittedCourse, returnValue.Data);
        Assert.Equal("Submitted course successfully", returnValue.Message);
    }
    
    [Fact]
    public async Task ReviewCourse_ReturnsOkResult_WithReviewedCourse()
    {
        // Arrange
        var courseId = 1;
        var reviewedCourse = new Course { Id = courseId, Name = "Sample Course" };
        
        var userResponse = new UserResponse<object>("Review Successfully", reviewedCourse);
        _mockInstructorService.Setup(service => service.ReviewCourse(courseId))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _controller.ReviewCourse(courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(reviewedCourse, returnValue.Data);
        Assert.Equal("Review Successfully", returnValue.Message);
    }
    
    [Fact]
    public async Task ChangeStatusCourse_ReturnsOkResult_WithStatusChangeResult()
    {
        // Arrange
        var courseId = 1;
        var changeStatusRequest = new ChangeStatusCourseRequest { Reason = "Active" };
        
        var userResponse = new UserResponse<object>("Change status success", changeStatusRequest);
        _mockInstructorService.Setup(service => service.ChangeStatusCourse(courseId, changeStatusRequest))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _controller.ChangeStatusCourse(courseId, changeStatusRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal("Change status success", returnValue.Message);
    }
    
    [Fact]
    public async Task EarningAnalytics_ReturnsOkResult_WithAnalyticsData()
    {
        // Arrange
        var expectedResult = new { TotalEarnings = 1000, NumberOfCourses = 5 };
        
        var userResponse = new UserResponse<object>("This is data of earning analytics", expectedResult);
        _mockInstructorService.Setup(service => service.EarningAnalytics())
            .ReturnsAsync(userResponse);

        // Act
        var result = await _controller.EarningAnalytics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(expectedResult, returnValue.Data);
        Assert.Equal("This is data of earning analytics", returnValue.Message);
    }
    
    [Fact]
    public async Task ToggleMarkGoodReview_ReturnsOkResult_WithToggleResult()
    {
        // Arrange
        var reviewId = 1;
        var expectedResult = new { Success = true, Message = "Review marked as good" };
        
        var userResponse = new UserResponse<object>("Mark this feedback to good feedback success", expectedResult);
        _mockInstructorService.Setup(service => service.toggleMarkGoodReview(reviewId))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _controller.toggleMarkGoodReview(reviewId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal("Mark this feedback to good feedback success", returnValue.Message);
    }
    
    [Fact]
    public async Task ReportReviewToAdmin_ReturnsOkResult_WithReportResult()
    {
        // Arrange
        var reviewId = 1;
        var reportFeedbackRequest = new ReportFeedbackRequest { Reason = "Inappropriate content" };
        var expectedResult = new { Success = true, Message = "Review reported to admin" };
        
        var userResponse = new UserResponse<object>("Report this feedback success", expectedResult);
        _mockReportFeedbackService.Setup(service => service.reportReviewToAdmin(reviewId, reportFeedbackRequest))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _controller.reportReviewToAdmin(reviewId, reportFeedbackRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal("Report this feedback success", returnValue.Message);
    }
    
    [Fact]
    public async Task UserDiscountForCourse_ReturnsOkResult_WithDiscountResult()
    {
        // Arrange
        var courseId = 1;
        var discountCourseDto = new DiscountCourseDTO { DiscountCode = "SAVE20" };
        var expectedResult = new { Success = true, Message = "Discount applied successfully" };
        
        var userResponse = new UserResponse<object>("Course discount successfully", expectedResult);
        _mockDiscountService.Setup(service => service.UseDiscountForCourse(courseId, discountCourseDto))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _controller.UserDiscountForCourse(courseId, discountCourseDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal("Course discount successfully", returnValue.Message);
    }
}
