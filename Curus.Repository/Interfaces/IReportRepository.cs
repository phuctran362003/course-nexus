using Curus.Repository.Entities;

namespace Curus.Repository.Interfaces;

public interface IReportRepository
{
    Task<Report> GetReportByUserId(int id, int courseId);

    Task<Report> GetReportByChapterId(int id, int chapterId);
}