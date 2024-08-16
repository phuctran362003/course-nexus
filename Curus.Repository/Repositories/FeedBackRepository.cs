using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository.Repositories;

public class FeedBackRepository : IFeedBackRepository
{
    private readonly CursusDbContext _context;

    public FeedBackRepository(CursusDbContext context)
    {
        _context = context;
    }

    public async Task<List<Feedback>> GetFeedbackByCourseId(int id)
    {
        return await _context.Feedbacks
            .Where(f => f.CourseId == id)
            .Include(u => u.User)
            .OrderByDescending(c => c.Id)
            .ToListAsync();
    }

    public async Task<Feedback> GetFeedbackById(int id)
    {
        return await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<bool> UpdateFeedback(Feedback feedback)
    {
        _context.Feedbacks.Update(feedback);
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

    public async Task<Feedback> GetFeedBackByUserId(int id, int courseId)
    {
        return await _context.Feedbacks
            .Where(c => c.UserId == id && c.CourseId == courseId)
            .FirstOrDefaultAsync();
    }
}