using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using Curus.Repository.ViewModels.User.Student;
using Curus.Service.ResponseDTO;

namespace Curus.Service.Interfaces;

public interface IUserService
{
    Task<User> GetByEmail(string email);
    Task UpdateUserAsync(User user);

    Task<UserResponse<object>> GetUserById(int id);
    //admin
    Task<(bool, string)> CreateCommentByAdmin(CommentDTO commentDto, int id);

    //get list student
    Task<IQueryable<StudentInfoDto>> GetInfoStudent(int PAGE_SIZE = 10, int page = 1);
    Task<string> UpdateStatusUser(ContentEmailDTO contentEmailDTO, int userId);
    //deactivate/activate a student.
    Task<GeneralServiceResponseDto> ActivateStudent(int userId);

    //bookmarked course
    Task<UserResponse<object>> ToggleBookmarkCourse(int courseId);

    Task<UserResponse<object>> GetStudentDashboard(StudentDashboard studentDashboard);
    Task SendRemindersAsync();
    Task DeleteInactiveUsersAsync();
    //update user
    Task<UserResponse<object>> UpdateUser(UpdateUserDTO updateUserDto);

    //finist course
    Task<UserResponse<object>> FinishCourse(int courseId);

}