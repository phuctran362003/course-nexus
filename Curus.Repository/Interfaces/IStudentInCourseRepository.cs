using Curus.Repository.Entities;

namespace Curus.Repository.Interfaces;

public interface IStudentInCourseRepository
{
    Task<List<StudentInCourse>> GetCourseByInstructorId(int id);
    Task<List<StudentInCourse>> GetStudentDashboardCourse(int id,DateTime filter);
    Task<List<StudentInCourse>> GetCourseByStudentById(int id);
    Task<StudentInCourse> GetCourseByUser(int id, int userId);
    Task FinishCourse(int courseId, int userId);
}