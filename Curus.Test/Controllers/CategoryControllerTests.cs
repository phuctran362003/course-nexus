using Curus.API.Controllers;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Curus.Tests.Controllers;

public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly CategoryController _categoryController;

        public CategoryControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _categoryController = new CategoryController(_mockCategoryService.Object);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsOkResult_WithListOfCategories()
        {
            // Arrange
            var pagedResult = new PagedResult<CategoryResponseDTO>
            {
                Items = new List<CategoryResponseDTO>
                {
                    new CategoryResponseDTO { Id = 1, CategoryName = "Category 1" },
                    new CategoryResponseDTO { Id = 2, CategoryName = "Category 2" }
                },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 2
            };
            _mockCategoryService.Setup(service => service.GetCategories(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CategorySortOptions>(), It.IsAny<SortDirection>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _categoryController.GetAllCategories(1, 10, CategorySortOptions.Id, SortDirection.Asc);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PagedResult<CategoryResponseDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Items.Count);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsOkResult_WithCategory()
        {
            // Arrange
            var category = new CategoryResponseDTO { Id = 1, CategoryName = "Category 1" };
            _mockCategoryService.Setup(service => service.GetCategoryById(It.IsAny<int>())).ReturnsAsync(category);

            // Act
            var result = await _categoryController.GetCategoryById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryResponseDTO>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNotFoundResult_WhenCategoryNotFound()
        {
            // Arrange
            _mockCategoryService.Setup(service => service.GetCategoryById(It.IsAny<int>())).ReturnsAsync((CategoryResponseDTO)null);

            // Act
            var result = await _categoryController.GetCategoryById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateCategory_ReturnsCreatedAtActionResult_WithCategory()
        {
            // Arrange
            var categoryDto = new CategoryDTO { CategoryName = "Category 1", Description = "Description 1" };
            var createdCategory = new CategoryResponseDTO { Id = 1, CategoryName = "Category 1" };
            _mockCategoryService.Setup(service => service.CreateCategory(It.IsAny<CategoryDTO>())).ReturnsAsync(createdCategory);

            // Act
            var result = await _categoryController.CreateCategory(categoryDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<CategoryResponseDTO>(createdAtActionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Category 1", returnValue.CategoryName);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsNoContentResult()
        {
            // Arrange
            var categoryDto = new CategoryDTO { CategoryName = "Updated Category", Description = "Updated Description" };

            // Act
            var result = await _categoryController.UpdateCategory(1, categoryDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockCategoryService.Verify(service => service.UpdateCategory(1, categoryDto), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNoContentResult()
        {
            // Act
            var result = await _categoryController.DeleteCategory(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockCategoryService.Verify(service => service.DeleteCategory(1), Times.Once);
        }

        [Fact]
        public async Task DeactivateCategory_ReturnsNoContentResult()
        {
            // Act
            var result = await _categoryController.DeactivateCategory(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockCategoryService.Verify(service => service.DeactivateCategory(1), Times.Once);
        }

        [Fact]
        public async Task ActivateCategory_ReturnsNoContentResult()
        {
            // Act
            var result = await _categoryController.ActivateCategory(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockCategoryService.Verify(service => service.ActivateCategory(1), Times.Once);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockCategoryService.Setup(service => service.GetCategories(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CategorySortOptions>(), It.IsAny<SortDirection>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _categoryController.GetAllCategories(1, 10, CategorySortOptions.Id, SortDirection.Asc);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
    }