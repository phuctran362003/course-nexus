using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curus.Repository;

public class InstructorRepository : IInstructorRepository
{
    private readonly CursusDbContext _context;
    private readonly ILogger<InstructorRepository> _logger;

    public InstructorRepository(CursusDbContext context, ILogger<InstructorRepository> logger)
    {
        _context = context;
        _logger = logger;
    }


    // Add and update instructor data
    public async Task AddAsync(InstructorData instructorData)
    {
        await _context.InstructorData.AddAsync(instructorData);
        await _context.SaveChangesAsync();
    }

    public async Task AddInstructorAsync(InstructorData instructorData)
    {
        await _context.InstructorData.AddAsync(instructorData);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(InstructorData instructorData)
    {
        _context.InstructorData.Update(instructorData);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateCourseAsync(Course course)
    {
        _context.Courses.Update(course);
        return await SaveChangeAsync();
    }

    // Get instructor and user details
    public async Task<InstructorData> GetInstructorByIdAsync(int userId)
    {
        return await _context.InstructorData.FirstOrDefaultAsync(id => id.UserId == userId);
    }


    public async Task<User> GetInstructorROLEByIdAsync(int instructorId)
    {
        _logger.LogInformation($"Fetching instructor with ID {instructorId}");

        var instructor = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == instructorId && u.Role.RoleName == "Instructor");

        if (instructor == null)
        {
            _logger.LogError($"Instructor with ID {instructorId} not found");
        }
        else
        {
            _logger.LogInformation($"Instructor with ID {instructorId} found: {instructor.FullName}");
        }

        return instructor;
    }




    public async Task<List<User>> GetAllInstructorAsync()
    {
        return await _context.Users
            .Where(u => u.Role.Id == 3)
            .ToListAsync();
    }

    public async Task<List<User>> GetPendingInstructorsAsync()
    {
        return await _context.Users
                     .Where(u => u.Status == UserStatus.Pending && u.RoleId == 3)
                     .Include(u => u.InstructorData)
                     .ToListAsync();
    }


    public async Task<List<Course>> GetAllCourseOfInstructor(int id)
    {
        return await _context.Courses
            .Where(c => c.InstructorId == id)
            .ToListAsync();
    }

    // Other methods for users, comments, and courses
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(e => e.UserId == id && e.Role.Id == 3);
    }

    public async Task<User> GetByUserIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<StudentInCourse>> GetStudentInCourse(int id)
    {
        return await _context.StudentInCourses
            .Include(s => s.Courses)
            .Where(s => s.InstructorId == id && s.Courses.Any(c => c.Status == "Active"))
            .ToListAsync();
    }
    
    public async Task CreateCommentUserAsync(CommentUser? commentUser)
    {
        _context.CommentUsers.Add(commentUser);
        await _context.SaveChangesAsync();
    }

    public async Task<List<CommentUser?>> GetCommentById(int id)
    {
        return await _context.CommentUsers
            .Where(c => c.UserId == id)
            .ToListAsync();
    }

    public async Task<CommentUser?> GetCommentByCommentId(int id)
    {
        return await _context.CommentUsers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task EditCommentByCommentId(CommentUser commentUser)
    {
        _context.CommentUsers.Update(commentUser);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCommentByCommentId(CommentUser commentUser)
    {
        _context.CommentUsers.Remove(commentUser);
        await _context.SaveChangesAsync();
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
}
