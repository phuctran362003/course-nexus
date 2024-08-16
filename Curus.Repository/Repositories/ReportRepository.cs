using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly CursusDbContext _context;

    public ReportRepository(CursusDbContext context)
    {
        _context = context;
    }

    public async Task<Report> GetReportByUserId(int id, int courseId)
    {
        return await _context.Reports
            .FirstOrDefaultAsync(r => r.UserId == id && r.CourseId == courseId);
    }

    public async Task<Report> GetReportByChapterId(int id,int chapterId)
    {
        return await _context.Reports
            .FirstOrDefaultAsync(r => r.UserId == id && r.ChapterId == chapterId);
    }
}