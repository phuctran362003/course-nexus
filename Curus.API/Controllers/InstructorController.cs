using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Request;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Curus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "InstructorPolicy")]

public class InstructorController : ControllerBase
{
    private readonly IInstructorService _instructorService;
    private readonly IReportFeedbackService _reportFeedbackService;
    private readonly ICourseService _courseService;
    private readonly IDiscountService _discountService;

    public InstructorController(IInstructorService instructorService, IReportFeedbackService reportFeedbackService, ICourseService courseService, IDiscountService discountService)
    {
        _instructorService = instructorService;
        _reportFeedbackService = reportFeedbackService;
        _courseService = courseService;
        _discountService = discountService;
    }


    /// <summary>
    /// Retrieves instructor data by ID.
    /// </summary>
    [HttpGet("instructors/{id}")]
    public async Task<ActionResult<ManageInstructorDTO>> GetInstructorDataByID(int id)
    {
        try
        {
            var instructorDto = await _instructorService.GetInstructorDataAsync(id);
            return Ok(instructorDto);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Submits a course for review.
    /// </summary>
    [HttpPost("courses/{id}/submit")]
    public async Task<IActionResult> SubmitCourse(int id)
    {
        try
        {
            var course = await _instructorService.SubmitCourse(id);
            return Ok(course);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Reviews a specific course by ID.
    /// </summary>
    [HttpGet("courses/{id}/review")]
    public async Task<IActionResult> ReviewCourse(int id)
    {
        try
        {
            var course = await _instructorService.ReviewCourse(id);
            return Ok(course);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Changes the status of a specific course by ID.
    /// </summary>
    [HttpPost("courses/{id}/status")]
    public async Task<IActionResult> ChangeStatusCourse(int id, [FromForm] ChangeStatusCourseRequest changeStatusCourseRequest)
    {
        try
        {
            var result = await _instructorService.ChangeStatusCourse(id, changeStatusCourseRequest);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
    
    [Authorize(Policy = "InstructorPolicy")]
    [HttpGet("earningAnalytics")]
    public async Task<IActionResult> EarningAnalytics()
    {
        try
        {
            var result = await _instructorService.EarningAnalytics();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize(Policy = "InstructorPolicy")]
    [HttpGet("viewReview")]
    public async Task<IActionResult> ViewReview(int id,int pageSize = 20, int pageIndex = 1)
    {
        try
        {
            var result = await _instructorService.ViewReview(id,pageSize,pageIndex);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize(Policy = "InstructorPolicy")]
    [HttpPatch("toggleMarkGoodReview/{id}")]
    public async Task<IActionResult> toggleMarkGoodReview(int id)
    {
        try
        {
            var result = await _instructorService.toggleMarkGoodReview(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
    
    [Authorize(Policy = "InstructorPolicy")]
    [HttpPost("reportReviewToAdmin/{id}")]
    public async Task<IActionResult> reportReviewToAdmin(int id,[FromForm] ReportFeedbackRequest reportFeedbackRequest)
    {
        try
        {
            var result = await _reportFeedbackService.reportReviewToAdmin(id,reportFeedbackRequest);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
    /// <summary>
    /// Create discount for course by instructor.
    /// </summary>
    [HttpPost("courses/{id}/discount")]
    public async Task<IActionResult> UserDiscountForCourse(int id, [FromForm] DiscountCourseDTO discountCourseDto)
    {
        try
        {
            var result = await _discountService.UseDiscountForCourse(id, discountCourseDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}