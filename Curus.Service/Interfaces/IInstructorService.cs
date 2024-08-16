using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Response;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Curus.Repository.ViewModels.Request;

namespace Curus.Service.Interfaces;

public interface IInstructorService
{
    


    //------- SPRINT 2 -------
    //APPROVE - REJECT INSTRUCTOR
    Task<UserResponse<object>> ApproveInstructorAsync(int instructorId);
    Task<UserResponse<object>> RejectInstructorAsync(ApproveRejectInstructorDTO rejectDto);
    //GET PENDING INSTRUCTOR
    Task<UserResponse<object>> GetPendingInstructorsAsync();

    //GET one INSTRUCTOR 
    Task<UserResponse<object>> GetInstructorDataAsync(int userId);
    //GET all INSTRUCTOR

    Task<UserResponse<object>> GetAllInstructorDataAsync(int pageIndex,int pageSize);
    
    //Submit course
    Task<UserResponse<object>> SubmitCourse(int id);

    //Course review
    Task<UserResponse<object>> ReviewCourse(int id);



    Task<UserResponse<object>> ChangeStatusInstructor(ContentEmailDTO contentEmailDto, int id);


    //CRUD COMMENT
    Task<(bool, string)> CreateCommentToInstructor(CommentDTO commentDTO, int id);
    Task<List<ViewCommentDTO>> ViewCommentById(int id);
    Task<bool> EditCommentByCommentId(CommentDTO commentDto, int id);
    Task<bool> DeleteCommentByCommentId(int id);
    Task<UserResponse<byte[]>> ExportToExcelAsync(int pageIndex, int pageSize);


    Task<UserResponse<object>> ChangeStatusCourse(int id, ChangeStatusCourseRequest changeStatusCourseRequest);


    Task<UserResponse<object>> EarningAnalytics();

    Task<UserResponse<object>> ViewReview(int id,int pageSize, int pageIndex);

    Task<UserResponse<object>> toggleMarkGoodReview(int id);

    
}


