using Curus.Repository.Entities;

namespace Curus.Repository.Interfaces;

public interface IBackUpChapterRepository
{
    Task<bool> CreateBackUpChapter(BackupChapter backupChapter);
    Task<BackupChapter> GetBackUpChapterByChapterId(int id);
    Task<bool> EditBackUpChapter(BackupChapter backupChapter);
}