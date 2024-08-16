using System.Data;
using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;

namespace Curus.Service.Interfaces;

public interface ICourseService
{
    Task<UserResponse<object>> CreateCourse(CourseDTO courseDto);

    Task<List<Chapter>> GetChaptersByCourseId(int id);

    Task<UserResponse<object>> EditDraft(int id, CourseEditDTO courseDto);
    Task<UserResponse<object>> ReviewRejectCourse();
    Task<UserResponse<object>> GetListBookmarkedCourse(int PAGE_SIZE = 20, int page = 1);
    
    Task<UserResponse<object>> UpdateStatusCourse(ChangeStatusCourseRequest changeStatusCourseRequest, int id);

    Task<UserResponse<object>> GetCoursesAsync(int instructorId, int pageSize, int pageIndex, string sortBy, bool sortDesc);

    Task<UserResponse<object>> GetCoursesByInstructorAsync(int pageSize, int pageIndex, string sortBy, bool sortDesc, CourseStatus? status);

    Task<UserResponse<object>> ViewSubmitCourseByAdmin();
    
    Task<byte[]> ExportCourseToExcelAsync();
    //get list all course
    Task<List<ManageCourseDTO>> GetInfoCourse(int pageSize, int pageIndex, string sortBy, bool sortDesc); // Sửa lại phần paging
    
    Task<UserResponse<object>> CreateCommentToCourse(CommentDTO commentDTO, int id);

    Task<UserResponse<object>> ManageCourseDetail(int id);

    //Student 
    Task<UserResponse<object>> StudentCreateCommentCourse(CommentDTO commentDTO, int id);
    Task<UserResponse<object>> ReportCourseById(ReportCourseDTO reportCourseDto, int id);
    
    Task<UserResponse<object>> ReportChapterById(ReportCourseDTO reportCourseDto, int id);

    
    Task<UserResponse<object>> ApproveCourseByAdmin(int id);
    Task<UserResponse<object>> RejectCourseByAdmin(int id, RejectCourseRequest rejectCourseRequest);

    Task<UserResponse<object>> GetAllActiveCoursesAsync(int pageSize, int pageIndex);
    Task<UserResponse<object>> SearchCoursesAsync(Search? searchBy, string search, int pageSize, int pageIndex);

    Task<UserResponse<object>> GetTopCoursesAsync();
    Task<UserResponse<object>> GetTopCategoriesAsync();
    Task<UserResponse<object>> GetTopFeedbacksAsync();

    Task<HeaderDTO> GetHeaderAsync();
    Task<FooterDTO> GetFooterAsync();
    Task<UserResponse<HeaderDTO>> UpdateHeaderAsync(HeaderDTO headerDto);
    Task<UserResponse<FooterDTO>> UpdateFooterAsync(FooterDTO footerDto);
    Task<UserResponse<object>> GetListEnrolledCourse(int PAGE_SIZE = 20, int page = 1);

    Task<UserResponse<object>> ReviewCourseById(FeedbackCourseDTO feedbackCourseDto, int id);
    
    // Discount Course

    Task<List<ViewCategoryNameDTO>> ViewCategoryName(int id);

    Task<List<CommentUserDetail>> ViewCommentDetail(int id);

    Task<List<CommentUserDetail>> ViewStudentCommentDetail(int id);

    

    Task<UserResponse<object>> GetTopPurchasedCoursesAsync(DateTime? startDate, DateTime? endDate);
    Task<UserResponse<object>> GetTopBadCoursesAsync(DateTime? startDate, DateTime? endDate);
    Task<UserResponse<object>> GetTopInstructorPayoutsAsync(DateTime? startDate, DateTime? endDate);
}