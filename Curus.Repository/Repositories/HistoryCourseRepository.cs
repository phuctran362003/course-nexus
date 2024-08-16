using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using HistoryCourse = Curus.Repository.Entities.HistoryCourse;

namespace Curus.Repository.Repositories;

public class HistoryCourseRepository : IHistoryCourseRepository
{
    private readonly CursusDbContext _context;

    public HistoryCourseRepository(CursusDbContext context)
    {
        _context = context;
    }

    public async Task<List<HistoryCourse>> GetAllHistoryOfCourseByCourseid(int id)
    {
        return await _context.HistoryCourses.Where(h => h.CourseId == id).ToListAsync();
    }

    public async Task<bool> CreateHistoryCourse(HistoryCourse historyCourse)
    {
        await _context.HistoryCourses.AddAsync(historyCourse);
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