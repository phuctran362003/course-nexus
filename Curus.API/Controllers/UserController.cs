using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Request;
using Curus.Service.Interfaces;
using Curus.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Curus.API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize(Policy = "UserPolicy")]
public class UserController : ControllerBase
{
    private readonly IInstructorService _instructorService;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IBlobService _blobService;
    private readonly ICourseService _courseService;
    private readonly IChapterService _chapterService;

    private readonly IEmailService _emailService;
    private readonly ILogger<UserController> _logger;

    public UserController(IInstructorService instructorService, IAuthService authService, IUserService userService,
        IBlobService blobService, IEmailService emailService, ILogger<UserController> logger,
        ICourseService courseService, IChapterService chapterService)
    {
        _instructorService = instructorService;
        _authService = authService;
        _userService = userService;
        _blobService = blobService;
        _emailService = emailService;
        _logger = logger;
        _courseService = courseService;
        _chapterService = chapterService;
    }

    [HttpGet("card-providers")]
    public IActionResult GetCardProviders()
    {
        var providers = _authService.GetCardProviders();
        return Ok(providers);
    }


    [HttpPost("course/comment")]
    public async Task<IActionResult> CreateCourseComment([FromForm] CommentDTO commentDTO, int id)
    {
        try
        {
            var providers = await _courseService.StudentCreateCommentCourse(commentDTO, id);
            return Ok(providers);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }


    [HttpGet("view-my-bookmarked-course")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> ViewListBookmarkedCourse(int PAGE_SIZE = 20, int page = 1)
    {
        var result = await _courseService.GetListBookmarkedCourse(PAGE_SIZE, page);
        return Ok(result);
    }

    [HttpPost("toggle-bookmark-course")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> ToggleBookmarkCourse(int courseId)
    {
        var result = await _userService.ToggleBookmarkCourse(courseId);
        return Ok(result);
    }


    [HttpPost("course/{id}/report")]
    public async Task<IActionResult> ReportCourseByStudent([FromForm] ReportCourseDTO reportCourseDto, int id)
    {
        try
        {
            var report = await _courseService.ReportCourseById(reportCourseDto, id);
            return Ok(report);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    [HttpPost("chapter/{id}/report")]
    public async Task<IActionResult> ReportChapterByStudent([FromForm] ReportCourseDTO reportCourseDto, int id)
    {
        try
        {
            var report = await _courseService.ReportChapterById(reportCourseDto, id);
            return Ok(report);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    [HttpPost("course/{id}/review")]
    public async Task<IActionResult> ReviewCourseByStudent([FromForm] FeedbackCourseDTO feedbackCourseDto, int id)
    {
        try
        {
            var feedback = await _courseService.ReviewCourseById(feedbackCourseDto, id);
            return Ok(feedback);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }


    [HttpGet("view-my-enrolled-course")]
    public async Task<IActionResult> ViewListEnrolledCourse(int PAGE_SIZE = 20, int page = 1)
    {
        var result = await _courseService.GetListEnrolledCourse(PAGE_SIZE, page);
        return Ok(result);
    }

    [HttpGet("StudentCourseDashboard")]
    public async Task<IActionResult> StudentCourseDashboard(
        [FromQuery] StudentDashboard studentDashboard = StudentDashboard.OneMonth)
    {
        try
        {
            var result = await _userService.GetStudentDashboard(studentDashboard);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("chapter/{id}/start")]
    public async Task<IActionResult> StartChapterById(int id)
    {
        try
        {
            var start = await _chapterService.StartChapterById(id);
            return Ok(start);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    [HttpPut("update-information")]
    [Authorize]
    public async Task<IActionResult> UpdateInformation([FromForm] UpdateUserDTO updateUser)
    {
        var result = await _userService.UpdateUser(updateUser);
        return Ok(result);
    }

    [Authorize(Policy = "UserPolicy")]
    [HttpPost("finish-course/{id}")]
    public async Task<IActionResult> FinishCourse([FromRoute] int id)
    {
        var result = await _userService.FinishCourse(id);
        return Ok(result);
    }
}