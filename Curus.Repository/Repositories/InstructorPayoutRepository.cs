using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository.Repositories;

public class InstructorPayoutRepository : IInstructorPayoutRepository
{
    private readonly CursusDbContext _context;

    public InstructorPayoutRepository(CursusDbContext context)
    {
        _context = context;
    }

    public async Task AddPayoutRequest(InstructorPayout payoutRequest)
    {
        _context.InstructorPayouts.Add(payoutRequest);
        await _context.SaveChangesAsync();
    }

    public async Task<InstructorPayout> GetPayoutRequestById(int payoutRequestId)
    {
        return await _context.InstructorPayouts.FindAsync(payoutRequestId);
    }

    public async Task UpdatePayoutRequest(InstructorPayout payoutRequest)
    {
        _context.Entry(payoutRequest).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<List<InstructorPayout>> GetInstructorPayoutByInstructorId(int instructorId)
    {
        return await _context.InstructorPayouts
            .Where(p => p.InstructorId == instructorId)
            .ToListAsync();
    }
}


