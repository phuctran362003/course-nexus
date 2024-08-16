using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly CursusDbContext _context;

    public DiscountRepository(CursusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateDiscount(Discount? discount)
    {
        await _context.Discounts.AddAsync(discount);
        return await SaveChangeAsync();
    }

    public async Task<Discount?> GetDiscountById(int id)
    {
        return await _context.Discounts.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<bool> UpdateDiscount(Discount? discount)
    {
         _context.Discounts.Update(discount);
         return await SaveChangeAsync();
    }

    public async Task<Discount?> GetDiscountByCode(string code)
    {
        return await _context.Discounts.FirstOrDefaultAsync(c => c.DiscountCode == code);
    }

    public async Task<bool> CreateHistoryDiscount(HistoryCourseDiscount historyCourseDiscount)
    {
        await _context.HistoryCourseDiscounts.AddAsync(historyCourseDiscount);
        return await SaveChangeAsync();

    }

    public async Task<HistoryCourseDiscount> GetHistoryCourseDiscount(int id, int instructorId)
    {
        return await _context.HistoryCourseDiscounts.FirstOrDefaultAsync(i => i.Id == id && i.InstructorId == instructorId);
    }
    
    public async Task<List<int?>> GetListCourseByDiscountId(int id)
    {
        return await _context.HistoryCourseDiscounts.Where(c => c.DiscountId == id)
            .Select(c => c.CourseId).ToListAsync();
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