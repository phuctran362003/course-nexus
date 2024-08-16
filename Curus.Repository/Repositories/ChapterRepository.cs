using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository.Repositories;

public class ChapterRepository : IChapterRepository
{
    private readonly CursusDbContext _context;

    public ChapterRepository(CursusDbContext context)
    {
        _context = context;
    }
        
    public async Task<bool> CreateChapter(Chapter chapter)
    {
        await _context.Chapters.AddAsync(chapter);
        return await SavChangeAsync();
    }

    public async Task<bool> SavChangeAsync()
    {
        var check = await _context.SaveChangesAsync();
        if (check != 0)
        {
            return true;
        }
        return false;
    }

    public async Task<Chapter> GetChapterById(int id)
    {
        return await _context.Chapters.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<bool> UpdateChapter(Chapter chapter)
    {
        _context.Chapters.Update(chapter);
        return await SavChangeAsync();
    }

    public async Task<bool> DeleteChapterById(Chapter chapter)
    {
        _context.Chapters.Remove(chapter);
        return await SavChangeAsync();
    }

    public async Task<List<Chapter>> StartChapterById(int id)
    {
        var listChapter = await _context.Chapters.Where(c => c.CourseId == id).ToListAsync();
        return listChapter;
    }
}