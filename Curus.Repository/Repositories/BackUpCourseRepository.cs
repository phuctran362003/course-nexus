using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository.Repositories;

public class BackUpCourseRepository : IBackUpCourseRepository
{
    private readonly CursusDbContext _context;

    public BackUpCourseRepository(CursusDbContext context)
    {
        _context = context;
    }
    public async Task<bool> CreateBackUpCourse(BackupCourse backupCourse)
    {
        await _context.BackupCourses.AddAsync(backupCourse);
        return await SaveChangeAsync();
    }

    public async Task<BackupCourse> GetBackUpCourseByCourseId(int? id)
    {
        return await _context.BackupCourses.FirstOrDefaultAsync(b => b.CourseId == id);
    }

    public async Task<bool> EditBackUpCourse(BackupCourse backupCourse)
    {
        _context.BackupCourses.Update(backupCourse);
        return await SaveChangeAsync();
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