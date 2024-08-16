
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Microsoft.Extensions.Logging;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository categoryRepository, ICourseRepository courseRepository, ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<CategoryResponseDTO> GetCategoryById(int id)
    {
        try
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null)
                throw new ArgumentException("Category not found");

            var categoryDto = new CategoryResponseDTO
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                Status = category.Status,
                ParentCategory = category.ParentCategory != null ? new ParentCategoryResponseDTO
                {
                    Id = category.ParentCategory.Id,
                    CategoryName = category.ParentCategory.CategoryName,
                    Description = category.ParentCategory.Description
                } : null
            };

            return categoryDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching category by id");
            throw new InvalidOperationException("Error fetching category by id", ex);
        }
    }

    public async Task<PagedResult<CategoryResponseDTO>> GetCategories(int pageNumber, int pageSize, CategorySortOptions sortBy, SortDirection sortDirection)
{
    try
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var categories = await _categoryRepository.GetAllCategories();

        var sortedCategories = categories.AsQueryable();
        switch (sortBy)
        {
            case CategorySortOptions.CategoryName:
                sortedCategories = sortDirection == SortDirection.Desc ? sortedCategories.OrderByDescending(c => c.CategoryName) : sortedCategories.OrderBy(c => c.CategoryName);
                break;
            case CategorySortOptions.Status:
                sortedCategories = sortDirection == SortDirection.Desc ? sortedCategories.OrderByDescending(c => c.Status) : sortedCategories.OrderBy(c => c.Status);
                break;
            default:
                sortedCategories = sortDirection == SortDirection.Desc ? sortedCategories.OrderByDescending(c => c.Id) : sortedCategories.OrderBy(c => c.Id);
                break;
        }

        var pagedCategories = sortedCategories.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        var categoryDtos = pagedCategories.Select(category => new CategoryResponseDTO
        {
            Id = category.Id,
            CategoryName = category.CategoryName,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            Status = category.Status,
            ParentCategory = category.ParentCategory != null ? new ParentCategoryResponseDTO
            {
                Id = category.ParentCategory.Id,
                CategoryName = category.ParentCategory.CategoryName,
                Description = category.ParentCategory.Description
            } : null
        }).ToList();

        var pagedResult = new PagedResult<CategoryResponseDTO>
        {
            Items = categoryDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = categories.Count()
        };

        return pagedResult;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching categories");
        throw new InvalidOperationException("Error fetching categories", ex);
    }
}



    public async Task<CategoryResponseDTO> CreateCategory(CategoryDTO categoryDto)
    {
        try
        {
            if (categoryDto.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetCategoryById(categoryDto.ParentCategoryId.Value);
                if (parentCategory == null)
                    throw new ArgumentException("Parent category not found");
            }

            var category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                ParentCategoryId = categoryDto.ParentCategoryId,
                Status = categoryDto.Status
            };

            await _categoryRepository.AddCategory(category);

            return new CategoryResponseDTO
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                Status = category.Status,
                ParentCategory = category.ParentCategory != null ? new ParentCategoryResponseDTO
                {
                    Id = category.ParentCategory.Id,
                    CategoryName = category.ParentCategory.CategoryName,
                    Description = category.ParentCategory.Description
                } : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            throw new InvalidOperationException("Error creating category", ex);
        }
    }

    public async Task UpdateCategory(int id, CategoryDTO categoryDto)
    {
        try
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null)
                throw new ArgumentException("Category not found");

            category.CategoryName = categoryDto.CategoryName;
            category.Description = categoryDto.Description;
            category.ParentCategoryId = categoryDto.ParentCategoryId;
            category.Status = categoryDto.Status;

            await _categoryRepository.UpdateCategory(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating category with id {id}");
            throw new InvalidOperationException("Error updating category", ex);
        }
    }

    public async Task DeactivateCategory(int id)
    {
        try
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null)
                throw new ArgumentException("Category not found");

            var hasCourses = await _courseRepository.HasCoursesInCategory(id);
            if (hasCourses)
                throw new InvalidOperationException("Cannot deactivate a category with courses.");

            category.Status = CategoryStatus.Deactivated;
            await _categoryRepository.UpdateCategory(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deactivating category with id {id}");
            throw new InvalidOperationException("Error deactivating category", ex);
        }
    }

    public async Task ActivateCategory(int id)
    {
        try
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null)
                throw new ArgumentException("Category not found");

            category.Status = CategoryStatus.Activated;
            await _categoryRepository.UpdateCategory(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error activating category with id {id}");
            throw new InvalidOperationException("Error activating category", ex);
        }
    }

    public async Task DeleteCategory(int id)
    {
        try
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null)
                throw new ArgumentException("Category not found");

            var hasCourses = await _courseRepository.HasCoursesInCategory(id);
            if (hasCourses)
                throw new InvalidOperationException("Cannot delete a category with courses.");

            await _categoryRepository.SoftDeleteCategory(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting category with id {id}");
            throw new InvalidOperationException("Error deleting category", ex);
        }
    }
}


