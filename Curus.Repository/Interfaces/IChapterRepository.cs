using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Response;

namespace Curus.Repository.Interfaces;

public interface IChapterRepository
{
    Task<bool> CreateChapter(Chapter chapter);

    Task<bool> SavChangeAsync();

    Task<Chapter> GetChapterById(int id);

    Task<bool> UpdateChapter(Chapter chapter);

    Task<bool> DeleteChapterById(Chapter chapter);

    Task<List<Chapter>> StartChapterById(int id);
}