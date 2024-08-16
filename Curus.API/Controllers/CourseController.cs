using Azure;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Request;
using Curus.Service.Interfaces;
using Curus.Service.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Requests.BatchRequest;
using Curus.Repository.Entities;

namespace Curus.API.Controllers;
[ApiController]
[Route("api/course")]
public class CourseController : ControllerBase
{

    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Creates a new course.
    /// </summary>
    [Authorize(Policy = "InstructorPolicy")]
    [HttpPost("courses")]
    public async Task<IActionResult> CreateCourse([FromForm] CourseDTO courseDto)
    {
        var course = await _courseService.CreateCourse(courseDto);
        return Ok(course);
    }


    /// <summary>
    /// Retrieves chapters by course ID.
    /// </summary>
    [HttpGet("courses/{courseId}/chapters")]
    public async Task<IActionResult> GetChapterByCourseId(int courseId)
    {
        var course = await _courseService.GetChaptersByCourseId(courseId);
        return Ok(course);
    }

    /// <summary>
    /// Edits a draft course by ID.
    /// </summary>
    [Authorize(Policy = "InstructorPolicy")]
    [HttpPost("courses/{id}/draft")]
    public async Task<IActionResult> EditDraft([FromForm] CourseEditDTO courseDto, int id)
    {
        try
        {
            var course = await _courseService.EditDraft(id, courseDto);
            return Ok(course);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Reviews rejected courses.
    /// </summary>
    [HttpGet("courses/rejected")]
    public async Task<IActionResult> ReviewRejectCourse()
    {
        try
        {
            var course = await _courseService.ReviewRejectCourse();
            return Ok(course);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves submitted courses for admin review.
    /// </summary>
    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("admin/submitted-courses")]
    public async Task<IActionResult> ViewSubmitCourseByAdmin()
    {
        try
        {
            var result = await _courseService.ViewSubmitCourseByAdmin();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves active courses by instructor ID for admin.
    /// </summary>
    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("admin/instructors/{instructorId}/active-courses")]
    public async Task<IActionResult> GetActiveCoursesForAdmin(int instructorId, int pageSize = 20, int pageIndex = 1, string sortBy = "name", bool sortDesc = true)
    {
        try
        {
            var courses = await _courseService.GetCoursesAsync(instructorId, pageSize, pageIndex, sortBy, sortDesc);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }


    /// <summary>
    /// Retrieves courses by instructor with pagination and sorting.
    /// </summary>
    [HttpGet("instructors/courses")]
    public async Task<IActionResult> GetCoursesByInstructor(int pageSize = 20, int pageIndex = 1, string sortBy = "name", bool sortDesc = true, CourseStatus? status = null)
    {
        try
        {
            var courses = await _courseService.GetCoursesByInstructorAsync(pageSize, pageIndex, sortBy, sortDesc, status);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all active courses for user.
    /// </summary>
    [HttpGet("courses/view")]
    public async Task<IActionResult> GetAllActiveCourses(int pageSize = 20, int pageIndex = 1)
    {
        try
        {
            var courses = await _courseService.GetAllActiveCoursesAsync(pageSize, pageIndex);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves active courses for user search action.
    /// </summary>
    [HttpGet("courses/search")]
    public async Task<IActionResult> SearchCourses(Search? searchBy = null, string search = null, int pageSize = 20, int pageIndex = 1)
    {
        try
        {
            var courses = await _courseService.SearchCoursesAsync(searchBy, search, pageSize, pageIndex);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("Admin/Course/Approve/{id}")]
    public async Task<IActionResult> ApproveCourseByAdmin(int id)
    {
        try
        {
            var result = await _courseService.ApproveCourseByAdmin(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("Admin/Course/Reject/{id}")]
    public async Task<IActionResult> RejectCourseByAdmin(int id,[FromForm] RejectCourseRequest rejectCourseRequest)
    {
        try
        {
            var result = await _courseService.RejectCourseByAdmin(id, rejectCourseRequest);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

}

