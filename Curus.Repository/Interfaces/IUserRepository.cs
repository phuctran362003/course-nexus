using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using System.Data;
using System.Linq.Expressions;

namespace Curus.Repository.Interfaces;

public interface IUserRepository
{


    //SPRINT 1

    Task<User?> FindByEmailOrPhoneAsync(string email, string phoneNumber);
    Task UpdateAsync(User user);
    Task AddAsync(User user);
    Task<User> GetUserByEmail(string email);

    Task<User> GetUserByVerificationToken(string token);
    Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate);
    
    
    Task<User> GetUserById(int id);
    Task<User> CheckPhoneNumber(string phone);

    Task<User> GetAllUserById(int id);



    //Instructor
    Task AddInstructorDataAsync(InstructorData instructorData);

    //Admin
    Task CreateCommentUserAsync(CommentUser? commentUser);

    Task<List<User>> GetUsersByRole(int role);

    //Student
    Task<List<User>> GetInfoStudent(int PAGE_SIZE = 20, int page = 1);
    Task<int> CountAmountStudentCourse(int userId);
    Task<List<StudentInCourse>> GetListStudentInCourse();
    Task ActiveStudent(User user);
    Task<User> GetStudentById(int userId);
    Task<DataTable> GetDataStudent();


    //forgot password
    Task<User> GetUserByEmailOrPhoneNumber(string emailOrPhoneNumber);
    Task<User> GetUserByResetToken(string resetToken);

    //bookmarked course
    Task AddBookmarkedCourse(BookmarkedCourse bookmarkedCourse);
    Task UnBookmarkedCourse(int courseId, int userId);
    Task<object> GetBookmarkedCourse(int userId, int courseId);
    
    //Auto clear
    Task UpdateLastActivityDateAsync(string email);
    Task<List<User>> GetInactiveStudentsAsync(DateTime time);
    Task<List<User>> GetInactiveInstructorsAsync(DateTime since);
    Task RemoveUsersAsync(IEnumerable<User> users);
    
    
}
