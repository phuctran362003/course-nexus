using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Assert = Xunit.Assert;
using Curus.Test.Utils;

namespace Curus.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly LoggerMock<CategoryService> _loggerMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockCourseRepository = new Mock<ICourseRepository>();
            _loggerMock = new LoggerMock<CategoryService>();
            _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockCourseRepository.Object, _loggerMock);
        }

        [Fact]
        public async Task GetCategories_ReturnsPagedResult()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, CategoryName = "Category 1", Description = "Description 1", Status = CategoryStatus.Activated },
                new Category { Id = 2, CategoryName = "Category 2", Description = "Description 2", Status = CategoryStatus.Activated }
            };
            _mockCategoryRepository.Setup(repo => repo.GetAllCategories()).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetCategories(1, 10, CategorySortOptions.Id, SortDirection.Asc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.Items.First().Id);
        }

        [Fact]
        public async Task GetCategories_InvalidPageNumber_UsesDefault()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, CategoryName = "Category 1", Description = "Description 1", Status = CategoryStatus.Activated }
            };
            _mockCategoryRepository.Setup(repo => repo.GetAllCategories()).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetCategories(-1, 10, CategorySortOptions.Id, SortDirection.Asc);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.PageNumber); // Default page number should be 1
        }

        [Fact]
        public async Task GetCategories_InvalidPageSize_UsesDefault()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, CategoryName = "Category 1", Description = "Description 1", Status = CategoryStatus.Activated }
            };
            _mockCategoryRepository.Setup(repo => repo.GetAllCategories()).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetCategories(1, -1, CategorySortOptions.Id, SortDirection.Asc);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(10, result.PageSize); // Default page size should be 10
        }

        [Fact]
        public async Task GetCategories_SortByCategoryName_Descending()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, CategoryName = "B Category", Description = "Description 1", Status = CategoryStatus.Activated },
                new Category { Id = 2, CategoryName = "A Category", Description = "Description 2", Status = CategoryStatus.Activated }
            };
            _mockCategoryRepository.Setup(repo => repo.GetAllCategories()).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetCategories(1, 10, CategorySortOptions.CategoryName, SortDirection.Desc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("B Category", result.Items.First().CategoryName);
        }

        [Fact]
        public async Task GetCategories_ThrowsException_LogsError()
        {
            // Arrange
            _mockCategoryRepository.Setup(repo => repo.GetAllCategories()).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _categoryService.GetCategories(1, 10, CategorySortOptions.Id, SortDirection.Asc));

            // Assert
            var logEntry = _loggerMock.LogEntries.FirstOrDefault();
            Assert.NotNull(logEntry);
            Assert.Equal(LogLevel.Error, logEntry.LogLevel);
            Assert.Equal("Error fetching categories", logEntry.Message);
        }
    }
}
