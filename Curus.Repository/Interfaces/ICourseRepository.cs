using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;

namespace Curus.Repository.Interfaces;

public interface ICourseRepository
{
    Task<bool> HasCoursesInCategory(int categoryId);

    Task<bool> CreateCourse(Course course);

    Task<List<Chapter>> GetChaptersByCourseId(int id);

    Task<BackupCourse> GetCourseByName(string name);
    
    Task<Course> GetCourseById(int? id);

    Task<List<Course>> GetAllCourseAsync();

    Task<List<CommentCourse>> GetCourseCommentById(int id);
    Task<bool> EditCourse(Course course);

    Task<List<StudentInCourse>> GetStudentInCourse(int id);

    Task<List<Course>> GetCourseByInstructorId(int id);
    Task<List<BookmarkedCourse>> ViewListBookmarkedCourse(int userId);

    Task<User> GetInstructorByIdAsync(int userId);

    Task<Category> GetCategoryByIdAsync(int id);

    Task<List<Course>> GetCourseByStatus();

    Task<List<StudentInCourse>> ListEnrolledCourse(int userId);

    Task<bool> CreateCourseComment(CommentCourse? commentCourse);

    Task<IEnumerable<Course>> GetAllCoursesAsync(string sortBy, bool sortDesc);

    Task<int> CountStudentInCourses(int id);

    Task<bool> CreateReportCourse(Report report);

    Task<List<Course>> GetPendingCoursesAsync(int pageSize, int pageIndex, string sortBy, bool sortDesc);

    Task<List<Course>> GetCoursesAsync(int instructorId, int pageSize, int pageIndex, string sortBy, bool sortDesc);

    Task<List<Course>> GetCoursesByInstructorAsync(int instructorId, int pageSize, int pageIndex, string sortBy, bool sortDesc, CourseStatus? status = null);

    Task<List<Course>> GetActiveCoursesAsync(int pageSize, int pageIndex);
    Task<List<int>> GetCategoryIdsByNameAsync(string categoryName);
    Task<List<Course>> GetCoursesByCategoryIdsAsync(List<int> categoryIds, int pageSize, int pageIndex);
    Task<List<Course>> GetCoursesByInstructorNameAsync(string instructorName, int pageSize, int pageIndex);

    Task<List<Course>> GetTopCoursesAsync(int count);
    Task<List<CategoryResponse>> GetTopCategoriesAsync(int count);
    Task<List<Feedback>> GetTopFeedbacksAsync(int count);

    Task<Header> GetHeaderAsync();
    Task<Footer> GetFooterAsync();
    Task<Header> UpdateHeaderAsync(Header header);
    Task<Footer> UpdateFooterAsync(Footer footer);

    Task<List<Course>> GetCourseSuggest();
    

    Task<bool> CreateFeedbackCourse(Feedback feedback);

    Task<StudentInCourse> GetStudentInCourseById(int id, int courseId);

    Task<bool> UpdateStudentInCourse(StudentInCourse studentInCourse);

    Task<List<Feedback>> GetAllFeedback();

    Task<double> GetStudentInCourseByCourseId(int id);
    Task<int> CountStudentsInCourseAsync(int courseId);

    Task<List<int>> GetTopPurchasedCoursesAsync(int count);
    Task<List<Course>> GetTopBadCoursesAsync(int count);
    Task<List<InstructorPayoutDTO>> GetTopInstructorPayoutsAsync(int count);

    Task<List<Course>> GetCoursesByInstructorIdAsync(int instructorId);

}