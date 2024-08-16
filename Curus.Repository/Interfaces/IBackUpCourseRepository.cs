using Curus.Repository.Entities;

namespace Curus.Repository.Interfaces;

public interface IBackUpCourseRepository
{
    Task<bool> CreateBackUpCourse(BackupCourse backupCourse);
    Task<BackupCourse> GetBackUpCourseByCourseId(int? id);
    Task<bool> EditBackUpCourse(BackupCourse backupCourse);
}