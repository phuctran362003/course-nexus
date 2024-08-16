using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CursusDbContext _context;

    public CourseRepository(CursusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasCoursesInCategory(int categoryId)
    {
        try
        {
            return await _context.CourseCategories.AnyAsync(cc => cc.CategoryId == categoryId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error checking courses in category", ex);
        }
    }

    public async Task<bool> CreateCourse(Course course)
    {
        await _context.Courses.AddAsync(course);
        return await SaveChangeAsync();
    }

    public async Task<List<Chapter>> GetChaptersByCourseId(int id)
    {
        return await _context.Chapters.Where(ch => ch.CourseId == id).ToListAsync();
    }

    public async Task<BackupCourse> GetCourseByName(string name)
    {
        return await _context.BackupCourses.FirstOrDefaultAsync(n => n.Name == name);
    }

    public async Task<Course> GetCourseById(int? id)
    {
        return await _context.Courses.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Course>> GetAllCourseAsync()
    {
        string checkStatus = CourseStatus.Pending.ToString();
        return await _context.Courses
            .Where(s => s.Status != checkStatus)
            .ToListAsync();
    }

    public async Task<List<CommentCourse>> GetCourseCommentById(int id)
    {
        return await _context.CommentCourses
            .Where(c => c.CourseId == id)
            .ToListAsync();
    }
    public async Task<List<Course>> GetPendingCoursesAsync(int pageSize, int pageIndex, string sortBy, bool sortDesc)
    {
        var query = _context.Courses.AsQueryable();

        query = query.Where(c => c.Status != CourseStatus.Pending.ToString());

        query = Sorting(query, sortBy, sortDesc);

        var courses = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return courses;
    }

    public async Task<User> GetInstructorByIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(id => id.UserId == userId);
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.Include(c => c.ParentCategory).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Course>> GetCoursesAsync(int instructorId, int pageSize, int pageIndex, string sortBy, bool sortDesc)
    {
        var query = _context.Courses.AsQueryable();

        query = query.Where(c => c.InstructorId == instructorId && c.Status == CourseStatus.Active.ToString());

        query = Sorting(query, sortBy, sortDesc);

        var courses = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return courses;
    }

    public async Task<List<Course>> GetCoursesByInstructorAsync(int instructorId, int pageSize, int pageIndex,
        string sortBy, bool sortDesc, CourseStatus? status = null)
    {
        var query = _context.Courses.AsQueryable();

        query = query.Where(c => c.InstructorId == instructorId && c.Status == status.ToString());

        query = Sorting(query, sortBy, sortDesc);

        var courses = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return courses;
    }

    public async Task<List<Course>> GetActiveCoursesAsync(int pageSize, int pageIndex)
    {
        return await _context.Courses
            .Where(c => c.Status == CourseStatus.Active.ToString())
            .OrderByDescending(c => c.Point)
            .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<List<int>> GetCategoryIdsByNameAsync(string categoryName)
    {
        return await _context.Categories
            .Where(c => c.CategoryName.ToLower().Contains(categoryName.ToLower()))
            .Select(c => c.Id)
            .ToListAsync();
    }

    public async Task<List<Course>> GetCoursesByCategoryIdsAsync(List<int> categoryIds, int pageSize, int pageIndex)
    {
        return await _context.Courses
            .Include(c => c.CourseCategories)
            .Where(c => c.CourseCategories.Any(cc => categoryIds.Contains(cc.CategoryId)))
            .OrderByDescending(c => c.Point)
            .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<List<Course>> GetCoursesByInstructorNameAsync(string instructorName, int pageSize, int pageIndex)
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .Where(c => c.Instructor.FullName.ToLower().Contains(instructorName.ToLower()))
            .OrderByDescending(c => c.Point)
            .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<List<Course>> GetTopCoursesAsync(int count)
    {
        return await _context.Courses
            .Where(c => c.Status == CourseStatus.Active.ToString())
            .OrderByDescending(c => c.Point)
            .Take(count)
            .ToListAsync();

    }

    public async Task<List<CategoryResponse>> GetTopCategoriesAsync(int count)
    {
        return await _context.Courses
            .Where(c => c.Status == CourseStatus.Active.ToString())
            .SelectMany(c => c.CourseCategories, (course, courseCategory) => new { course, courseCategory })
            .GroupBy(cc => cc.courseCategory.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                CourseCount = g.Count(),
                AveragePoint = g.Average(x => x.course.Point)
            })
            .OrderByDescending(c => c.AveragePoint)
            .Take(count)
            .Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                Course = c.CourseCount,
                RatingPoint = Math.Round((double)c.AveragePoint, 1)
            })
            .ToListAsync();
    }

    public async Task<List<Feedback>> GetTopFeedbacksAsync(int count)
    {
        return await _context.Feedbacks.Where(f => f.IsDelete.HasValue)
            .OrderByDescending(f => f.ReviewPoint)
            .Include(f => f.User)
            .Include(f => f.Course)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> SaveChangeAsync()
    {
        var check = await _context.SaveChangesAsync();
        if (check != 0)
        {
            return true;
        }

        return false;
    }

    private IQueryable<Course> Sorting(IQueryable<Course> query, string sortBy, bool sortDesc)
    {
        switch (sortBy?.ToLower())
        {
            case "id":
                query = sortDesc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id);
                break;

            case "name":
                query = sortDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);
                break;

            case "status":
                query = sortDesc ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status);
                break;

            default:
                query = sortDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);
                break;
        }

        return query;
    }

    public async Task<bool> EditCourse(Course course)
    {
        _context.Courses.Update(course);
        return await SaveChangeAsync();
    }

    public async Task<List<StudentInCourse>> GetStudentInCourse(int id)
    {
        return await _context.StudentInCourses.Where(s => s.CourseId == id).ToListAsync();
    }

    public async Task<List<Course>> GetCourseByInstructorId(int id)
    {
        return await _context.Courses.Where(c => c.InstructorId == id).ToListAsync();
    }

    public async Task<List<Course>> GetCourseByStatus()
    {
        return await _context.Courses.Where(c => c.Status == "Submitted").ToListAsync();
    }

    public async Task<bool> CreateCourseComment(CommentCourse? commentCourse)
    {
        await _context.CommentCourses.AddAsync(commentCourse);
        var check = await _context.SaveChangesAsync();
        if (check == 0)
        {
            return false;
        }

        return true;
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync(string sortBy, bool sortDesc)
    {
        var query = _context.Courses.AsQueryable();

        query = query.Where(c => c.Status != CourseStatus.Pending.ToString());

        query = Sorting(query, sortBy, sortDesc);

        return await query.ToListAsync();
    }

    public async Task<int> CountStudentInCourses(int id)
    {
        var check = await _context.StudentInCourses.Where(i => i.CourseId == id).CountAsync();
        return check;
    }

    public async Task<bool> CreateReportCourse(Report report)
    {
        await _context.Reports.AddAsync(report);
        var check = await _context.SaveChangesAsync();
        if (check == 0)
        {
            return false;
        }

        return true;
    }

    public async Task<List<StudentInCourse>> ListEnrolledCourse(int userId)
    {
        var enrolledCourses = _context.StudentInCourses
            .Where(b => b.UserId == userId).ToList();
        return enrolledCourses;
    }

    public async Task<List<BookmarkedCourse>> ViewListBookmarkedCourse(int userId)
    {
        var bookmarkedCourses = _context.BookmarkedCourses
            .Where(b => b.UserId == userId)
            .ToList();
        return bookmarkedCourses;
    }

    public async Task<Header> GetHeaderAsync()
    {
        return await _context.Headers.FirstOrDefaultAsync();
    }

    public async Task<Footer> GetFooterAsync()
    {
        return await _context.Footers.FirstOrDefaultAsync();
    }

    public async Task<Header> UpdateHeaderAsync(Header header)
    {
        _context.Headers.Update(header);
        await _context.SaveChangesAsync();
        return header;
    }

    public async Task<Footer> UpdateFooterAsync(Footer footer)
    {
        _context.Footers.Update(footer);
        await _context.SaveChangesAsync();
        return footer;
    }

    public async Task<List<Course>> GetCourseSuggest()
    {
        return await _context.Courses
            .OrderByDescending(c=>c.Point)
            .ToListAsync();
    }

    public async Task<bool> CreateFeedbackCourse(Feedback feedback)
    {
        await _context.Feedbacks.AddAsync(feedback);
        return await SaveChangeAsync();
    }

    public async Task<StudentInCourse> GetStudentInCourseById(int id, int courseId)
    {
        return await _context.StudentInCourses
            .FirstOrDefaultAsync(sic => sic.UserId == id && sic.CourseId == courseId);
    }

    public async Task<bool> UpdateStudentInCourse(StudentInCourse studentInCourse)
    {
        _context.StudentInCourses.Update(studentInCourse);
        return await SaveChangeAsync();
    }

    public async Task<List<Feedback>> GetAllFeedback()
    {
        return await _context.Feedbacks.ToListAsync();
    }

    public async Task<double> GetStudentInCourseByCourseId(int id)
    {
        return await _context.StudentInCourses.Where(c => c.CourseId == id).AverageAsync(r => r.Rating);
    }
    public async Task<int> CountStudentsInCourseAsync(int courseId)
    {
        return await _context.StudentInCourses.CountAsync(sic => sic.CourseId == courseId);
    }

    public async Task<List<int>> GetTopPurchasedCoursesAsync(int count)
    {
        return await _context.StudentInCourses
            .GroupBy(sic => sic.CourseId)
            .OrderByDescending(c => c.Count())
            .Take(count)
            .Select(c => c.Key)
            .ToListAsync();
    }

    public async Task<List<Course>> GetTopBadCoursesAsync(int count)
    {
        return await _context.Courses
          .Where(c => c.Status == CourseStatus.Active.ToString())
          .OrderBy(c => c.Point)
          .Take(count)
          .ToListAsync();
    }

    public async Task<List<InstructorPayoutDTO>> GetTopInstructorPayoutsAsync(int count)
    {
        return await _context.InstructorPayouts
        .GroupBy(p => p.InstructorId)
        .Select(g => new InstructorPayoutDTO
        {
            InstructorId = g.Key,
            PayoutAmount = g.Sum(p => p.PayoutAmount),
            PayoutDate = g.Max(p => p.PayoutDate)
        })
        .OrderByDescending(g => g.PayoutAmount)
        .Take(count)
        .ToListAsync();
    }

    public async Task<List<Course>> GetCoursesByInstructorIdAsync(int instructorId)
    {
        return await _context.Courses
            .Where(c => c.InstructorId == instructorId)
            .Select(c => new Course
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }
}