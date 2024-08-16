using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CursusDbContext _context;

        public CategoryRepository(CursusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            try
            {
                return await _context.Categories
                    .Where(c => !c.IsDelete.HasValue || !c.IsDelete.Value)
                    .Include(c => c.ParentCategory)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching categories", ex);
            }
        }

        public async Task AddCategory(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding category", ex);
            }
        }

        public async Task UpdateCategory(Category category)
        {
            try
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating category", ex);
            }
        }

        public async Task<Category> GetCategoryById(int id)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.ParentCategory)
                    .SingleOrDefaultAsync(c => c.Id == id && (!c.IsDelete.HasValue || !c.IsDelete.Value));
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching category by id", ex);
            }
        }

        public async Task SoftDeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category != null)
                {
                    category.IsDelete = true;
                    category.DeletedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error soft deleting category", ex);
            }
        }

        public async Task<List<Category>> GetCategoryNameByCourseId(int id)
        {
            return await _context.CourseCategories.Where(c => c.CourseId == id).Select(ct => ct.Category).ToListAsync();
        }

        public async Task<bool> CreateCourseCategory(CourseCategory courseCategory)
        {
            await _context.CourseCategories.AddAsync(courseCategory);
            return await SaveChangeAsync();
        }

        public async Task<List<int?>> GetAllCategory()
        {
            return await _context.Categories
                .Select(c => c.ParentCategoryId)
                .ToListAsync();
        }

        public async Task DeleteCourseCategory(int id)
        {
            try
            {
                var categories = await _context.CourseCategories.Where(c => c.CourseId == id).ToListAsync();
                _context.CourseCategories.RemoveRange(categories);
            }
            catch (Exception ex)
            {
                throw new Exception("Error soft deleting category", ex);
            }
        }

        public async Task<bool> SaveChangeAsync()
        {
            var check = await _context.SaveChangesAsync();
            if (check == 0)
            {
                return false;
            }

            return true;
        }
    }
}