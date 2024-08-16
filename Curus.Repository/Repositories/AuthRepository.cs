using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository;

public class AuthRepository : IAuthRepository
{

    private readonly CursusDbContext _context;

    public AuthRepository(CursusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateUser(User user)
    {
        var roleUser = await _context.Roles.FirstOrDefaultAsync(c => c.Id == 2);
        user.Role = roleUser;
        await _context.AddAsync(user);
        return await SaveChange();
    }

    public async Task<bool> UpdateRefreshToken(int userId, string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        user.RefreshToken = refreshToken;
        return await SaveChange();
    }

    public async Task<User> GetRefreshToken(string refreshToken)
    {
        return await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(r => r.RefreshToken == refreshToken);
    }

    public async Task<bool> DeleteRefreshToken(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        user.RefreshToken = "";
        return await SaveChange();
    }

    public async Task<bool> SaveChange()
    {
        var save = await _context.SaveChangesAsync();
        return save > 0 && true;
    }

}