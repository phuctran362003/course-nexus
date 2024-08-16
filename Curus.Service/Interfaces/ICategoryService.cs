using Curus.Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;

namespace Curus.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseDTO> GetCategoryById(int id);

        Task<PagedResult<CategoryResponseDTO>> GetCategories(int pageNumber, int pageSize, CategorySortOptions sortBy, SortDirection sortDirection);
        Task<CategoryResponseDTO> CreateCategory(CategoryDTO categoryDto);
        Task UpdateCategory(int id, CategoryDTO categoryDto);
        Task DeactivateCategory(int id);
        Task ActivateCategory(int id);
        Task DeleteCategory(int id);
    }
}
