using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository.Repositories;

public class BackUpChapterRepository : IBackUpChapterRepository
{
    private readonly CursusDbContext _context;

    public BackUpChapterRepository(CursusDbContext context)
    {
        _context = context;
    }
    public async Task<bool> CreateBackUpChapter(BackupChapter backupChapter)
    {
        await _context.BackupChapters.AddAsync(backupChapter);
        return await SaveChangeAsync();
    }

    public async Task<BackupChapter> GetBackUpChapterByChapterId(int id)
    {
        return await _context.BackupChapters.FirstOrDefaultAsync(b => b.ChapterId == id);
    }

    public async Task<bool> EditBackUpChapter(BackupChapter backupChapter)
    {
        _context.BackupChapters.Update(backupChapter);
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