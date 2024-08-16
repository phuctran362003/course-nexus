using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository.Repositories;

public class StudentInCourseRepository : IStudentInCourseRepository
{
    private readonly CursusDbContext _context;

    public StudentInCourseRepository(CursusDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentInCourse>> GetCourseByInstructorId(int id)
    {
        return await _context.StudentInCourses.Where(s => s.InstructorId == id).ToListAsync();
    }

    public async Task<List<StudentInCourse>> GetStudentDashboardCourse(int id, DateTime filter)
    {
        return await _context.StudentInCourses
            .Where(o => o.UserId == id && o.CreatedDate >= filter)
            .ToListAsync();
    }

    public async Task<List<StudentInCourse>> GetCourseByStudentById(int id)
    {
        return await _context.StudentInCourses
            .Where(st => st.UserId == id)
            .ToListAsync();
    }
    public async Task<StudentInCourse> GetCourseByUser(int id, int userId)
    {
        return await _context.StudentInCourses.FirstOrDefaultAsync(sic => sic.CourseId == id && sic.UserId == userId);
    }

    public async Task FinishCourse(int courseId, int userId)
    {
        var getCourse = await _context.StudentInCourses.FirstOrDefaultAsync(sic => sic.CourseId == courseId && sic.UserId == userId);
        getCourse.IsFinish = true;
        await _context.SaveChangesAsync();
    }
}