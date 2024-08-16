using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Curus.Service.Services;
using Curus.Test.Utils;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;
using OfficeOpenXml;
using Xunit;
using Assert = Xunit.Assert;

namespace Curus.Tests;

public class CourseServiceTest
{
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IBlobService> _mockBlobService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IBackUpCourseRepository> _mockBackUpCourseRepository;
    private readonly Mock<IBackUpChapterRepository> _mockBackUpChapterRepository;
    private readonly Mock<IHistoryCourseRepository> _mockHistoryCourseRepository;
    private readonly Mock<IChapterRepository> _mockChapterRepository;
    private readonly Mock<IFeedBackRepository> _mockFeedBackRepository;
    private readonly Mock<IReportRepository> _mockReportRepository;
    private readonly LoggerMock<CourseService> _loggerMock;
    private readonly CourseService _courseService;
    private readonly Mock<CourseService> _mockCourseService;
    private readonly Mock<ICourseService> _mockICourseService;

    public CourseServiceTest()
    {
        // Initialize Hangfire with in-memory storage
        GlobalConfiguration.Configuration.UseMemoryStorage();
        JobStorage.Current = new MemoryStorage();
        _mockICourseService = new Mock<ICourseService>();
        _mockCourseService = new Mock<CourseService>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockBlobService = new Mock<IBlobService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockBackUpCourseRepository = new Mock<IBackUpCourseRepository>();
        _mockBackUpChapterRepository = new Mock<IBackUpChapterRepository>();
        _mockHistoryCourseRepository = new Mock<IHistoryCourseRepository>();
        _mockChapterRepository = new Mock<IChapterRepository>();
        _mockFeedBackRepository = new Mock<IFeedBackRepository>();
        _mockReportRepository = new Mock<IReportRepository>();
        _loggerMock = new LoggerMock<CourseService>();

        _courseService = new CourseService(
            _mockCourseRepository.Object,
            _mockHttpContextAccessor.Object,
            _mockUserRepository.Object,
            _mockCategoryRepository.Object,
            _mockBlobService.Object,
            _mockEmailService.Object,
            _mockBackUpCourseRepository.Object,
            _mockBackUpChapterRepository.Object,
            _mockHistoryCourseRepository.Object,
            _mockChapterRepository.Object,
            _mockFeedBackRepository.Object,
            _mockReportRepository.Object,
            _loggerMock
        );
    }


    [Fact]
    public async Task CreateCourse_ShouldReturnSuccess_WhenCourseIsCreated()
    {
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
        // Arrange
        var courseDto = new CourseDTO
        {
            Name = "NewCourse",
            CategoryIds = new List<int> { 1 },
            Description = "Description",
            Price = 100,
            ShortSummary = "Short summary",
            AllowComments = true,
            Thumbnail = mockFile.Object,
        };

        SetupMockHttpContext("valid_token");

        var user = new User { Status = UserStatus.Active };
        _mockUserRepository.Setup(repo => repo.GetAllUserById(It.IsAny<int>())).ReturnsAsync(user);
        _mockCategoryRepository.Setup(repo => repo.GetAllCategory()).ReturnsAsync(new List<int?> { 3 });
        _mockCategoryRepository.Setup(repo => repo.GetCategoryById(It.IsAny<int>()))
            .ReturnsAsync(new Category { Id = 1, ParentCategory = null });
        _mockCourseRepository.Setup(repo => repo.GetCourseByName(It.IsAny<string>())).ReturnsAsync((BackupCourse)null);
        _mockCourseRepository.Setup(repo => repo.CreateCourse(It.IsAny<Course>())).ReturnsAsync(true);
        _mockBackUpCourseRepository.Setup(repo => repo.CreateBackUpCourse(It.IsAny<BackupCourse>())).ReturnsAsync(true);

        // Act
        var result = await _courseService.CreateCourse(courseDto);

        // Assert
        Assert.Equal("Created course successfully", result.Message);
    }


    [Fact]
    public async Task GetChaptersByCourseId_ShouldReturnChapters_WhenChaptersExist()
    {
        // Arrange
        var courseId = 1;
        var chapters = new List<Chapter>
        {
            new Chapter { Id = 1, CourseId = courseId },
            new Chapter { Id = 2, CourseId = courseId }
        };

        _mockCourseRepository.Setup(repo => repo.GetChaptersByCourseId(courseId)).ReturnsAsync(chapters);

        // Act
        var result = await _courseService.GetChaptersByCourseId(courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    // [Fact]
    // public async Task EditDraft_ShouldReturnSuccess_WhenCourseIsEdited()
    // {
    //     // Arrange
    //
    //     var mockFile = new Mock<IFormFile>();
    //     var content = "Hello World from a Fake File";
    //     var fileName = "test.png";
    //     var ms = new MemoryStream();
    //     var writer = new StreamWriter(ms);
    //     writer.Write(content);
    //     writer.Flush();
    //     ms.Position = 0;
    //     mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
    //     mockFile.Setup(_ => _.FileName).Returns(fileName);
    //     mockFile.Setup(_ => _.Length).Returns(ms.Length);
    //
    //     var courseId = 1;
    //     var userId = 1;
    //     var courseDto = new CourseEditDTO()
    //     {
    //         Name = "Updated Course",
    //         CategoryIds = new List<int> { 1 },
    //         Description = "Updated Description",
    //         Price = 150,
    //         ShortSummary = "Updated Summary",
    //         AllowComments = true,
    //         Thumbnail = mockFile.Object,
    //     };
    //
    //     SetupMockHttpContext("valid_token");
    //
    //     var course = new Course
    //     {
    //         Id = courseId,
    //         Name = "Original Course",
    //         Description = "Original Description",
    //         Price = 100,
    //         Status = "Draft",
    //         Version = "1",
    //         Point = 0,
    //         ShortSummary = "Original Summary",
    //         AllowComments = true,
    //         Thumbnail = "original_thumbnail_url",
    //         InstructorId = userId
    //     };
    //
    //     _mockCourseRepository.Setup(repo => repo.GetCourseById(courseId)).ReturnsAsync(course);
    //     _mockCategoryRepository.Setup(repo => repo.GetCategoryById(It.IsAny<int>())).ReturnsAsync(new Category());
    //     _mockCourseRepository.Setup(repo => repo.EditCourse(It.IsAny<Course>())).ReturnsAsync(true);
    //     _mockCategoryRepository.Setup(repo => repo.CreateCourseCategory(It.IsAny<CourseCategory>())).ReturnsAsync(true);
    //
    //     // Act
    //     var result = await _courseService.EditDraft(courseId, courseDto);
    //
    //     // Assert
    //     Assert.Equal("Save draft success", result.Message);
    //     Assert.NotNull(result.Data);
    //     var editedCourse = result.Data as Course;
    //     Assert.Equal("Updated Course", editedCourse.Name);
    //     Assert.Equal("Updated Description", editedCourse.Description);
    //     Assert.Equal("Pending", editedCourse.Status);
    //     Assert.Equal("2", editedCourse.Version);
    // }

    [Fact]
    public async Task ReviewRejectCourse_ShouldReturnRejectedCourses()
    {
        // Arrange
        var userId = 1;
        SetupMockHttpContext("valid_token");

        var course = new Course
        {
            Id = 1,
            Name = "Course 1",
            Description = "Description 1",
            Thumbnail = "thumbnail1.png",
            ShortSummary = "Summary 1",
            InstructorId = userId,
            Point = 10,
            Price = 100,
            Status = "Rejected"
        };

        var chapter = new Chapter
        {
            CourseId = 1,
            Content = "Content 1",
            Thumbnail = "chapter_thumbnail.png",
            Order = 1,
            Duration = new TimeSpan(12),
            Type = ChapterType.DocFile
        };

        var history = new HistoryCourse
        {
            CourseId = 1,
            CreatedDate = DateTime.UtcNow,
            Description = "Rejected due to quality issues"
        };

        _mockCourseRepository.Setup(repo => repo.GetCourseByInstructorId(userId))
            .ReturnsAsync(new List<Course> { course });

        _mockCourseRepository.Setup(repo => repo.GetChaptersByCourseId(course.Id))
            .ReturnsAsync(new List<Chapter> { chapter });

        _mockHistoryCourseRepository.Setup(repo => repo.GetAllHistoryOfCourseByCourseid(course.Id))
            .ReturnsAsync(new List<HistoryCourse> { history });

        // Act
        var result = await _courseService.ReviewRejectCourse();

        // Assert
        Assert.Equal("This is your list reject course", result.Message);
        var rejectCourses = result.Data as List<RejectCourseRespone>;
        Assert.NotNull(rejectCourses);
        Assert.Single(rejectCourses);

        var rejectCourse = rejectCourses.First();
        Assert.Equal(course.Id, rejectCourse.Id);
        Assert.Equal(course.Name, rejectCourse.Name);
        Assert.Equal(course.Description, rejectCourse.Description);
        Assert.Equal(course.Thumbnail, rejectCourse.Thumbnail);
        Assert.Equal(course.ShortSummary, rejectCourse.ShortSummary);
        Assert.Equal(course.InstructorId, rejectCourse.InstructorId);
        Assert.Equal(course.Point, rejectCourse.Point);
        Assert.Equal(course.Price, rejectCourse.Price);

        var sortedChapters = rejectCourse.Chapters;
        Assert.Single(sortedChapters);
        var sortedChapter = sortedChapters.First();
        Assert.Equal(chapter.CourseId, sortedChapter.CourseId);
        Assert.Equal(chapter.Content, sortedChapter.Content);
        Assert.Equal(chapter.Thumbnail, sortedChapter.Thumbnail);
        Assert.Equal(chapter.Order, sortedChapter.Order);
        Assert.Equal(chapter.Duration, sortedChapter.Duration);
        Assert.Equal(chapter.Type, sortedChapter.Type);

        var historyCourses = rejectCourse.History;
        Assert.Single(historyCourses);
        var historyCourse = historyCourses.First();
        Assert.Equal(history.CreatedDate, historyCourse.Date);
        Assert.Equal(history.Description, historyCourse.Description);
    }


    [Fact]
    public async Task ViewSubmitCourseByAdmin_Success()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course
            {
                Id = 1,
                Name = "Test Course",
                Status = "Submitted",
                Description = "Test Description",
                Thumbnail = "Test Thumbnail",
                ShortSummary = "Test Summary",
                AllowComments = true,
                Price = 100
            }
        };

        var chapters = new List<Chapter>
        {
            new Chapter
            {
                Order = 1,
                Duration = new TimeSpan(12),
                Content = "Test Content",
                Thumbnail = "Test Thumbnail",
                Type = ChapterType.DocFile
            }
        };

        _mockCourseRepository.Setup(repo => repo.GetCourseByStatus()).ReturnsAsync(courses);
        _mockCourseRepository.Setup(repo => repo.GetChaptersByCourseId(It.IsAny<int>())).ReturnsAsync(chapters);

        // Act
        var result = await _courseService.ViewSubmitCourseByAdmin();

        // Assert
        Assert.Equal("This is course waiting to approve/reject", result.Message);
        var course = result.Data as ReviewCourseDTO;
        Assert.NotNull(course);
        Assert.Equal(1, course.Id);
        Assert.Equal("Test Course", course.Name);
        Assert.Equal("Submitted", course.Status);
        Assert.Equal("Test Description", course.Description);
        Assert.Equal("Test Thumbnail", course.Thumbnail);
        Assert.Equal("Test Summary", course.ShortSummary);
        Assert.True(course.AllowComments);
        Assert.Equal(100, course.Price);
        Assert.Single(course.Chapters);
        Assert.Equal(1, course.Chapters.First().Order);
        Assert.Equal("Test Content", course.Chapters.First().Content);
        Assert.Equal("Test Thumbnail", course.Chapters.First().Thumbnail);
    }

// [Fact]
// public async Task ExportCourseToExcelAsync_Success()
// {
//     // Arrange
//     var courseData = new List<ManageCourseDTO>
//     {
//         new ManageCourseDTO
//         {
//             Id = 1,
//             Name = "Test Course",
//             InstructorName = "Test Instructor",
//             NumberOfStudent = 50,
//             Version = "1.0",
//             TotalOfPurchased = 20,
//             Rating = 4.5,
//             CategoryName = new List<ViewCategoryNameDTO>
//             {
//                 new ViewCategoryNameDTO { CategoryName = "Category 1" },
//                 new ViewCategoryNameDTO { CategoryName = "Category 2" }
//             },
//             AdminComment = new List<CommentUserDetail>()
//         }
//     };
//
//     _mockICourseService.Setup(service => service.GetInfoCourse(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
//                       .ReturnsAsync(courseData);
//
//     // Act
//     var result = await _courseService.ExportCourseToExcelAsync();
//
//     // Assert
//     Assert.NotNull(result);
//     using (var package = new ExcelPackage(new System.IO.MemoryStream(result)))
//     {
//         var worksheet = package.Workbook.Worksheets.First();
//         var expectedDate = DateTime.Now.ToString("yyyyMMdd");
//         Assert.Equal($"ManageCourse_{expectedDate}", worksheet.Name);
//
//         // Headers
//         Assert.Equal("Id", worksheet.Cells[4, 1].Value);
//         Assert.Equal("Course Name", worksheet.Cells[4, 2].Value);
//         Assert.Equal("Instructor Name", worksheet.Cells[4, 3].Value);
//         Assert.Equal("Category Name", worksheet.Cells[4, 4].Value);
//         Assert.Equal("Number Of Student", worksheet.Cells[4, 5].Value);
//         Assert.Equal("Version", worksheet.Cells[4, 6].Value);
//         Assert.Equal("Total Of Purchase", worksheet.Cells[4, 7].Value);
//         Assert.Equal("Rating Number", worksheet.Cells[4, 8].Value);
//
//         // Data
//         var rowIndex = 5;
//         Assert.Equal(1, worksheet.Cells[rowIndex, 1].Value);
//         Assert.Equal("Test Course", worksheet.Cells[rowIndex, 2].Value);
//         Assert.Equal("Test Instructor", worksheet.Cells[rowIndex, 3].Value);
//         Assert.Equal("Category 1, Category 2", worksheet.Cells[rowIndex, 4].Value);
//         Assert.Equal(50, worksheet.Cells[rowIndex, 5].Value);
//         Assert.Equal("1.0", worksheet.Cells[rowIndex, 6].Value);
//         Assert.Equal(20, worksheet.Cells[rowIndex, 7].Value);
//         Assert.Equal(4.5, worksheet.Cells[rowIndex, 8].Value);
//     }
// }

[Fact]
public async Task GetInfoCourse_Success_ReturnsCourseInfo()
{
    // Arrange
    var pageSize = 10;
    var pageIndex = 1;
    var sortBy = "Name";
    var sortDesc = false;

    var courses = new List<Course>
    {
        new Course { Id = 1, Name = "Course 1", InstructorId = 1, Version = "v1", Price = 100 },
        new Course { Id = 2, Name = "Course 2", InstructorId = 2, Version = "v1", Price = 200 }
    };

    var instructors = new List<User>
    {
        new User { UserId = 1, FullName = "Instructor 1" },
        new User { UserId = 2, FullName = "Instructor 2" }
    };

    var categories1 = new List<Category>
    {
        new Category { CategoryName = "Category 1" }
    };

    var categories2 = new List<Category>
    {
        new Category { CategoryName = "Category 2" }
    };

    var comments1 = new List<CommentCourse>
    {
        new CommentCourse { Content = "Admin comment 1", ByAdmin = true }
    };

    var comments2 = new List<CommentCourse>
    {
        new CommentCourse { Content = "Admin comment 2", ByAdmin = true }
    };

    _mockCourseRepository.Setup(repo => repo.GetAllCoursesAsync(sortBy, sortDesc))
        .ReturnsAsync(courses);

    _mockCourseRepository.Setup(repo => repo.GetPendingCoursesAsync(pageSize, pageIndex, sortBy, sortDesc))
        .ReturnsAsync(courses);

    _mockUserRepository.Setup(repo => repo.GetAllUserById(It.IsAny<int>()))
        .ReturnsAsync((int id) => instructors.FirstOrDefault(u => u.UserId == id));

    _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(It.IsAny<int>()))
        .ReturnsAsync((int id) => id == 1 ? categories1 : categories2);

    _mockCourseRepository.Setup(repo => repo.GetCourseCommentById(It.IsAny<int>()))
        .ReturnsAsync((int id) => id == 1 ? comments1 : comments2);

    _mockCourseRepository.Setup(repo => repo.CountStudentInCourses(It.IsAny<int>()))
        .ReturnsAsync(5); // Assuming each course has 5 students

    // Act
    var result = await _courseService.GetInfoCourse(pageSize, pageIndex, sortBy, sortDesc);

    // Assert
    Assert.NotNull(result);
    var courseList = Assert.IsType<List<ManageCourseDTO>>(result);

    Assert.Equal(2, courseList.Count);

    var course1 = courseList.First(dto => dto.Id == 1);
    Assert.Equal("Course 1", course1.Name);
    Assert.Equal("Instructor 1", course1.InstructorName);
    Assert.Single(course1.CategoryName);
    Assert.Equal("Category 1", course1.CategoryName.First().CategoryName);
    Assert.Single(course1.AdminComment);
    Assert.Equal("Admin comment 1", course1.AdminComment.First().comment);
    Assert.Equal(500, course1.TotalOfPurchased); // Price * NumberOfStudent

    var course2 = courseList.First(dto => dto.Id == 2);
    Assert.Equal("Course 2", course2.Name);
    Assert.Equal("Instructor 2", course2.InstructorName);
    Assert.Single(course2.CategoryName);
    Assert.Equal("Category 2", course2.CategoryName.First().CategoryName);
    Assert.Single(course2.AdminComment);
    Assert.Equal("Admin comment 2", course2.AdminComment.First().comment);
    Assert.Equal(1000, course2.TotalOfPurchased); // Price * NumberOfStudent
    
}
    [Fact]
    public async Task CreateCommentToCourse_Success()
    {
        // Arrange
        var commentDTO = new CommentDTO { content = "This is a comment" };
        var courseId = 1;
        SetupMockHttpContext("valid_token");

        // Mock _courseRepository
        _mockCourseRepository.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(new Course { Id = courseId });

        _mockCourseRepository.Setup(repo => repo.CreateCourseComment(It.IsAny<CommentCourse>()))
            .ReturnsAsync(true);

        // Act
        var result = await _courseService.CreateCommentToCourse(commentDTO, courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Comment to course successfully", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ViewCommentDetail_Success()
    {
        // Arrange
        var courseId = 1;
        var commentsFromRepo = new List<CommentCourse>
        {
            new CommentCourse
            {
                Content = "Admin Comment 1",
                ByAdmin = true
            },
            new CommentCourse
            {
                Content = "User Comment 1",
                ByAdmin = false
            },
            new CommentCourse
            {
                Content = "Admin Comment 2",
                ByAdmin = true
            }
        };

        var expectedComments = new List<CommentUserDetail>
        {
            new CommentUserDetail { comment = "Admin Comment 1" },
            new CommentUserDetail { comment = "Admin Comment 2" }
        };

        // Mock repository
        _mockCourseRepository.Setup(repo => repo.GetCourseCommentById(courseId))
            .ReturnsAsync(commentsFromRepo);

        // Act
        var result = await _courseService.ViewCommentDetail(courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedComments.Count, result.Count);

        for (int i = 0; i < expectedComments.Count; i++)
        {
            Assert.Equal(expectedComments[i].comment, result[i].comment);
        }
    }

    [Fact]
    public async Task ViewCategoryName_ReturnsCategories_Success()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { CategoryName = "Category 1" },
            new Category { CategoryName = "Category 2" }
        };

        _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(It.IsAny<int>()))
            .ReturnsAsync(categories);

        var courseId = 1; // Example course ID

        // Act
        var result = await _courseService.ViewCategoryName(courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Category 1", result[0].CategoryName);
        Assert.Equal("Category 2", result[1].CategoryName);
    }

    [Fact]
public async Task ManageCourseDetail_Success_ReturnsCourseDetail()
{
    // Arrange
    var courseId = 1;
    var course = new Course
    {
        Id = courseId,
        Name = "Course 1",
        Description = "Description 1",
        Thumbnail = "Thumbnail1",
        ShortSummary = "Summary 1",
        Status = "Active",
        Price = 100,
        AllowComments = true,
        InstructorId = 1
    };

    var user = new User
    {
        UserId = 1,
        FullName = "Instructor 1"
    };

    var chapters = new List<Chapter>
    {
        new Chapter { Order = 1, Duration = new TimeSpan(12), Content = "Chapter 1 Content", Thumbnail = "Chapter1Thumb", Type = ChapterType.DocFile },
        new Chapter { Order = 2, Duration = new TimeSpan(12), Content = "Chapter 2 Content", Thumbnail = "Chapter2Thumb", Type = ChapterType.DocFile }
    };
    
    var categories = new List<Category>
    {
        new Category { CategoryName = "Category 1" },
        new Category { CategoryName = "Category 2" }
    };

    var comments = new List<CommentCourse>
    {
        new CommentCourse { Content = "Great course!", ByAdmin = false },
        new CommentCourse { Content = "Very informative.", ByAdmin = false }
    };

    _mockCourseRepository.Setup(repo => repo.GetCourseById(courseId))
        .ReturnsAsync(course);

    _mockUserRepository.Setup(repo => repo.GetAllUserById(course.InstructorId))
        .ReturnsAsync(user);

    _mockCourseRepository.Setup(repo => repo.GetChaptersByCourseId(courseId))
        .ReturnsAsync(chapters);

    _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(courseId))
        .ReturnsAsync(categories);

    _mockCourseRepository.Setup(repo => repo.GetCourseCommentById(courseId))
        .ReturnsAsync(comments);

    _mockCourseRepository.Setup(repo => repo.CountStudentInCourses(courseId))
        .ReturnsAsync(10); // Assuming there are 10 students

    // Act
    var result = await _courseService.ManageCourseDetail(courseId);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Review Successfully", result.Message);

    var reviewCourseDTO = Assert.IsType<ReviewCourseDTO>(result.Data);

    Assert.Equal(courseId, reviewCourseDTO.Id);
    Assert.Equal("Course 1", reviewCourseDTO.Name);
    Assert.Equal(course.Description, reviewCourseDTO.Description);
    Assert.Equal(course.Thumbnail, reviewCourseDTO.Thumbnail);
    Assert.Equal(course.ShortSummary, reviewCourseDTO.ShortSummary);
    Assert.Equal(course.Status, reviewCourseDTO.Status);
    Assert.Equal(course.Price, reviewCourseDTO.Price);
    Assert.Equal(course.AllowComments, reviewCourseDTO.AllowComments);
    Assert.Equal("Instructor 1", reviewCourseDTO.InstructorName);
    Assert.Equal(1000, reviewCourseDTO.EarnedMoney); // 100 price * 10 students

    Assert.Equal(2, reviewCourseDTO.Chapters.Count);
    Assert.Equal("Chapter 1 Content", reviewCourseDTO.Chapters[0].Content);
    Assert.Equal("Chapter2Thumb", reviewCourseDTO.Chapters[1].Thumbnail);

    Assert.Equal(2, reviewCourseDTO.StudentComment.Count);
    Assert.Equal("Great course!", reviewCourseDTO.StudentComment[0].comment);
    Assert.Equal("Very informative.", reviewCourseDTO.StudentComment[1].comment);
}

    [Fact]
    public async Task StudentCreateCommentCourse_Success()
    {
        // Arrange
        var commentDTO = new CommentDTO { content = "Great course!" };
        var courseId = 1;
        var userId = 1;

        // Set up the mock HTTP context with a valid token
        SetupMockHttpContext("valid_token");

        // Mock course repository methods
        _mockCourseRepository.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(new Course { Id = courseId });

        _mockCourseRepository.Setup(repo => repo.CreateCourseComment(It.IsAny<CommentCourse>()))
            .ReturnsAsync(true);

        // Act
        var result = await _courseService.StudentCreateCommentCourse(commentDTO, courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Comment to course successfully", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ReportCourseById_Success()
    {
        // Arrange
        var reportCourseDto = new ReportCourseDTO
        {
            Content = "Inappropriate content",
            Attachment = new FormFile(new MemoryStream(), 0, 0, "file", "file.png") // Adjust as needed
        };
        var courseId = 1;
        var userId = 1;

        // Set up the mock HTTP context with a valid token
        SetupMockHttpContext("valid_token");

        // Mock course repository methods
        _mockCourseRepository.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(new Course { Id = courseId, Status = "Active" });

        _mockCourseRepository.Setup(repo => repo.GetStudentInCourseById(userId, courseId))
            .ReturnsAsync(new StudentInCourse());

        _mockReportRepository.Setup(repo => repo.GetReportByUserId(userId, courseId))
            .ReturnsAsync((Report)null);

        _mockBlobService.Setup(service => service.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("uploaded_file_url");

        _mockCourseRepository.Setup(repo => repo.CreateReportCourse(It.IsAny<Report>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(repo => repo.GetAllUserById(It.IsAny<int>()))
            .ReturnsAsync(new User { Email = "instructor@example.com" });

        _mockEmailService.Setup(service => service.SendReportStudentMail(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.ReportCourseById(reportCourseDto, courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Report successfully", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ReportChapterById_Success()
    {
        // Arrange
        var reportCourseDto = new ReportCourseDTO
        {
            Content = "Inappropriate content",
            Attachment = new FormFile(new MemoryStream(), 0, 0, "file", "file.png") // Adjust as needed
        };
        var chapterId = 1;
        var courseId = 1;
        var userId = 1;

        // Set up the mock HTTP context with a valid token
        SetupMockHttpContext("valid_token");

        // Mock chapter repository methods
        _mockChapterRepository.Setup(repo => repo.GetChapterById(chapterId))
            .ReturnsAsync(new Chapter { Id = chapterId, CourseId = courseId });

        // Mock course repository methods
        _mockCourseRepository.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(new Course { Id = courseId, InstructorId = userId, Status = "Active" });

        _mockCourseRepository.Setup(repo => repo.GetStudentInCourseById(userId, courseId))
            .ReturnsAsync(new StudentInCourse());

        _mockReportRepository.Setup(repo => repo.GetReportByChapterId(userId, chapterId))
            .ReturnsAsync((Report)null);

        _mockBlobService.Setup(service => service.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("uploaded_file_url");

        _mockCourseRepository.Setup(repo => repo.CreateReportCourse(It.IsAny<Report>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(repo => repo.GetAllUserById(It.IsAny<int>()))
            .ReturnsAsync(new User { Email = "instructor@example.com" });

        _mockEmailService.Setup(service => service.SendReportStudentMail(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.ReportChapterById(reportCourseDto, chapterId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Report successfully", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ApproveCourseByAdmin_Success()
    {
        // Arrange
        var courseId = 1;
        var course = new Course
        {
            Id = courseId,
            Status = "Submitted",
            Name = "Sample Course",
            ShortSummary = "Summary",
            Description = "Description",
            Thumbnail = "thumbnail.png",
            Price = 100,
            Version = "v1",
            Point = 10,
            AllowComments = true,
            InstructorId = 1
        };

        var chapters = new List<Chapter>
        {
            new Chapter
            {
                Id = 1,
                Content = "Chapter Content",
                Thumbnail = "chapter-thumbnail.png",
                Order = 1,
                Duration = TimeSpan.FromMinutes(10),
                Type = ChapterType.DocFile,
                CourseId = courseId
            }
        };

        var backupCourse = new BackupCourse
        {
            Id = 1,
            Name = course.Name,
            ShortSummary = course.ShortSummary,
            Description = course.Description,
            Thumbnail = course.Thumbnail,
            Price = course.Price,
            Status = "Active",
            Version = course.Version,
            Point = course.Point,
            AllowComments = course.AllowComments,
            CourseId = course.Id
        };

        var backupChapter = new BackupChapter
        {
            Id = 1,
            Content = "Chapter Content",
            Thumbnail = "chapter-thumbnail.png",
            Order = 1,
            Duration = TimeSpan.FromMinutes(10),
            Type = ChapterType.DocFile,
            ChapterId = 1,
            BackupCourseId = 1
        };

        _mockCourseRepository.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(course);

        _mockCourseRepository.Setup(repo => repo.GetChaptersByCourseId(courseId))
            .ReturnsAsync(chapters);

        _mockBackUpCourseRepository.Setup(repo => repo.GetBackUpCourseByCourseId(courseId))
            .ReturnsAsync((BackupCourse)null);

        _mockBackUpCourseRepository.Setup(repo => repo.CreateBackUpCourse(It.IsAny<BackupCourse>()))
            .ReturnsAsync(true);

        _mockBackUpChapterRepository.Setup(repo => repo.GetBackUpChapterByChapterId(It.IsAny<int>()))
            .ReturnsAsync((BackupChapter)null);

        _mockBackUpChapterRepository.Setup(repo => repo.CreateBackUpChapter(It.IsAny<BackupChapter>()))
            .ReturnsAsync(true);

        _mockHistoryCourseRepository.Setup(repo => repo.CreateHistoryCourse(It.IsAny<HistoryCourse>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(repo => repo.GetAllUserById(course.InstructorId))
            .ReturnsAsync(new User { Email = "instructor@example.com" });

        _mockEmailService.Setup(service => service.SendApprovalCourseEmailAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.ApproveCourseByAdmin(courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(
            "The course has been successfully approved and backed up. An email has been sent to the instructor.",
            result.Message);

        // Verify interactions
        _mockCourseRepository.Verify(repo => repo.GetCourseById(courseId), Times.Once);
        _mockCourseRepository.Verify(repo => repo.GetChaptersByCourseId(courseId),
            Times.Exactly(2)); // Adjusted for multiple invocations
        _mockBackUpCourseRepository.Verify(repo => repo.CreateBackUpCourse(It.IsAny<BackupCourse>()), Times.Once);
        _mockBackUpChapterRepository.Verify(repo => repo.CreateBackUpChapter(It.IsAny<BackupChapter>()),
            Times.Exactly(chapters.Count));
        _mockHistoryCourseRepository.Verify(repo => repo.CreateHistoryCourse(It.IsAny<HistoryCourse>()), Times.Once);
        _mockEmailService.Verify(service => service.SendApprovalCourseEmailAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RejectCourseByAdmin_Success()
    {
        // Arrange
        var courseId = 1;
        var rejectReason = "Course content does not meet standards";
        var rejectCourseRequest = new RejectCourseRequest { Reason = rejectReason };

        var course = new Course
        {
            Id = courseId,
            Status = "Submitted",
            InstructorId = 1
        };

        var instructor = new User { Email = "instructor@example.com" };
        var backupCourse = new BackupCourse
        {
            Id = courseId,
            Status = "Active",
            CourseId = courseId
        };

        _mockCourseRepository.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(course);

        _mockUserRepository.Setup(repo => repo.GetAllUserById(course.InstructorId))
            .ReturnsAsync(instructor);

        _mockBackUpCourseRepository.Setup(repo => repo.GetBackUpCourseByCourseId(courseId))
            .ReturnsAsync(backupCourse);

        _mockHistoryCourseRepository.Setup(repo => repo.CreateHistoryCourse(It.IsAny<HistoryCourse>()))
            .ReturnsAsync(true);

        _mockCourseRepository.Setup(repo => repo.EditCourse(It.IsAny<Course>()))
            .ReturnsAsync(true);

        _mockEmailService
            .Setup(service => service.SendRejectionCourseEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.RejectCourseByAdmin(courseId, rejectCourseRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("This course is rejected success when create", result.Message);

        // Verify interactions
        _mockCourseRepository.Verify(repo => repo.GetCourseById(courseId), Times.Once);
        _mockUserRepository.Verify(repo => repo.GetAllUserById(course.InstructorId), Times.Once);
        _mockBackUpCourseRepository.Verify(repo => repo.GetBackUpCourseByCourseId(courseId), Times.Once);
        _mockHistoryCourseRepository.Verify(repo => repo.CreateHistoryCourse(It.IsAny<HistoryCourse>()), Times.Once);
        _mockCourseRepository.Verify(repo => repo.EditCourse(It.Is<Course>(c => c.Status == "Rejected")), Times.Once);
        _mockEmailService.Verify(service => service.SendRejectionCourseEmailAsync(instructor.Email, rejectReason),
            Times.Once);
    }

    [Fact]
    public async Task GetListEnrolledCourse_Success_ReturnsEnrolledCourses()
    {
        // Arrange

        SetupMockHttpContext("Valid_token");
        var enrolledCourses = new List<StudentInCourse>
        {
            new StudentInCourse { CourseId = 1, Progress = 80 },
            new StudentInCourse { CourseId = 2, Progress = 90 }
        };

        var courses = new List<Course>
        {
            new Course
            {
                Id = 1, Name = "Course 1", Thumbnail = "Thumb1", Price = 100, ShortSummary = "Summary 1",
                InstructorId = 1, Point = 4.5
            },
            new Course
            {
                Id = 2, Name = "Course 2", Thumbnail = "Thumb2", Price = 200, ShortSummary = "Summary 2",
                InstructorId = 2, Point = 4.0
            }
        };

        var categories1 = new List<Category>
        {
            new Category { CategoryName = "Category 1" }
        };

        var categories2 = new List<Category>
        {
            new Category { CategoryName = "Category 2" }
        };

        _mockCourseRepository.Setup(repo => repo.ListEnrolledCourse(1))
            .ReturnsAsync(enrolledCourses);

        _mockCourseRepository.Setup(repo => repo.GetCourseById(1))
            .ReturnsAsync(courses[0]);

        _mockCourseRepository.Setup(repo => repo.GetCourseById(2))
            .ReturnsAsync(courses[1]);

        _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(1))
            .ReturnsAsync(categories1);

        _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(2))
            .ReturnsAsync(categories2);

        // Act
        var result = await _courseService.GetListEnrolledCourse();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Get list student enrolled course successfully", result.Message);

        var enrolledCoursesList = Assert.IsType<List<EnrolledCourseDTO>>(result.Data);
        Assert.Equal(2, enrolledCoursesList.Count);

        var firstCourse = enrolledCoursesList[0];
        var secondCourse = enrolledCoursesList[1];

        Assert.Equal("Course 1", firstCourse.CourseName);
        Assert.Equal("Thumb1", firstCourse.CourseImageThumb);
        Assert.Equal("Summary 1", firstCourse.CourseSumary);
        Assert.Single(firstCourse.CategoryName);
        Assert.Equal("Category 1", firstCourse.CategoryName[0].CategoryName);
        Assert.Equal(80, firstCourse.Progress);
        Assert.Equal(4.5, firstCourse.Rating);

        Assert.Equal("Course 2", secondCourse.CourseName);
        Assert.Equal("Thumb2", secondCourse.CourseImageThumb);
        Assert.Equal("Summary 2", secondCourse.CourseSumary);
        Assert.Single(secondCourse.CategoryName);
        Assert.Equal("Category 2", secondCourse.CategoryName[0].CategoryName);
        Assert.Equal(90, secondCourse.Progress);
        Assert.Equal(4.0, secondCourse.Rating);
    }

    [Fact]
    public async Task ReviewCourseById_Success()
    {
        // Arrange
        var tokenId = 1;
        var courseId = 1;
        SetupMockHttpContext("valid_token");

        var feedbackCourseDto = new FeedbackCourseDTO
        {
            ReviewPoint = 4,
            Content = "Great course!",
            Attachment = null
        };

        var course = new Course
        {
            Id = courseId,
            Status = "Active",
            Point = 0
        };

        var backupCourse = new BackupCourse
        {
            Id = courseId,
            Point = 0
        };

        var studentInCourse = new StudentInCourse
        {
            CourseId = courseId,
            UserId = tokenId,
            Progress = 100
        };

        var existingFeedback = new List<Feedback>();

        _mockCourseRepository.Setup(repo => repo.GetCourseById(courseId)).ReturnsAsync(course);
        _mockBackUpCourseRepository.Setup(repo => repo.GetBackUpCourseByCourseId(courseId)).ReturnsAsync(backupCourse);
        _mockCourseRepository.Setup(repo => repo.GetStudentInCourseById(tokenId, courseId))
            .ReturnsAsync(studentInCourse);
        _mockCourseRepository.Setup(repo => repo.GetAllFeedback()).ReturnsAsync(existingFeedback);
        _mockFeedBackRepository.Setup(repo => repo.GetFeedBackByUserId(tokenId, courseId)).ReturnsAsync((Feedback)null);
        _mockCourseRepository.Setup(repo => repo.UpdateStudentInCourse(It.IsAny<StudentInCourse>())).ReturnsAsync(true);
        _mockCourseRepository.Setup(repo => repo.EditCourse(It.IsAny<Course>())).ReturnsAsync(true);
        _mockCourseRepository.Setup(repo => repo.CreateFeedbackCourse(It.IsAny<Feedback>())).ReturnsAsync(true);
        _mockCourseRepository.Setup(repo => repo.GetStudentInCourseByCourseId(courseId)).ReturnsAsync(4.0);

        // Act
        var result = await _courseService.ReviewCourseById(feedbackCourseDto, courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Feedback successfully", result.Message);
        _mockCourseRepository.Verify(repo => repo.GetCourseById(courseId), Times.Once);
        _mockBackUpCourseRepository.Verify(repo => repo.GetBackUpCourseByCourseId(courseId), Times.Once);
        _mockCourseRepository.Verify(repo => repo.UpdateStudentInCourse(It.IsAny<StudentInCourse>()), Times.Once);
        _mockCourseRepository.Verify(repo => repo.EditCourse(It.IsAny<Course>()), Times.Once);
        _mockCourseRepository.Verify(repo => repo.CreateFeedbackCourse(It.IsAny<Feedback>()), Times.Once);
        _mockCourseRepository.Verify(repo => repo.GetStudentInCourseByCourseId(courseId), Times.Once);
    }


    [Fact]
    public async Task GetListBookmarkedCourse_Success_ReturnsBookmarkedCourses()
    {
        // Arrange

        SetupMockHttpContext("valid_token");

        var bookmarkedCourses = new List<BookmarkedCourse>
        {
            new BookmarkedCourse { CourseId = 1 },
            new BookmarkedCourse { CourseId = 2 }
        };

        var courses = new List<Course>
        {
            new Course
            {
                Id = 1, Name = "Course 1", Thumbnail = "Thumb1", Price = 100, ShortSummary = "Summary 1",
                InstructorId = 1
            },
            new Course
            {
                Id = 2, Name = "Course 2", Thumbnail = "Thumb2", Price = 200, ShortSummary = "Summary 2",
                InstructorId = 2
            }
        };

        var instructors = new List<User>
        {
            new User { UserId = 1, FullName = "Instructor 1" },
            new User { UserId = 2, FullName = "Instructor 2" }
        };

        var categories = new List<List<Category>>
        {
            new List<Category> { new Category { CategoryName = "Category 1" } },
            new List<Category> { new Category { CategoryName = "Category 2" } }
        };

        _mockCourseRepository.Setup(repo => repo.ViewListBookmarkedCourse(1))
            .ReturnsAsync(bookmarkedCourses);

        _mockCourseRepository.Setup(repo => repo.GetCourseById(1))
            .ReturnsAsync(courses[0]);

        _mockCourseRepository.Setup(repo => repo.GetCourseById(2))
            .ReturnsAsync(courses[1]);

        _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(1))
            .ReturnsAsync(categories[0]);

        _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(2))
            .ReturnsAsync(categories[1]);

        // Act
        var result = await _courseService.GetListBookmarkedCourse();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Get list bookmark successfully", result.Message);

        var bookmarkedCoursesList = Assert.IsType<List<BookMarkedCourseDTO>>(result.Data);
        Assert.Equal(2, bookmarkedCoursesList.Count);

        var firstCourse = bookmarkedCoursesList[0];
        var secondCourse = bookmarkedCoursesList[1];

        Assert.Equal("Course 1", firstCourse.CourseName);
        Assert.Equal("Thumb1", firstCourse.CourseImageThumb);
        Assert.Equal(100, firstCourse.CoursePrice);
        Assert.Equal("Summary 1", firstCourse.CourseSumary);
        Assert.Single(firstCourse.CategoryName);
        Assert.Equal("Category 1", firstCourse.CategoryName[0].CategoryName);

        Assert.Equal("Course 2", secondCourse.CourseName);
        Assert.Equal("Thumb2", secondCourse.CourseImageThumb);
        Assert.Equal(200, secondCourse.CoursePrice);
        Assert.Equal("Summary 2", secondCourse.CourseSumary);
        Assert.Single(secondCourse.CategoryName);
        Assert.Equal("Category 2", secondCourse.CategoryName[0].CategoryName);
    }

    [Fact]
    public async Task GetCoursesAsync_Success_ReturnsCourses()
    {
        // Arrange
        var instructorId = 1;
        var pageSize = 10;
        var pageIndex = 1;
        var sortBy = "Name";
        var sortDesc = false;

        var instructor = new User() { UserId = instructorId };
        var courses = new List<Course>
        {
            new Course
            {
                Name = "Course 1",
                Description = "Description 1",
                Thumbnail = "thumb1.jpg",
                ShortSummary = "Summary 1",
                AllowComments = true,
                Price = 100
            },
            new Course
            {
                Name = "Course 2",
                Description = "Description 2",
                Thumbnail = "thumb2.jpg",
                ShortSummary = "Summary 2",
                AllowComments = false,
                Price = 200
            }
        };

        _mockCourseRepository.Setup(repo => repo.GetInstructorByIdAsync(instructorId))
            .ReturnsAsync(instructor);
        _mockCourseRepository.Setup(repo => repo.GetCoursesAsync(instructorId, pageSize, pageIndex, sortBy, sortDesc))
            .ReturnsAsync(courses);

        // Act
        var result = await _courseService.GetCoursesAsync(instructorId, pageSize, pageIndex, sortBy, sortDesc);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, ((List<CourseResponse>)result.Data).Count);
        Assert.Equal("Course 1", ((List<CourseResponse>)result.Data).First().Name);
        Assert.Equal("thumb1.jpg", ((List<CourseResponse>)result.Data).First().Thumbnail);
        _mockCourseRepository.Verify(repo => repo.GetInstructorByIdAsync(instructorId), Times.Once);
        _mockCourseRepository.Verify(repo => repo.GetCoursesAsync(instructorId, pageSize, pageIndex, sortBy, sortDesc),
            Times.Once);
    }


    [Fact]
    public async Task GetCoursesByInstructorAsync_Success_ReturnsCourses()
    {
        // Arrange
        var pageSize = 10;
        var pageIndex = 1;
        var sortBy = "Name";
        var sortDesc = false;
        CourseStatus? status = null;
        var instructorId = 1;
        var courses = new List<Course>
        {
            new Course
            {
                Name = "Course 1",
                Description = "Description 1",
                Thumbnail = "thumb1.jpg",
                ShortSummary = "Summary 1",
                AllowComments = true,
                Price = 100
            },
            new Course
            {
                Name = "Course 2",
                Description = "Description 2",
                Thumbnail = "thumb2.jpg",
                ShortSummary = "Summary 2",
                AllowComments = false,
                Price = 200
            }
        };

        SetupMockHttpContext("valid_token");

        _mockCourseRepository.Setup(repo =>
                repo.GetCoursesByInstructorAsync(instructorId, pageSize, pageIndex, sortBy, sortDesc, status))
            .ReturnsAsync(courses);

        // Act
        var result = await _courseService.GetCoursesByInstructorAsync(pageSize, pageIndex, sortBy, sortDesc, status);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Data);
        var courseDtoList = (List<CourseResponse>)result.Data;
        Assert.Equal(2, courseDtoList.Count);
        Assert.Equal("Course 1", courseDtoList.First().Name);
        Assert.Equal("thumb1.jpg", courseDtoList.First().Thumbnail);
        _mockCourseRepository.Verify(
            repo => repo.GetCoursesByInstructorAsync(instructorId, pageSize, pageIndex, sortBy, sortDesc, status),
            Times.Once);
    }

    [Fact]
    public async Task GetAllActiveCoursesAsync_CoursesFound_ReturnsCourseList()
    {
        // Arrange
        int pageSize = 10;
        int pageIndex = 1;
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1", InstructorId = 1, CreatedDate = DateTime.Now },
            new Course { Id = 2, Name = "Course 2", InstructorId = 2, CreatedDate = DateTime.Now }
        };

        var instructor1 = new User() { UserId = 1, FullName = "Instructor 1" };
        var instructor2 = new User() { UserId = 2, FullName = "Instructor 2" };

        var categories1 = new List<Category>
        {
            new Category { CategoryName = "Category 1" },
            new Category { CategoryName = "Category 2" }
        };

        var categories2 = new List<Category>
        {
            new Category { CategoryName = "Category 3" },
            new Category { CategoryName = "Category 4" }
        };

        _mockCourseRepository.Setup(repo => repo.GetActiveCoursesAsync(pageSize, pageIndex))
            .ReturnsAsync(courses);

        _mockCourseRepository.Setup(repo => repo.GetInstructorByIdAsync(1))
            .ReturnsAsync(instructor1);

        _mockCourseRepository.Setup(repo => repo.GetInstructorByIdAsync(2))
            .ReturnsAsync(instructor2);

        _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(1))
            .ReturnsAsync(categories1);

        _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(2))
            .ReturnsAsync(categories2);

        // Act
        var result = await _courseService.GetAllActiveCoursesAsync(pageSize, pageIndex);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        var courseList = Assert.IsType<List<ViewAndSearchDTO>>(result.Data);
        Assert.Equal(2, courseList.Count);

        Assert.Equal("Course 1", courseList[0].Name);
        Assert.Equal("Instructor 1", courseList[0].InstructorName);
        Assert.Equal(2, courseList[0].CategoryName.Count);
        Assert.Equal("Category 1", courseList[0].CategoryName[0].CategoryName);

        Assert.Equal("Course 2", courseList[1].Name);
        Assert.Equal("Instructor 2", courseList[1].InstructorName);
        Assert.Equal(2, courseList[1].CategoryName.Count);
        Assert.Equal("Category 3", courseList[1].CategoryName[0].CategoryName);
    }

    [Fact]
    public async Task GetTopCategoriesAsync_Success()
    {
        // Arrange
        var topCategories = new List<CategoryResponse>
        {
            new CategoryResponse { CategoryId = 1, Course = 10, RatingPoint = 4.5 },
            new CategoryResponse { CategoryId = 2, Course = 15, RatingPoint = 4.0 },
            new CategoryResponse { CategoryId = 3, Course = 8, RatingPoint = 4.8 }
        };

        var categoryResponses = new List<Category>
        {
            new Category { Id = 1, CategoryName = "Category 1" },
            new Category { Id = 2, CategoryName = "Category 2" },
            new Category { Id = 3, CategoryName = "Category 3" }
        };

        _mockCourseRepository.Setup(repo => repo.GetTopCategoriesAsync(It.IsAny<int>())).ReturnsAsync(topCategories);
        _mockCategoryRepository.Setup(repo => repo.GetCategoryById(It.IsAny<int>()))
            .ReturnsAsync((int id) => categoryResponses.FirstOrDefault(c => c.Id == id));

        // Act
        var result = await _courseService.GetTopCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        var categoryList = Assert.IsType<List<CategoryResponse>>(result.Data);
        Assert.Equal(3, categoryList.Count);
        Assert.Contains(categoryList, c => c.CategoryId == 1 && c.CategoryName == "Category 1");
        Assert.Contains(categoryList, c => c.CategoryId == 2 && c.CategoryName == "Category 2");
        Assert.Contains(categoryList, c => c.CategoryId == 3 && c.CategoryName == "Category 3");
    }

    [Fact]
    public async Task GetTopFeedbacksAsync_Success()
    {
        // Arrange
        var topFeedbacks = new List<Feedback>
        {
            new Feedback
            {
                Content = "Great course!", ReviewPoint = 5, User = new User { FullName = "John Doe" },
                Course = new Course { Name = "Course 1" }
            },
            new Feedback
            {
                Content = "Very informative.", ReviewPoint = 4, User = new User { FullName = "Jane Smith" },
                Course = new Course { Name = "Course 2" }
            }
        };

        _mockCourseRepository.Setup(repo => repo.GetTopFeedbacksAsync(It.IsAny<int>())).ReturnsAsync(topFeedbacks);

        // Act
        var result = await _courseService.GetTopFeedbacksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        var feedbackList = Assert.IsType<List<FeedbackResponse>>(result.Data);
        Assert.Equal(2, feedbackList.Count);
        Assert.Contains(feedbackList,
            f => f.Content == "Great course!" && f.UserName == "John Doe" && f.CourseName == "Course 1");
        Assert.Contains(feedbackList,
            f => f.Content == "Very informative." && f.UserName == "Jane Smith" && f.CourseName == "Course 2");
    }

    [Fact]
    public async Task GetHeaderAsync_ReturnsHeaderDTO()
    {
        // Arrange
        var header = new Header
        {
            BranchName = "Main Branch",
            SupportHotline = "123-456-7890"
        };

        _mockCourseRepository.Setup(repo => repo.GetHeaderAsync()).ReturnsAsync(header);

        // Act
        var result = await _courseService.GetHeaderAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Main Branch", result.BranchName);
        Assert.Equal("123-456-7890", result.SupportHotline);
    }

    [Fact]
    public async Task GetFooterAsync_ReturnsFooterDTO()
    {
        // Arrange
        var footer = new Footer
        {
            PhoneNumber = "123-456-7890",
            Address = "123 Main St",
            WorkingTime = "9 AM - 5 PM",
            Privacy = "Privacy Policy",
            Term_of_use = "Terms of Use"
        };

        _mockCourseRepository.Setup(repo => repo.GetFooterAsync()).ReturnsAsync(footer);

        // Act
        var result = await _courseService.GetFooterAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123-456-7890", result.PhoneNumber);
        Assert.Equal("123 Main St", result.Address);
        Assert.Equal("9 AM - 5 PM", result.WorkingTime);
        Assert.Equal("Privacy Policy", result.Privacy);
        Assert.Equal("Terms of Use", result.Team_of_use);
    }

    [Fact]
    public async Task UpdateHeaderAsync_ReturnsUpdatedHeaderDTO()
    {
        // Arrange
        var headerDto = new HeaderDTO
        {
            BranchName = "New Branch",
            SupportHotline = "123-456-7890"
        };

        var header = new Header
        {
            BranchName = "Old Branch",
            SupportHotline = "098-765-4321"
        };

        var updatedHeader = new Header
        {
            BranchName = "New Branch",
            SupportHotline = "123-456-7890"
        };

        _mockCourseRepository.Setup(repo => repo.GetHeaderAsync()).ReturnsAsync(header);
        _mockCourseRepository.Setup(repo => repo.UpdateHeaderAsync(It.IsAny<Header>())).ReturnsAsync(updatedHeader);

        // Act
        var result = await _courseService.UpdateHeaderAsync(headerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("New Branch", result.Data.BranchName);
        Assert.Equal("123-456-7890", result.Data.SupportHotline);

        _mockCourseRepository.Verify(repo => repo.GetHeaderAsync(), Times.Once);
        _mockCourseRepository.Verify(repo => repo.UpdateHeaderAsync(It.IsAny<Header>()), Times.Once);
    }

    [Fact]
    public async Task GetTopPurchasedCoursesAsync_CoursesFoundWithinDateRange_ReturnsSuccessResponse()
    {
        // Arrange
        var courseIds = new List<int> { 1, 2 };
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 12, 31);

        var courses = new List<Course>
        {
            new Course
            {
                Id = 1, Name = "Course 1", CreatedDate = new DateTime(2023, 6, 1), Thumbnail = "thumb1.jpg",
                InstructorId = 1, Point = 4.5, Price = 100, ShortSummary = "Summary 1", Description = "Description 1"
            },
            new Course
            {
                Id = 2, Name = "Course 2", CreatedDate = new DateTime(2023, 7, 1), Thumbnail = "thumb2.jpg",
                InstructorId = 2, Point = 4.7, Price = 150, ShortSummary = "Summary 2", Description = "Description 2"
            }
        };

        var instructors = new List<User>
        {
            new User { UserId = 1, FullName = "Instructor 1" },
            new User { UserId = 2, FullName = "Instructor 2" }
        };

        var categories = new List<Category>
        {
            new Category { CategoryName = "Category 1" }
        };

        _mockCourseRepository.Setup(repo => repo.GetTopPurchasedCoursesAsync(It.IsAny<int>()))
            .ReturnsAsync(courseIds);
        _mockCourseRepository.Setup(repo => repo.GetCourseById(It.IsAny<int>()))
            .ReturnsAsync((int id) => courses.Find(c => c.Id == id));
        _mockCourseRepository.Setup(repo => repo.GetInstructorByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => instructors.Find(i => i.UserId == id));
        _mockCourseRepository.Setup(repo => repo.CountStudentsInCourseAsync(It.IsAny<int>()))
            .ReturnsAsync(100);
        _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(It.IsAny<int>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _courseService.GetTopPurchasedCoursesAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Data);
        var courseDto = Assert.IsType<List<ViewAndSearchDTO>>(result.Data);
        Assert.Equal(2, courseDto.Count);
    }

    [Fact]
    public async Task GetTopBadCoursesAsync_CoursesFoundWithinDateRange_ReturnsSuccessResponse()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course
            {
                Id = 1, Name = "Course 1", CreatedDate = new DateTime(2023, 6, 1), Thumbnail = "thumb1.jpg",
                InstructorId = 1, Point = 2.5, Price = 100, ShortSummary = "Summary 1", Description = "Description 1"
            },
            new Course
            {
                Id = 2, Name = "Course 2", CreatedDate = new DateTime(2023, 7, 1), Thumbnail = "thumb2.jpg",
                InstructorId = 2, Point = 2.7, Price = 150, ShortSummary = "Summary 2", Description = "Description 2"
            }
        };

        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 12, 31);

        var instructors = new List<User>
        {
            new User { UserId = 1, FullName = "Instructor 1" },
            new User { UserId = 2, FullName = "Instructor 2" }
        };

        var categories = new List<Category>
        {
            new Category { CategoryName = "Category 1" }
        };

        _mockCourseRepository.Setup(repo => repo.GetTopBadCoursesAsync(It.IsAny<int>()))
            .ReturnsAsync(courses);
        _mockCourseRepository.Setup(repo => repo.GetInstructorByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => instructors.Find(i => i.UserId == id));
        _mockCategoryRepository.Setup(repo => repo.GetCategoryNameByCourseId(It.IsAny<int>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _courseService.GetTopBadCoursesAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Data);
        var courseDto = Assert.IsType<List<ViewAndSearchDTO>>(result.Data);
        Assert.Equal(2, courseDto.Count);
    }

    [Fact]
    public async Task GetTopInstructorPayoutsAsync_PayoutsFoundWithinDateRange_ReturnsSuccessResponse()
    {
        // Arrange
        var payouts = new List<InstructorPayoutDTO>
        {
            new InstructorPayoutDTO { InstructorId = 1, PayoutAmount = 1000, PayoutDate = new DateTime(2023, 6, 1) },
            new InstructorPayoutDTO { InstructorId = 2, PayoutAmount = 1500, PayoutDate = new DateTime(2023, 7, 1) }
        };

        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 12, 31);

        var instructors = new List<User>
        {
            new User { UserId = 1, FullName = "Instructor 1" },
            new User { UserId = 2, FullName = "Instructor 2" }
        };

        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1" },
            new Course { Id = 2, Name = "Course 2" }
        };

        _mockCourseRepository.Setup(repo => repo.GetTopInstructorPayoutsAsync(It.IsAny<int>()))
            .ReturnsAsync(payouts);
        _mockCourseRepository.Setup(repo => repo.GetInstructorByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => instructors.Find(i => i.UserId == id));
        _mockCourseRepository.Setup(repo => repo.GetCoursesByInstructorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(courses);

        // Act
        var result = await _courseService.GetTopInstructorPayoutsAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Data);
        var payoutDto = Assert.IsType<List<InstructorPayoutDTO>>(result.Data);
        Assert.Equal(2, payoutDto.Count);
    }

    [Fact]
    public async Task ViewCourseName_CoursesFound_ReturnsCourseList()
    {
        // Arrange
        var instructorId = 1;
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Course 1" },
            new Course { Id = 2, Name = "Course 2" }
        };

        _mockCourseRepository.Setup(repo => repo.GetCoursesByInstructorIdAsync(instructorId))
            .ReturnsAsync(courses);

        // Act
        var result = await _courseService.ViewCourseName(instructorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Course 1", result[0].Name);
        Assert.Equal("Course 2", result[1].Name);
    }

    [Fact]
    public async Task UpdateFooterAsync_FooterExists_UpdatesFooter()
    {
        // Arrange
        var footerDto = new FooterDTO
        {
            PhoneNumber = "1234567890",
            Address = "123 Main St",
            WorkingTime = "9 AM - 5 PM",
            Privacy = "Privacy Policy",
            Team_of_use = "Terms of Use"
        };

        var existingFooter = new Footer
        {
            PhoneNumber = "0987654321",
            Address = "456 Elm St",
            WorkingTime = "10 AM - 6 PM",
            Privacy = "Old Privacy Policy",
            Term_of_use = "Old Terms of Use"
        };

        var updatedFooter = new Footer
        {
            PhoneNumber = "1234567890",
            Address = "123 Main St",
            WorkingTime = "9 AM - 5 PM",
            Privacy = "Privacy Policy",
            Term_of_use = "Terms of Use"
        };

        _mockCourseRepository.Setup(repo => repo.GetFooterAsync())
            .ReturnsAsync(existingFooter);

        _mockCourseRepository.Setup(repo => repo.UpdateFooterAsync(It.IsAny<Footer>()))
            .ReturnsAsync(updatedFooter);

        // Act
        var result = await _courseService.UpdateFooterAsync(footerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        Assert.Equal(footerDto.PhoneNumber, result.Data.PhoneNumber);
        Assert.Equal(footerDto.Address, result.Data.Address);
        Assert.Equal(footerDto.WorkingTime, result.Data.WorkingTime);
    }

    [Fact]
    public async Task UpdateStatusCourse_Success_ChangesStatusToInactiveAndSchedulesReactivation()
    {
        // Arrange
        var changeStatusCourseRequest = new ChangeStatusCourseRequest
        {
            Reason = "Reason for deactivation",
            DeactivationPeriod = 1,
            ChangeByTime = 0
        };

        var course = new Course
        {
            Id = 1,
            Status = "Active",
            InstructorId = 1
        };

        var instructor = new User
        {
            Email = "instructor@example.com"
        };

        _mockCourseRepository.Setup(repo => repo.GetCourseById(1))
            .ReturnsAsync(course);

        _mockUserRepository.Setup(repo => repo.GetAllUserById(1))
            .ReturnsAsync(instructor);

        _mockCourseRepository.Setup(repo => repo.EditCourse(It.IsAny<Course>()))
            .ReturnsAsync(true);

        _mockEmailService.Setup(service => service.SendChangeStatusEmailAsync(instructor.Email, changeStatusCourseRequest.Reason))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.UpdateStatusCourse(changeStatusCourseRequest, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Active", course.Status);
        Assert.Equal("Reason for deactivation", course.Reason);
        Assert.True(course.AdminModified);
        Assert.Equal("Update course successfully", result.Message);
        
    }

    private void SetupMockHttpContext(string token)
    {
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(req => req.Headers).Returns(new HeaderDictionary
        {
            { "Authorization", $"Bearer {token}" }
        });

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(ctx => ctx.Request).Returns(mockRequest.Object);

        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

        var key = "your-256-bit-secret-key-1234567890123456";
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

        var jwtToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken(
            issuer: "testIssuer",
            audience: "testAudience",
            subject: new ClaimsIdentity(new Claim[] { new Claim("id", "1") }),
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        mockRequest.Setup(req => req.Headers["Authorization"]).Returns(new[] { $"Bearer {tokenString}" });
    }
}