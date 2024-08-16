using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using System.Threading.Tasks;
using Curus.Repository.ViewModels.Response;

namespace Curus.Repository.Interfaces;

public interface IInstructorRepository
{
    //INSTRUCTOR
    Task AddAsync(InstructorData instructorData);
    Task AddInstructorAsync(InstructorData instructorData);

    Task<InstructorData> GetInstructorByIdAsync(int userId);
    Task<User> GetInstructorROLEByIdAsync(int instructorId);
    Task<List<User>> GetAllInstructorAsync();
    Task<List<User>> GetPendingInstructorsAsync();
    Task<List<Course>> GetAllCourseOfInstructor(int id);

    Task UpdateAsync(InstructorData instructorData);

    Task<bool> UpdateCourseAsync(Course course);

    //USER
    Task<User?> GetUserByIdAsync(int id);
    Task UpdateUserAsync(User user);

    //COURSE
    Task<List<StudentInCourse>> GetStudentInCourse(int id);
    


    //COMMENT
    Task CreateCommentUserAsync(CommentUser? commentUser);
    Task<List<CommentUser?>> GetCommentById(int id);
    Task<CommentUser?> GetCommentByCommentId(int id);
    Task EditCommentByCommentId(CommentUser commentUser);
    Task DeleteCommentByCommentId(CommentUser commentUser);
}