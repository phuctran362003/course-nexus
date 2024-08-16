using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Curus.Repository.Repositories;

public class ReportFeedbackRepository : IReportFeedbackRepository
{
    private readonly CursusDbContext _context;

    public ReportFeedbackRepository(CursusDbContext context)
    {
        _context = context;
    }

    public async Task<ReportFeedback> GetReportFeedbackByFeedbackId(int id)
    {
        return await _context.ReportFeedbacks.FirstOrDefaultAsync(rf => rf.FeedbackId == id);
    }

    public async Task<bool> CreateReportFeedback(ReportFeedback reportFeedback)
    {
        await _context.ReportFeedbacks.AddAsync(reportFeedback);
        return await SaveChangeAsync();
    }

    public async Task<ReportFeedback> GetReportFeedbackById(int id)
    {
        return await _context.ReportFeedbacks.FirstOrDefaultAsync(rf => rf.Id == id && rf.IsDelete == false);
    }

    public async Task<bool> EditReportFeedback(ReportFeedback reportFeedback)
    {
        _context.ReportFeedbacks.Update(reportFeedback);
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