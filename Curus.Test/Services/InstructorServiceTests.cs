using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Request;
using Curus.Service.Interfaces;
using Curus.Service.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Assert = Xunit.Assert;
namespace Curus.Tests.Services;

public class InstructorServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IInstructorRepository> _mockInstructorRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IStudentInCourseRepository> _mockStudentInCourseRepository;
    private readonly Mock<IInstructorPayoutRepository> _mockInstructorPayoutRepository;
    private readonly Mock<IHistoryCourseRepository> _mockHistoryCourseRepository;
    private readonly Mock<IFeedBackRepository> _mockFeedBackRepository;
    private readonly Mock<IOrderDetailRepository> _mockOrderDetailRepository;
    private readonly Mock<ILogger<InstructorService>> _mockLogger;
    private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient;
    private readonly InstructorService _service;

    public InstructorServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockInstructorRepository = new Mock<IInstructorRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockStudentInCourseRepository = new Mock<IStudentInCourseRepository>();
        _mockInstructorPayoutRepository = new Mock<IInstructorPayoutRepository>();
        _mockHistoryCourseRepository = new Mock<IHistoryCourseRepository>();
        _mockFeedBackRepository = new Mock<IFeedBackRepository>();
        _mockOrderDetailRepository = new Mock<IOrderDetailRepository>();
        _mockLogger = new Mock<ILogger<InstructorService>>();
        

        _service = new InstructorService(
            _mockUserRepository.Object,
            _mockInstructorRepository.Object,
            _mockEmailService.Object,
            _mockHttpContextAccessor.Object,
            _mockCourseRepository.Object,
            _mockStudentInCourseRepository.Object,
            _mockInstructorPayoutRepository.Object,
            _mockHistoryCourseRepository.Object,
            _mockFeedBackRepository.Object,
            _mockLogger.Object,
            _mockOrderDetailRepository.Object
        );
    }

    [Fact]
    public async Task ApproveInstructorAsync_UserNotFound_ShouldReturnUserNotFoundResponse()
    {
        // Arrange
        var instructorId = 3;
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(instructorId))
            .ReturnsAsync((User)null);

        // Act
        var result = await _service.ApproveInstructorAsync(instructorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User not found !", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ApproveInstructorAsync_UserAlreadyActiveOrRejected_ShouldReturnAlreadyActiveOrRejectedResponse()
    {
        // Arrange
        var instructorId = 3;
        var user = new User { Status = UserStatus.Active };
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(instructorId))
            .ReturnsAsync(user);

        // Act
        var result = await _service.ApproveInstructorAsync(instructorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User has already been active or rejected !", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ApproveInstructorAsync_SuccessfulApproval_ShouldUpdateUserAndSendEmail()
    {
        // Arrange
        var instructorId = 1;
        var user = new User { Status = UserStatus.Pending, Email = "test@example.com" };
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(instructorId))
            .ReturnsAsync(user);

        // Act
        await _service.ApproveInstructorAsync(instructorId);

        // Assert
        Assert.True(user.IsVerified);
        Assert.Equal(UserStatus.Active, user.Status);
        _mockUserRepository.Verify(repo => repo.UpdateAsync(user), Times.Once);
        _mockEmailService.Verify(emailService => emailService.SendApprovalEmailAsync(user.Email), Times.Once);
    }
    
    [Fact]
    public async Task RejectInstructorAsync_UserNotFound_ShouldReturnUserNotFoundResponse()
    {
        // Arrange
        var rejectDto = new ApproveRejectInstructorDTO { InstructorId = 3, Reason = "Not suitable" };
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(rejectDto.InstructorId))
            .ReturnsAsync((User)null);

        // Act
        var result = await _service.RejectInstructorAsync(rejectDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User not found !", result.Message);
        Assert.Null(result.Data);
    }
    
    [Fact]
    public async Task RejectInstructorAsync_UserAlreadyActiveOrRejected_ShouldReturnAlreadyApprovedOrRejectedResponse()
    {
        // Arrange
        var rejectDto = new ApproveRejectInstructorDTO { InstructorId = 3, Reason = "Not suitable" };
        var user = new User { Status = UserStatus.Active };
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(rejectDto.InstructorId))
            .ReturnsAsync(user);

        // Act
        var result = await _service.RejectInstructorAsync(rejectDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User has already been approved or rejected !", result.Message);
        Assert.Null(result.Data);
    }
    
    [Fact]
    public async Task RejectInstructorAsync_SuccessfulRejection_ShouldUpdateUserAndSendEmail()
    {
        // Arrange
        var rejectDto = new ApproveRejectInstructorDTO { InstructorId = 3, Reason = "Not suitable" };
        var user = new User { Status = UserStatus.Pending, Email = "test@example.com" };
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(rejectDto.InstructorId))
            .ReturnsAsync(user);

        // Act
        var result = await _service.RejectInstructorAsync(rejectDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Reject success", result.Message);
        Assert.Null(result.Data);
        Assert.Equal(UserStatus.Rejected, user.Status);
        _mockUserRepository.Verify(repo => repo.UpdateAsync(user), Times.Once);
        _mockEmailService.Verify(emailService => emailService.SendRejectionEmailAsync(user.Email, rejectDto.Reason), Times.Once);
    }
    
    [Fact]
    public async Task GetPendingInstructorsAsync_NoPendingInstructors_ShouldReturnNoInstructorsResponse()
    {
        // Arrange
        _mockInstructorRepository.Setup(repo => repo.GetPendingInstructorsAsync())
            .ReturnsAsync(new List<User>()); // No pending instructors

        // Act
        var result = await _service.GetPendingInstructorsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Not have any instructor in pending", result.Message);
        Assert.Null(result.Data);
    }
    
    [Fact]
    public async Task GetPendingInstructorsAsync_WithPendingInstructors_ShouldReturnPendingInstructorsList()
    {
        // Arrange
        var pendingInstructors = new List<User>
        {
            new User
            {
                UserId = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Status = UserStatus.Pending,
                InstructorData = new InstructorData { Certification = "cert_path" }
            },
            new User
            {
                UserId = 2,
                FullName = "Jane Smith",
                Email = "jane.smith@example.com",
                PhoneNumber = "0987654321",
                Status = UserStatus.Pending,
                InstructorData = new InstructorData { Certification = "cert_path_2" }
            }
        };
        _mockInstructorRepository.Setup(repo => repo.GetPendingInstructorsAsync())
            .ReturnsAsync(pendingInstructors);

        // Act
        var result = await _service.GetPendingInstructorsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("This is pending list instructor", result.Message);
        Assert.NotNull(result.Data);
        var data = Assert.IsType<List<PendingInstructorDTO>>(result.Data);
        Assert.Equal(2, data.Count);
        Assert.Contains(data, dto => dto.UserId == 1 && dto.FullName == "John Doe");
        Assert.Contains(data, dto => dto.UserId == 2 && dto.FullName == "Jane Smith");
    }
    
    [Fact]
    public async Task GetInstructorDataAsync_UserNotFound_ShouldReturnUserNotFoundResponse()
    {
        // Arrange
        var instructorId = 1;
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(instructorId))
            .ReturnsAsync((User)null);

        // Act
        var result = await _service.GetInstructorDataAsync(instructorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User not found !", result.Message);
        Assert.Null(result.Data);
    }
    
    [Fact]
    public async Task GetInstructorDataAsync_NoCoursesAndStudents_ShouldReturnDataWithZeroValues()
    {
        // Arrange
        var instructorId = 1;
        var user = new User
        {
            UserId = instructorId,
            Email = "test@example.com",
            FullName = "John Doe",
            PhoneNumber = "1234567890",
            Status = UserStatus.Active
        };
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(instructorId))
            .ReturnsAsync(user);
        _mockInstructorRepository.Setup(repo => repo.GetAllCourseOfInstructor(instructorId))
            .ReturnsAsync(new List<Course>()); // No courses
        _mockInstructorRepository.Setup(repo => repo.GetStudentInCourse(instructorId))
            .ReturnsAsync(new List<StudentInCourse>()); // No students
        _mockInstructorRepository.Setup(repo => repo.GetCommentById(instructorId))
            .ReturnsAsync(new List<CommentUser>()); // No comments

        // Act
        var result = await _service.GetInstructorDataAsync(instructorId);

        // Assert
        Assert.NotNull(result);
        var data = Assert.IsType<ManageInstructorDTO>(result.Data);
        Assert.Equal("This is instructor data", result.Message);
        Assert.Equal(0, data.TotalCourses);
        Assert.Equal(0, data.ActivatedCourses);
        Assert.Equal(0, data.TotalEarnedMoney);
        Assert.Equal(0, data.TotalPayout);
        Assert.Equal(0, data.RatingNumber);
        Assert.Empty(data.AdminComment);
    }
    
    [Fact]
    public async Task GetInstructorDataAsync_WithCoursesAndStudents_ShouldReturnCalculatedData()
    {
        // Arrange
        var instructorId = 1;
        var user = new User
        {
            UserId = instructorId,
            Email = "test@example.com",
            FullName = "John Doe",
            PhoneNumber = "1234567890",
            Status = UserStatus.Active
        };
        var courses = new List<Course>
        {
            new Course { Id = 1, Price = 100, Status = "Activated" },
            new Course { Id = 2, Price = 200, Status = "NotActivated" }
        };
        var studentsInCourses = new List<StudentInCourse>
        {
            new StudentInCourse { CourseId = 1, Rating = 5, Courses = new List<Course> { courses[0] } },
            new StudentInCourse { CourseId = 2, Rating = 4, Courses = new List<Course> { courses[1] } }
        };
        var comments = new List<CommentUser>
        {
            new CommentUser { Content = "Good instructor" }
        };
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(instructorId))
            .ReturnsAsync(user);
        _mockInstructorRepository.Setup(repo => repo.GetAllCourseOfInstructor(instructorId))
            .ReturnsAsync(courses);
        _mockInstructorRepository.Setup(repo => repo.GetStudentInCourse(instructorId))
            .ReturnsAsync(studentsInCourses);
        _mockInstructorRepository.Setup(repo => repo.GetCommentById(instructorId))
            .ReturnsAsync(comments);

        // Act
        var result = await _service.GetInstructorDataAsync(instructorId);

        // Assert
        Assert.NotNull(result);
        var data = Assert.IsType<ManageInstructorDTO>(result.Data);
        Assert.Equal("This is instructor data", result.Message);
        Assert.Equal(2, data.TotalCourses);
        Assert.Equal(1, data.ActivatedCourses);
        Assert.Equal((100 + 200) / 2, data.TotalEarnedMoney); // (Price of 100 * 1 + 200 * 1) / 2
        Assert.Equal(data.TotalEarnedMoney * 0.90m, data.TotalPayout);
        Assert.Equal((5 + 4) / 2.0, data.RatingNumber); // Average rating
        Assert.Single(data.AdminComment);
    }
    
    [Fact]
    public async Task GetAllInstructorDataAsync_NoInstructors_ShouldReturnNoInstructorsResponse()
    {
        // Arrange
        _mockInstructorRepository.Setup(repo => repo.GetAllInstructorAsync())
            .ReturnsAsync((List<User>)null);

        // Act
        var result = await _service.GetAllInstructorDataAsync(1,20);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("List instructor not found", result.Message);
        Assert.Null(result.Data);
    }
    
    [Fact]
    public async Task GetAllInstructorDataAsync_WithInstructors_ShouldReturnPagedAndSortedData()
    {
        // Arrange
        var instructors = new List<User>
        {
            new User { UserId = 1, Email = "john@example.com", FullName = "John Doe", PhoneNumber = "1234567890", Status = UserStatus.Active },
            new User { UserId = 2, Email = "jane@example.com", FullName = "Jane Smith", PhoneNumber = "0987654321", Status = UserStatus.Inactive }
        };

        var courses = new List<Course>
        {
            new Course { Id = 1, Price = 100, Status = "Activated" }
        };

        var studentsInCourses = new List<StudentInCourse>
        {
            new StudentInCourse { CourseId = 1, Rating = 5, Courses = courses }
        };

        var comments = new List<CommentUser>
        {
            new CommentUser { Content = "Great instructor" }
        };

        _mockInstructorRepository.Setup(repo => repo.GetAllInstructorAsync())
            .ReturnsAsync(instructors);
        _mockInstructorRepository.Setup(repo => repo.GetAllCourseOfInstructor(It.IsAny<int>()))
            .ReturnsAsync(courses);
        _mockInstructorRepository.Setup(repo => repo.GetStudentInCourse(It.IsAny<int>()))
            .ReturnsAsync(studentsInCourses);
        _mockInstructorRepository.Setup(repo => repo.GetCommentById(It.IsAny<int>()))
            .ReturnsAsync(comments);

        // Act
        var result = await _service.GetAllInstructorDataAsync(1,20);

        // Assert
        Assert.NotNull(result);
        var data = Assert.IsType<List<ManageInstructorDTO>>(result.Data);
        Assert.Equal("This is list instructor data", result.Message);
        Assert.Equal(2, data.Count);
        Assert.Equal(2, data.First().Id); // Verify sorting by ID in descending order
    }

    [Fact]
    public async Task GetAllInstructorDataAsync_WithPaging_ShouldReturnPagedData()
    {
        // Arrange
        var instructors = Enumerable.Range(1, 25).Select(i => new User
        {
            UserId = i,
            Email = $"user{i}@example.com",
            FullName = $"User {i}",
            PhoneNumber = $"12345678{i}",
            Status = UserStatus.Active
        }).ToList();

        var courses = new List<Course>
        {
            new Course { Id = 1, Price = 100, Status = "Activated" }
        };

        var studentsInCourses = new List<StudentInCourse>
        {
            new StudentInCourse { CourseId = 1, Rating = 5, Courses = courses }
        };

        var comments = new List<CommentUser>
        {
            new CommentUser { Content = "Good instructor" }
        };

        _mockInstructorRepository.Setup(repo => repo.GetAllInstructorAsync())
            .ReturnsAsync(instructors);
        _mockInstructorRepository.Setup(repo => repo.GetAllCourseOfInstructor(It.IsAny<int>()))
            .ReturnsAsync(courses);
        _mockInstructorRepository.Setup(repo => repo.GetStudentInCourse(It.IsAny<int>()))
            .ReturnsAsync(studentsInCourses);
        _mockInstructorRepository.Setup(repo => repo.GetCommentById(It.IsAny<int>()))
            .ReturnsAsync(comments);

        // Act
        var result = await _service.GetAllInstructorDataAsync(2,5); // Page 2 with pageSize 20

        // Assert
        Assert.NotNull(result);
        var data = Assert.IsType<List<ManageInstructorDTO>>(result.Data);
        Assert.Equal("This is list instructor data", result.Message);
        Assert.Equal(5, data.Count); // Page 2 should have 5 items
        Assert.Equal(20, data.First().Id); // Verify the starting ID of the second page
    }
    
    
    
    [Fact]
    public async Task ExportToExcelAsync_ReturnsError_WhenNoInstructors()
    {
        // Arrange
        var instructorList = new List<User>();
        _mockInstructorRepository.Setup(repo => repo.GetAllInstructorAsync()).ReturnsAsync(instructorList);

        // Act
        var result = await _service.ExportToExcelAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Not have any instructor", result.Message);
        Assert.Null(result.Data);
    }
    
    [Fact]
    public async Task CreateCommentToInstructor_UserNotFound_ReturnsFalse()
    {
        // Arrange
        int userId = 123;
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

        var commentDTO = new CommentDTO { content = "Test comment" };

        // Act
        var result = await _service.CreateCommentToInstructor(commentDTO, userId);

        // Assert
        Assert.False(result.Item1);
        Assert.Equal("No user with id 123 in instructor role", result.Item2);
    }

    [Fact]
    public async Task CreateCommentToInstructor_UserIsAdmin_ReturnsFalse()
    {
        // Arrange
        int adminId = 1;
        var user = new User { UserId = adminId };
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(adminId)).ReturnsAsync(user);

        var commentDTO = new CommentDTO { content = "Test comment" };

        // Act
        var result = await _service.CreateCommentToInstructor(commentDTO, adminId);

        // Assert
        Assert.False(result.Item1);
        Assert.Equal("Cannot comment to admin", result.Item2);
    }

    [Fact]
    public async Task CreateCommentToInstructor_ValidUser_AddsComment()
    {
        // Arrange
        int userId = 123;
        var user = new User { UserId = userId };
        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

        var commentDTO = new CommentDTO { content = "Test comment" };
        _mockInstructorRepository.Setup(repo => repo.CreateCommentUserAsync(It.IsAny<CommentUser>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateCommentToInstructor(commentDTO, userId);

        // Assert
        Assert.True(result.Item1);
        Assert.Equal("Add comment success", result.Item2);
        _mockInstructorRepository.Verify(repo => repo.CreateCommentUserAsync(It.Is<CommentUser>(c => c.Content == commentDTO.content && c.UserId == userId && c.CommentedById == 1)), Times.Once);
    }
    
    [Fact]
    public async Task ViewCommentById_CommentExists_ReturnsCommentList()
    {
        // Arrange
        int commentId = 1;
        var comments = new List<CommentUser>
        {
            new CommentUser { Id = 1, Content = "Comment 1" },
            new CommentUser { Id = 2, Content = "Comment 2" }
        };

        _mockInstructorRepository.Setup(repo => repo.GetCommentById(commentId)).ReturnsAsync(comments);

        // Act
        var result = await _service.ViewCommentById(commentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Comment 1", result[0].comment);
        Assert.Equal("Comment 2", result[1].comment);
    }

    [Fact]
    public async Task ViewCommentById_NoComments_ReturnsEmptyList()
    {
        // Arrange
        int commentId = 1;
        var comments = new List<CommentUser>();

        _mockInstructorRepository.Setup(repo => repo.GetCommentById(commentId)).ReturnsAsync(comments);

        // Act
        var result = await _service.ViewCommentById(commentId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task EditCommentByCommentId_CommentExists_EditsComment()
    {
        // Arrange
        int commentId = 1;
        var existingComment = new CommentUser { Id = commentId, Content = "Old Content" };
        var updatedContent = "New Content";
        var commentDto = new CommentDTO { content = updatedContent };

        _mockInstructorRepository.Setup(repo => repo.GetCommentByCommentId(commentId)).ReturnsAsync(existingComment);
        _mockInstructorRepository.Setup(repo => repo.EditCommentByCommentId(existingComment)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.EditCommentByCommentId(commentDto, commentId);

        // Assert
        Assert.True(result);
        Assert.Equal(updatedContent, existingComment.Content);
    }

    [Fact]
    public async Task EditCommentByCommentId_CommentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        int commentId = 1;
        var commentDto = new CommentDTO { content = "New Content" };

        _mockInstructorRepository.Setup(repo => repo.GetCommentByCommentId(commentId)).ReturnsAsync((CommentUser)null);

        // Act
        var result = await _service.EditCommentByCommentId(commentDto, commentId);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task DeleteCommentByCommentId_CommentExists_DeletesComment()
    {
        // Arrange
        int commentId = 1;
        var existingComment = new CommentUser { Id = commentId, Content = "Some Content" };

        _mockInstructorRepository.Setup(repo => repo.GetCommentByCommentId(commentId)).ReturnsAsync(existingComment);
        _mockInstructorRepository.Setup(repo => repo.DeleteCommentByCommentId(existingComment)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteCommentByCommentId(commentId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCommentByCommentId_CommentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        int commentId = 1;

        _mockInstructorRepository.Setup(repo => repo.GetCommentByCommentId(commentId)).ReturnsAsync((CommentUser)null);

        // Act
        var result = await _service.DeleteCommentByCommentId(commentId);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task ChangeStatusInstructor_UserNotFound_ReturnsErrorMessage()
    {
        // Arrange
        int userId = 1;
        var contentEmailDto = new ContentEmailDTO { content = "Test Content" };

        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _service.ChangeStatusInstructor(contentEmailDto, userId);

        // Assert
        Assert.Equal("User not found !", result.Message);
    }

    [Fact]
    public async Task ChangeStatusInstructor_UserPendingStatus_ReturnsErrorMessage()
    {
        // Arrange
        int userId = 1;
        var contentEmailDto = new ContentEmailDTO { content = "Test Content" };
        var user = new User { UserId = userId, Status = UserStatus.Pending };

        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _service.ChangeStatusInstructor(contentEmailDto, userId);

        // Assert
        Assert.Equal("This user is in pending status !", result.Message);
    }

    [Fact]
    public async Task ChangeStatusInstructor_UserActive_SendsDeactivationEmail()
    {
        // Arrange
        int userId = 1;
        var contentEmailDto = new ContentEmailDTO { content = "Deactivation Content" };
        var user = new User { UserId = userId, Status = UserStatus.Active };

        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _mockEmailService.Setup(service => service.SendDeactiveEmailAsync(user.Email, contentEmailDto.content)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.ChangeStatusInstructor(contentEmailDto, userId);

        // Assert
        Assert.Equal("Status updated successfully", result.Message);
        Assert.Equal(UserStatus.Inactive, user.Status);
        _mockEmailService.Verify(service => service.SendDeactiveEmailAsync(user.Email, contentEmailDto.content), Times.Once);
        _mockInstructorRepository.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task ChangeStatusInstructor_UserInactive_SendsActivationEmail()
    {
        // Arrange
        int userId = 1;
        var contentEmailDto = new ContentEmailDTO { content = "Activation Content" };
        var user = new User { UserId = userId, Status = UserStatus.Inactive };

        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _mockEmailService.Setup(service => service.SendActiveEmailAsync(user.Email)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.ChangeStatusInstructor(contentEmailDto, userId);

        // Assert
        Assert.Equal("Status updated successfully", result.Message);
        Assert.Equal(UserStatus.Active, user.Status);
        _mockEmailService.Verify(service => service.SendActiveEmailAsync(user.Email), Times.Once);
        _mockInstructorRepository.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task ChangeStatusInstructor_UserInvalidStatus_ReturnsErrorMessage()
    {
        // Arrange
        int userId = 1;
        var contentEmailDto = new ContentEmailDTO { content = "Test Content" };
        var user = new User { UserId = userId, Status = (UserStatus)999 }; // Invalid status

        _mockInstructorRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _service.ChangeStatusInstructor(contentEmailDto, userId);

        // Assert
        Assert.Equal("Invalid status change request", result.Message);
    }
    
    [Fact]
    public async Task ChangeStatusCourse_InactiveCourse_SendsEmailsToStudents()
    {
        // Arrange
        int courseId = 1;
        var request = new ChangeStatusCourseRequest { DeactivationPeriod = 1, ChangeByTime = 0 }; // 1 year
        var course = new Course { Status = "Inactive" };
        var students = new List<StudentInCourse> { new StudentInCourse { UserId = 1 } };
        var user = new User { Email = "test@example.com" };

        _mockCourseRepository.Setup(repo => repo.GetCourseById(courseId)).ReturnsAsync(course);
        _mockCourseRepository.Setup(repo => repo.GetStudentInCourse(courseId)).ReturnsAsync(students);
        _mockUserRepository.Setup(repo => repo.GetAllUserById(It.IsAny<int>())).ReturnsAsync(user);

        // Act
        var result = await _service.ChangeStatusCourse(courseId, request);

        // Assert
        Assert.Equal("Change status success", result.Message);
        _mockEmailService.Verify(service => service.SendChangeStatusEmailAsync(user.Email, request.Reason), Times.Once);
    }
    
}
