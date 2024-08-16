using Curus.API.Controllers;
using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;


namespace Curus.Tests.Controllers;

public class CourseControllerTest
{
    private readonly Mock<ICourseService> _mock;
    private readonly CourseController _courseController;

    public CourseControllerTest()
    {
        _mock = new Mock<ICourseService>();
        _courseController = new CourseController(_mock.Object);
    }

    [Fact]
    public async Task CreateCourse_ReturnsOkResult_WithCreatedCourse()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var content = "Hello World from a Fake File";
        var fileName = "test.png";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;
        mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
        mockFile.Setup(_ => _.FileName).Returns(fileName);
        mockFile.Setup(_ => _.Length).Returns(ms.Length);

        var courseDto = new CourseDTO
        {
            Name = "Test Course",
            CategoryIds = new List<int> { 1, 2 },
            Description = "This is a test course description.",
            Thumbnail = mockFile.Object,
            ShortSummary = "Short summary of the test course.",
            AllowComments = true,
            Price = 100000
        };

        var createdCourse = new
        {
            Id = 1,
            Name = courseDto.Name,
            Description = courseDto.Description,
            ShortSummary = courseDto.ShortSummary,
            AllowComments = courseDto.AllowComments,
            Price = courseDto.Price,
            CreatedDate = DateTime.UtcNow,
            InstructorId = 1,
            Thumbnail = fileName,
        };

        var userResponse = new UserResponse<object>("Course created successfully", createdCourse);

        _mock.Setup(service => service.CreateCourse(It.IsAny<CourseDTO>())).ReturnsAsync(userResponse);

        // Act
        var result = await _courseController.CreateCourse(courseDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal("Course created successfully", returnValue.Message);

        var courseData = returnValue.Data as dynamic;
        Assert.NotNull(courseData);
        Assert.Equal(1, courseData.Id);
        Assert.Equal("Test Course", courseData.Name);
        Assert.Equal("This is a test course description.", courseData.Description);
        Assert.Equal(fileName, courseData.Thumbnail);
    }

    [Fact]
    public async Task GetChapterByCourseId_ReturnsOkResult_WithListOfChapters()
    {
        // Arrange
        int courseId = 1;
        var chapters = new List<Chapter>
        {
            new Chapter
            {
                Id = 1, Thumbnail = "thumbnail1.png", Content = "content1", Duration = TimeSpan.FromMinutes(30)
            },
        };
        _mock.Setup(service => service.GetChaptersByCourseId(courseId)).ReturnsAsync(chapters);

        // Act
        var result = await _courseController.GetChapterByCourseId(courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Chapter>>(okResult.Value);

        Assert.Equal(1, returnValue[0].Id);
        Assert.Equal("thumbnail1.png", returnValue[0].Thumbnail);
        Assert.Equal("content1", returnValue[0].Content);
        Assert.Equal(TimeSpan.FromMinutes(30), returnValue[0].Duration);
    }
    
    [Fact]
    public async Task EditDraft_ReturnsOkResult_WithUpdatedCourse()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var content = "Hello World from a Fake File";
        var fileName = "test.png";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;
        mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
        mockFile.Setup(_ => _.FileName).Returns(fileName);
        mockFile.Setup(_ => _.Length).Returns(ms.Length);
        
        
        int courseId = 1;
        var courseDto = new CourseEditDTO
        {
            Name = "Updated Course",
            CategoryIds = new List<int> { 1, 2 },
            Description = "Updated description.",
            Thumbnail = mockFile.Object,
            ShortSummary = "Updated short summary.",
            AllowComments = true,
            Price = 120000
        };
        var updatedCourse = new Course
        {
            Id = courseId,
            Name = courseDto.Name,
            Description = courseDto.Description,
            ShortSummary = courseDto.ShortSummary,
            AllowComments = courseDto.AllowComments,
            Price = courseDto.Price,
            CreatedDate = DateTime.UtcNow,
            InstructorId = 1
        };

        var userResponse = new UserResponse<object>("Course update successfully", updatedCourse);
        _mock.Setup(service => service.EditDraft(courseId, courseDto)).ReturnsAsync(userResponse);

        // Act
        var result = await _courseController.EditDraft(courseDto, courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        var actualCourse = Assert.IsType<Course>(returnValue.Data);
        Assert.Equal(updatedCourse.Id, actualCourse.Id);
        Assert.Equal(updatedCourse.Name, actualCourse.Name);
        Assert.Equal(updatedCourse.Description, actualCourse.Description);
        Assert.Equal(updatedCourse.ShortSummary, actualCourse.ShortSummary);
        Assert.Equal(updatedCourse.AllowComments, actualCourse.AllowComments);
        Assert.Equal(updatedCourse.Price, actualCourse.Price);
    }
    
    [Fact]
    public async Task ReviewRejectCourse_ReturnsOkResult_WithCourse()
    {
        // Arrange
        var course = new Course { Id = 1, Name = "Rejected Course" };
        var userResponse = new UserResponse<object>("Course update successfully", course);
        _mock.Setup(service => service.ReviewRejectCourse()).ReturnsAsync(userResponse);

        // Act
        var result = await _courseController.ReviewRejectCourse();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        var actualCourse = Assert.IsType<Course>(returnValue.Data);
        Assert.Equal(course.Id, actualCourse.Id);
        Assert.Equal(course.Name, actualCourse.Name);
    }
    
    [Fact]
    public async Task ViewSubmitCourseByAdmin_ReturnsOkResult_WithCourses()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1" },
            new Course { Id = 2, Name = "Course 2" }
        };
        var userResponse = new UserResponse<object>("View submitted courses successfully", courses);
        _mock.Setup(service => service.ViewSubmitCourseByAdmin()).ReturnsAsync(userResponse);

        // Act
        var result = await _courseController.ViewSubmitCourseByAdmin();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        var actualCourses = Assert.IsType<List<Course>>(returnValue.Data);

        Assert.Equal(courses.Count, actualCourses.Count);

        // Use Assert.Contains with a predicate
        Assert.Contains(actualCourses, c => c.Id == 1 && c.Name == "Course 1");
        Assert.Contains(actualCourses, c => c.Id == 2 && c.Name == "Course 2");
    }

    [Fact]
    public async Task GetActiveCoursesForAdmin_ReturnsOkResult_WithCourses()
    {
        // Arrange
        int instructorId = 1;
        int pageSize = 20;
        int pageIndex = 1;
        string sortBy = "name";
        bool sortDesc = true;

        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1" },
            new Course { Id = 2, Name = "Course 2" }
        };
        var userResponse = new UserResponse<object>("Get active courses successfully", courses);
        _mock.Setup(service => service.GetCoursesAsync(instructorId, pageSize, pageIndex, sortBy, sortDesc))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _courseController.GetActiveCoursesForAdmin(instructorId, pageSize, pageIndex, sortBy, sortDesc);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        var actualCourses = Assert.IsType<List<Course>>(returnValue.Data);

        Assert.Equal(courses.Count, actualCourses.Count);
        Assert.Contains(actualCourses, c => c.Id == 1 && c.Name == "Course 1");
        Assert.Contains(actualCourses, c => c.Id == 2 && c.Name == "Course 2");
    }
    
    [Fact]
    public async Task GetCoursesByInstructor_ReturnsOkResult_WithCourses()
    {
        // Arrange
        int pageSize = 20;
        int pageIndex = 1;
        string sortBy = "name";
        bool sortDesc = true;
        CourseStatus? status = CourseStatus.Active;

        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1", Status = CourseStatus.Active.ToString() },
            new Course { Id = 2, Name = "Course 2", Status = CourseStatus.Active.ToString() }
        };
        var userResponse = new UserResponse<object>("Get courses successfully", courses);
        _mock.Setup(service => service.GetCoursesByInstructorAsync(pageSize, pageIndex, sortBy, sortDesc, status))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _courseController.GetCoursesByInstructor(pageSize, pageIndex, sortBy, sortDesc, status);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        var actualCourses = Assert.IsType<List<Course>>(returnValue.Data);


        Assert.Equal(courses.Count, actualCourses.Count);
        Assert.Contains(actualCourses, c => c.Id == 1 && c.Name == "Course 1");
        Assert.Contains(actualCourses, c => c.Id == 2 && c.Name == "Course 2");
    }
    
    [Fact]
    public async Task GetAllActiveCourses_ReturnsOkResult_WithCourses()
    {
        // Arrange
        int pageSize = 20;
        int pageIndex = 1;

        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Active Course 1" },
            new Course { Id = 2, Name = "Active Course 2" }
        };
        var userResponse = new UserResponse<object>("Get active courses successfully", courses);
        _mock.Setup(service => service.GetAllActiveCoursesAsync(pageSize, pageIndex))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _courseController.GetAllActiveCourses(pageSize, pageIndex);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        var actualCourses = Assert.IsType<List<Course>>(returnValue.Data);


        Assert.Equal(courses.Count, actualCourses.Count);
        Assert.Contains(actualCourses, c => c.Id == 1 && c.Name == "Active Course 1");
        Assert.Contains(actualCourses, c => c.Id == 2 && c.Name == "Active Course 2");
    }
    
    [Fact]
    public async Task SearchCourses_ReturnsOkResult_WithCourses()
    {
        // Arrange
        Search? searchBy = Search.CategoryName; // Example Search enum value, adjust as needed
        string search = "Active";
        int pageSize = 20;
        int pageIndex = 1;

        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Active Course 1" },
            new Course { Id = 2, Name = "Active Course 2" }
        };
        var userResponse = new UserResponse<object>("Courses search successfully", courses);
        _mock.Setup(service => service.SearchCoursesAsync(searchBy, search, pageSize, pageIndex))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _courseController.SearchCourses(searchBy, search, pageSize, pageIndex);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        var actualCourses = Assert.IsType<List<Course>>(returnValue.Data);

        Assert.Equal(courses.Count, actualCourses.Count);
        Assert.Contains(actualCourses, c => c.Id == 1 && c.Name == "Active Course 1");
        Assert.Contains(actualCourses, c => c.Id == 2 && c.Name == "Active Course 2");
    }

    [Fact]
    public async Task ApproveCourseByAdmin_ReturnsOkResult_WithSuccessMessage()
    {
        // Arrange
        int courseId = 1;
        var successMessage = "Course approved successfully";
        var userResponse = new UserResponse<object>("Course approved successfully", null);
        _mock.Setup(service => service.ApproveCourseByAdmin(courseId))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _courseController.ApproveCourseByAdmin(courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(successMessage, returnValue.Message);
    }
    
    [Fact]
    public async Task RejectCourseByAdmin_ReturnsOkResult_WithSuccessMessage()
    {
        // Arrange
        int courseId = 1;
        var rejectCourseRequest = new RejectCourseRequest
        {
            Reason = "Not meeting quality standards"
        };
        var successMessage = "Course rejected successfully";
        var userResponse = new UserResponse<object>("Course rejected successfully", null);
        _mock.Setup(service => service.RejectCourseByAdmin(courseId, rejectCourseRequest))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _courseController.RejectCourseByAdmin(courseId, rejectCourseRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
        Assert.Equal(successMessage, returnValue.Message);
    }
}