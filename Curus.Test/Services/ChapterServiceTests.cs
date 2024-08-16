using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Service.Services;
using Curus.Service.Interfaces;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Assert = Xunit.Assert;
using Curus.Repository.Entities;
using Microsoft.AspNetCore.Http;
using System.IO.Packaging;
using Curus.Repository.ViewModels.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Curus.Test.UTils;
using Microsoft.IdentityModel.Tokens;

namespace Curus.Tests
{
    public class ChapterServiceTests
    {
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly Mock<IChapterRepository> _mockChapterRepository;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly Mock<IVideoService> _mockVideoService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ILogger<ChapterService>> _mockLogger;
        private readonly ChapterService _chapterService;

        public ChapterServiceTests()
        {
            _mockCourseRepository = new Mock<ICourseRepository>();
            _mockChapterRepository = new Mock<IChapterRepository>();
            _mockBlobService = new Mock<IBlobService>();
            _mockVideoService = new Mock<IVideoService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLogger = new Mock<ILogger<ChapterService>>();

            _chapterService = new ChapterService(
                _mockChapterRepository.Object,
                _mockBlobService.Object,
                _mockCourseRepository.Object,
                _mockVideoService.Object,
                _mockHttpContextAccessor.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task CreateChapter_Success()
        {
            // Arrange

            var videoFile = new Mock<IFormFile>();
            var contents = "Hello World from a Fake File";
            var fileNames = "test.mp4";
            var mss = new MemoryStream();
            var writers = new StreamWriter(mss);
            writers.Write(contents);
            writers.Flush();
            mss.Position = 0;
            videoFile.Setup(_ => _.OpenReadStream()).Returns(mss);
            videoFile.Setup(_ => _.FileName).Returns(fileNames);
            videoFile.Setup(_ => _.Length).Returns(mss.Length);

            var chapterDto = new ChapterDTO
            {
                CourseId = 1,
                Content = videoFile.Object,
                Order = 1,
                Type = ChapterType.VideoFile
            };

            var course = new Course
            {
                Id = 1,
                Status = "Active",
                Thumbnail = "existing-thumbnail-url"
            };

            _mockCourseRepository.Setup(repo => repo.GetCourseById(It.IsAny<int>()))
                                 .ReturnsAsync(course);

            _mockBlobService.Setup(service => service.UploadFileAsync(It.IsAny<IFormFile>()))
                            .ReturnsAsync("fileUrl");

            _mockVideoService.Setup(service => service.GetVideoDuration(It.IsAny<IFormFile>()))
                             .ReturnsAsync(TimeSpan.FromMinutes(5));

            _mockChapterRepository.Setup(repo => repo.CreateChapter(It.IsAny<Chapter>()))
                                  .ReturnsAsync(true);

            // Act
            var result = await _chapterService.CreateChapter(chapterDto);

            // Assert
            Assert.Equal("Chapter created successfully", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task UpdateChapter_Success()
        {
            // Arrange
            var updateChapterDto = new UpdateChapterDTO
            {
                Thumbnail = new Mock<IFormFile>().Object,
                Content = new Mock<IFormFile>().Object,
                Type = ChapterType.VideoFile
            };

            var chapter = new Chapter
            {
                Id = 1,
                CourseId = 1,
                Type = ChapterType.DocFile
            };

            var course = new Course
            {
                Id = 1,
                Status = "Active"
            };

            _mockChapterRepository.Setup(repo => repo.GetChapterById(It.IsAny<int>()))
                                  .ReturnsAsync(chapter);

            _mockBlobService.Setup(service => service.UploadFileAsync(It.IsAny<IFormFile>()))
                            .ReturnsAsync("fileUrl");

            _mockVideoService.Setup(service => service.GetVideoDuration(It.IsAny<IFormFile>()))
                             .ReturnsAsync(TimeSpan.FromMinutes(5));

            _mockCourseRepository.Setup(repo => repo.GetCourseById(It.IsAny<int>()))
                                 .ReturnsAsync(course);

            _mockCourseRepository.Setup(repo => repo.EditCourse(It.IsAny<Course>()))
                       .ReturnsAsync(true);

            _mockChapterRepository.Setup(repo => repo.UpdateChapter(It.IsAny<Chapter>()))
                                  .ReturnsAsync(true);

            var thumbnailMock = new Mock<IFormFile>();
            thumbnailMock.Setup(file => file.FileName).Returns("thumbnail.jpg");

            var contentMock = new Mock<IFormFile>();
            contentMock.Setup(file => file.FileName).Returns("video.mp4");

            updateChapterDto.Thumbnail = thumbnailMock.Object;
            updateChapterDto.Content = contentMock.Object;

            // Act
            var result = await _chapterService.UpdateChapter(1, updateChapterDto);

            // Assert
            Assert.Equal("Update chapter successfully", result.Message);
            Assert.Null(result.Data);

            _mockChapterRepository.Verify(repo => repo.GetChapterById(It.IsAny<int>()), Times.Once);
            _mockBlobService.Verify(service => service.UploadFileAsync(It.IsAny<IFormFile>()), Times.Exactly(2));
            _mockVideoService.Verify(service => service.GetVideoDuration(It.IsAny<IFormFile>()), Times.Once);
            _mockCourseRepository.Verify(repo => repo.GetCourseById(It.IsAny<int>()), Times.Once);
            _mockCourseRepository.Verify(repo => repo.EditCourse(It.IsAny<Course>()), Times.Once);
            _mockChapterRepository.Verify(repo => repo.UpdateChapter(It.IsAny<Chapter>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderChapter_Success()
        {
            // Arrange
            var updateOrderChapterDto = new UpdateOrderChapterDTO
            {
                Order = 2
            };

            var chapter = new Chapter
            {
                Id = 1,
                Order = 1,
                CourseId = 1
            };

            var course = new Course
            {
                Id = 1,
                Status = "Active"
            };

            _mockChapterRepository.Setup(repo => repo.GetChapterById(It.IsAny<int>()))
                                  .ReturnsAsync(chapter);

            _mockCourseRepository.Setup(repo => repo.GetCourseById(It.IsAny<int>()))
                                 .ReturnsAsync(course);

            _mockCourseRepository.Setup(repo => repo.EditCourse(It.IsAny<Course>()))
                                 .ReturnsAsync(true);  // Ensure EditCourse returns true to indicate success

            _mockChapterRepository.Setup(repo => repo.UpdateChapter(It.IsAny<Chapter>()))
                                  .ReturnsAsync(true);  // Ensure UpdateChapter returns true to indicate success

            // Act
            var result = await _chapterService.UpdateOrderChapter(1, updateOrderChapterDto);

            // Assert
            Assert.Equal("Update chapter successfully", result.Message);
            Assert.Null(result.Data);

            _mockChapterRepository.Verify(repo => repo.GetChapterById(It.IsAny<int>()), Times.Once);
            _mockCourseRepository.Verify(repo => repo.GetCourseById(It.IsAny<int>()), Times.Once);
            _mockCourseRepository.Verify(repo => repo.EditCourse(It.IsAny<Course>()), Times.Once);
            _mockChapterRepository.Verify(repo => repo.UpdateChapter(It.IsAny<Chapter>()), Times.Once);
        }

        [Fact]
        public async Task DeleteChapterById_Success()
        {
            // Arrange
            var chapter = new Chapter
            {
                Id = 1
            };

            _mockChapterRepository.Setup(repo => repo.GetChapterById(It.IsAny<int>()))
                                  .ReturnsAsync(chapter);

            _mockChapterRepository.Setup(repo => repo.DeleteChapterById(It.IsAny<Chapter>()))
                                  .ReturnsAsync(true);  // Ensure DeleteChapterById returns true to indicate success

            // Act
            var result = await _chapterService.DeleteChapterById(1);

            // Assert
            Assert.Equal("Delete chapter successfully", result.Message);
            Assert.Null(result.Data);

            _mockChapterRepository.Verify(repo => repo.GetChapterById(It.IsAny<int>()), Times.Once);
            _mockChapterRepository.Verify(repo => repo.DeleteChapterById(It.IsAny<Chapter>()), Times.Once);
        }

        [Fact]
        public async Task StartChapterById_ReturnsSuccess_WhenValidData()
        {
            // Arrange
            var chapterId = 1;
            var userId = 1;
            SetupMockHttpContext("valid token");
            var chapter = new Chapter { Id = chapterId, CourseId = 1, IsStart = false };
            var course = new Course { Id = 1, Status = "Active" };
            var studentInCourse = new StudentInCourse { CourseId = 1, UserId = userId, Progress = 0 };

            _mockChapterRepository.Setup(repo => repo.GetChapterById(chapterId)).ReturnsAsync(chapter);
            _mockCourseRepository.Setup(repo => repo.GetCourseById(chapter.CourseId)).ReturnsAsync(course);
            _mockCourseRepository.Setup(repo => repo.GetStudentInCourseById(userId, course.Id)).ReturnsAsync(studentInCourse);
            _mockChapterRepository.Setup(repo => repo.StartChapterById(course.Id)).ReturnsAsync(new List<Chapter> { chapter });
            _mockChapterRepository.Setup(repo => repo.UpdateChapter(It.IsAny<Chapter>())).ReturnsAsync(true);
            _mockCourseRepository.Setup(repo => repo.UpdateStudentInCourse(It.IsAny<StudentInCourse>())).ReturnsAsync(true);


            // Act
            var result = await _chapterService.StartChapterById(chapterId);

            // Assert
            var response = Assert.IsType<UserResponse<object>>(result);
            Assert.Equal("Start chapter successfully", response.Message);
            var chapterResponse = Assert.IsType<ChapterRespone>(response.Data);
            Assert.Equal(chapter.CourseId, chapterResponse.CourseId);
            Assert.Equal(chapter.Content, chapterResponse.Content);
            Assert.Equal(chapter.Type, chapterResponse.Type);
            Assert.Equal(chapter.Thumbnail, chapterResponse.Thumbnail);
            Assert.Equal(chapter.Order, chapterResponse.Order);
            Assert.Equal(chapter.Duration, chapterResponse.Duration);
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
}
