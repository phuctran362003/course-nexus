using Curus.API.Controllers;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;
using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Curus.Tests.Controllers
{
    public class ChapterControllerTests
    {
        private readonly Mock<IChapterService> _mockChapterService;
        private readonly ChapterController _chapterController;

        public ChapterControllerTests()
        {
            _mockChapterService = new Mock<IChapterService>();
            _chapterController = new ChapterController(_mockChapterService.Object);
        }

        [Fact]
        public async Task CreateChapter_ReturnsOkResult_WithChapterId()
        {
            // Arrange
            var chapterDto = new ChapterDTO
            {
                CourseId = 1,
                Content = new FormFile(null, 0, 0, null, "test.mp4"),
                Thumbnail = new FormFile(null, 0, 0, null, "thumbnail.jpg"),
                Order = 1,
                Type = ChapterType.VideoFile
            };

            var userResponse = new UserResponse<object>("Chapter created successfully", new { ChapterId = 1 });
            _mockChapterService.Setup(service => service.CreateChapter(It.IsAny<ChapterDTO>()))
                               .ReturnsAsync(userResponse);

            // Act
            var result = await _chapterController.CreateChapter(chapterDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
            Assert.Equal("Chapter created successfully", returnValue.Message);
            Assert.NotNull(returnValue.Data);
            Assert.Equal(1, ((dynamic)returnValue.Data).ChapterId);

            _mockChapterService.Verify(service => service.CreateChapter(It.IsAny<ChapterDTO>()), Times.Once);
        }

        [Fact]
        public async Task UpdateChapter_ReturnsOkResult_WithSuccessMessage()
        {
            // Arrange
            var updateChapterDto = new UpdateChapterDTO
            {
                Content = new FormFile(null, 0, 0, null, "test.mp4"), // Use appropriate mock file
                Thumbnail = new FormFile(null, 0, 0, null, "thumbnail.jpg"),
                Type = ChapterType.VideoFile
            };

            var userResponse = new UserResponse<object>("Update chapter successfully", null);
            _mockChapterService.Setup(service => service.UpdateChapter(It.IsAny<int>(), It.IsAny<UpdateChapterDTO>()))
                               .ReturnsAsync(userResponse);

            // Act
            var result = await _chapterController.UpdateChapter(1, updateChapterDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
            Assert.Equal("Update chapter successfully", returnValue.Message);

            _mockChapterService.Verify(service => service.UpdateChapter(It.IsAny<int>(), It.IsAny<UpdateChapterDTO>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderChapter_ReturnsOkResult_WithSuccessMessage()
        {
            // Arrange
            var updateOrderChapterDto = new UpdateOrderChapterDTO
            {
                Order = 2
            };

            var userResponse = new UserResponse<object>("Update chapter successfully", null);
            _mockChapterService.Setup(service => service.UpdateOrderChapter(It.IsAny<int>(), It.IsAny<UpdateOrderChapterDTO>()))
                               .ReturnsAsync(userResponse);

            // Act
            var result = await _chapterController.UpdateOrderChapter(1, updateOrderChapterDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
            Assert.Equal("Update chapter successfully", returnValue.Message);

            _mockChapterService.Verify(service => service.UpdateOrderChapter(It.IsAny<int>(), It.IsAny<UpdateOrderChapterDTO>()), Times.Once);
        }

        [Fact]
        public async Task DeleteChapterById_ReturnsOkResult_WithSuccessMessage()
        {
            // Arrange
            var userResponse = new UserResponse<object>("Delete chapter successfully", null);
            _mockChapterService.Setup(service => service.DeleteChapterById(It.IsAny<int>()))
                               .ReturnsAsync(userResponse);

            // Act
            var result = await _chapterController.DeleteChapterById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserResponse<object>>(okResult.Value);
            Assert.Equal("Delete chapter successfully", returnValue.Message);

            _mockChapterService.Verify(service => service.DeleteChapterById(It.IsAny<int>()), Times.Once);
        }
    }
}
