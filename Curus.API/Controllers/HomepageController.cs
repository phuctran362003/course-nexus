using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Curus.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HomepageController : ControllerBase
{

    private readonly ICourseService _courseService;

    public HomepageController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Retrieves the face of cursus.
    /// </summary>
    [HttpGet("user/Homepage")]
    public async Task<IActionResult> GetCourses()
    {
        try
        {
            var Courses = await _courseService.GetTopCoursesAsync();
            var Categories = await _courseService.GetTopCategoriesAsync();
            var Feedbacks = await _courseService.GetTopFeedbacksAsync();

            var response = new
            {
                BestCourses = Courses.Data,
                TrendingCategories = Categories.Data,
                TopFeedbacks = Feedbacks.Data
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves the header of homepage.
    /// </summary>
    [HttpGet("Header")]
    public async Task<IActionResult> GetHeader()
    {
        try
        {
            var header = await _courseService.GetHeaderAsync();
            return Ok(header);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
    
    /// <summary>
    /// Retrieves the footer of homepage.
    /// </summary>
    [HttpGet("Footer")]
    public async Task<IActionResult> GetFooter()
    {
        try
        {
            var footer = await _courseService.GetFooterAsync();
            return Ok(footer);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Edit header for homepage.
    /// </summary>
    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("Update-header")]
    public async Task<IActionResult> UpdateHeader([FromBody] HeaderDTO headerDto)
    {
        try
        {
            var updatedHeader = await _courseService.UpdateHeaderAsync(headerDto);
            return Ok(updatedHeader);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Edit footer for homepage.
    /// </summary>
    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("Update-footer")]
    public async Task<IActionResult> UpdateFooter([FromBody] FooterDTO footerDto)
    {
        try
        {
            var updatedFooter = await _courseService.UpdateFooterAsync(footerDto);
            return Ok(updatedFooter);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves the dashboard of admin.
    /// </summary>
    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("Admin/Dashboard")]
    public async Task<IActionResult> AdminDashboard([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var topPurchased = await _courseService.GetTopPurchasedCoursesAsync(startDate, endDate);
            var topBad = await _courseService.GetTopBadCoursesAsync(startDate, endDate);
            var topPayout = await _courseService.GetTopInstructorPayoutsAsync(startDate, endDate);

            var response = new
            {
                TopPurchasedCourses = topPurchased.Data,
                TopBadCourses = topBad.Data,
                TopInstructorPayouts = topPayout.Data
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
