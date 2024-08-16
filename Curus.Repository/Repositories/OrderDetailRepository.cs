using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Curus.Repository.Repositories;

public class OrderDetailRepository : IOrderDetailRepository
{
    private readonly CursusDbContext _context;

    public OrderDetailRepository(CursusDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDetail>> GetOrderDetailByCourseId(int id)
    {
        return await _context.OrderDetails.Where(o => o.CourseId == id).ToListAsync();
    }
}