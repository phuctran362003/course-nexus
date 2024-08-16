using Curus.Repository.ViewModels;
using HistoryCourse = Curus.Repository.Entities.HistoryCourse;

namespace Curus.Repository.Interfaces;

public interface IHistoryCourseRepository
{
    Task<List<HistoryCourse>> GetAllHistoryOfCourseByCourseid(int id);
    Task<bool> CreateHistoryCourse(HistoryCourse historyCourse);


}