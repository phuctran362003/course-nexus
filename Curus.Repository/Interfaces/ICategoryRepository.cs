    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task<Category> GetCategoryById(int id);
        Task SoftDeleteCategory(int id);
        
        Task<List<Category>> GetCategoryNameByCourseId(int id);

        Task<bool> CreateCourseCategory(CourseCategory courseCategory);

        Task<List<int?>> GetAllCategory();

        Task DeleteCourseCategory(int id);

    }
}
