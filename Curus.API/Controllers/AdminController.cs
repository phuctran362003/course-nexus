using ClosedXML.Excel;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using Curus.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Curus.Repository.ViewModels.Request;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

[ApiController]
[Route("api/admin/manage")]
[Authorize(Policy = "AdminPolicy")]
public class
    AdminController : ControllerBase
{
    private readonly IInstructorService _instructorService;
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly ICourseService _courseService;
    private readonly IReportFeedbackService _reportFeedbackService;
    private readonly IDiscountService _discountService;

    public AdminController(IInstructorService instructorService, IUserService userService, IUserRepository userRepository, ICourseService courseService, IDiscountService discountService, IReportFeedbackService reportFeedbackService)
    {
        _instructorService = instructorService;
        _userService = userService;
        _userRepository = userRepository;
        _courseService = courseService;
        _discountService = discountService;
        _reportFeedbackService = reportFeedbackService;
    }

    /// <summary>
    /// Gets details of a specific instructor by ID.
    /// </summary>
    [HttpGet("instructors/{id}")]
    public async Task<IActionResult> GetInstructorDetails(int id)
    {
        var instructorDetails = await _instructorService.GetInstructorDataAsync(id);
        return Ok(instructorDetails);
    }

    /// <summary>
    /// Gets all instructor data with pagination.
    /// </summary>
    [HttpGet("instructors")]
    public async Task<IActionResult> GetAllInstructorData(int pageIndex = 1, int pageSize = 20)
    {
        try
        {
            var instructorDto = await _instructorService.GetAllInstructorDataAsync(pageIndex,pageSize);
            return Ok(instructorDto);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Views comments for a specific instructor by ID.
    /// </summary>
    [HttpGet("instructors/{id}/comments")]
    public async Task<ActionResult<ViewCommentDTO>> ViewCommentById(int id)
    {
        try
        {
            var comment = await _instructorService.ViewCommentById(id);
            if (comment.Count == 0)
            {
                return Ok(new { Message = "This user does not have comments by admin or does not exist." });
            }

            return Ok(comment);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Exports instructor data to an Excel file.
    /// </summary>
    [HttpGet("instructors/export")]
    public async Task<IActionResult> ExportToExcel(int pageIndex = 1, int pageSize = 20)
    {
        var fileContents = await _instructorService.ExportToExcelAsync(pageIndex,pageSize);
        var exportDate = DateTime.Now.ToString("yyyyMMdd");
        var fileName = $"ManageInstructor_{exportDate}.xlsx";
        var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        if (fileContents.Data == null)
        {
            return Ok(fileContents);
        }
        return File(fileContents.Data, mimeType, fileName);
    }

    /// <summary>
    /// Creates a comment for a specific instructor by ID.
    /// </summary>
    [HttpPost("instructors/{id}/comments")]
    public async Task<IActionResult> CreateCommentToInstructor([FromForm] CommentDTO commentDto, int id)
    {
        try
        {
            var (result, message) = await _instructorService.CreateCommentToInstructor(commentDto, id);
            if (!result)
            {
                return BadRequest(new { Message = message });
            }

            return Ok(new { Message = message });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while adding the comment", Details = ex.Message });
        }
    }


    /// <summary>
    /// Edits a comment for a specific instructor by comment ID.
    /// </summary>
    [HttpPut("instructors/comments/{commentId}")]
    public async Task<IActionResult> EditCommentFromUser([FromForm] CommentDTO commentDto,
        int commentId)
    {
        try
        {
            var result = await _instructorService.EditCommentByCommentId(commentDto, commentId);
            if (!result)
            {
                return Ok("Cannot find this comment");
            }

            return Ok("Update successful");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a comment for a specific instructor by comment ID.
    /// </summary>
    [HttpDelete("instructors/{instructorId}/comments/{commentId}")]
    public async Task<IActionResult> DeleteCommentFromUser(int instructorId, int commentId)
    {
        try
        {
            var result = await _instructorService.DeleteCommentByCommentId(commentId);
            if (!result)
            {
                return NotFound("Cannot find this comment");
            }

            return Ok("Delete comment successful");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Gets all pending instructors.
    /// </summary>
    [HttpGet("instructors/pending")]
    public async Task<IActionResult> GetPendingInstructors()
    {
        var pendingInstructors = await _instructorService.GetPendingInstructorsAsync();
        return Ok(pendingInstructors);
    }

    /// <summary>
    /// Changes the status of a specific instructor.
    /// </summary>
    [HttpPut("instructors/{id}/status")]
    public async Task<IActionResult> ChangeStatusInstructor([FromForm] ContentEmailDTO contentEmailDto, int id)
    {
        try
        {
            var message = await _instructorService.ChangeStatusInstructor(contentEmailDto, id);
            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating the status", details = ex.Message });
        }
    }

    /// <summary>
    /// Approves a specific instructor.
    /// </summary>
    [HttpPost("instructors/{id}/approve")]
    public async Task<IActionResult> ApproveInstructor(int id)
    {
        var response = await _instructorService.ApproveInstructorAsync(id);
        if (response.Data == null)
        {
            return NotFound(new { response.Message });
        }

        return Ok(new { response.Message });
    }

    /// <summary>
    /// Rejects a specific instructor.
    /// </summary>
    [HttpPost("instructors/reject")]
    public async Task<IActionResult> RejectInstructor([FromForm] ApproveRejectInstructorDTO rejectDto)
    {
        var response = await _instructorService.RejectInstructorAsync(rejectDto);
        if (response.Data == null)
        {
            return NotFound(new { response.Message });
        }

        return Ok(new { response.Message });
    }


    // Student Endpoints

    /// <summary>
    /// Gets details of a specific student by ID.
    /// </summary>
    [HttpGet("students/{id}")]
    public async Task<IActionResult> ManageStudentDetail(int id)
    {
        try
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet]
    [Route("get-info-student")]
    public async Task<IActionResult> GetInfoStudent(int PAGE_SIZE = 20, int page = 1)
    {
        try
        {
            var result = await _userService.GetInfoStudent(PAGE_SIZE, page);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Changes the status of a specific student by ID.
    /// </summary>
    [HttpPatch("students/{id}/status")]
    public async Task<IActionResult> ChangeStatusStudent([FromForm] ContentEmailDTO contentEmailDto, int id)
    {
        try
        {
            var message = await _userService.UpdateStatusUser(contentEmailDto, id);
            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating the status", details = ex.Message });
        }
    }


    [HttpGet]
    [Route("export-student-list")]
    public async Task<IActionResult> ExportStudentList()
    {
        var listStudent = await _userRepository.GetDataStudent();
        using (XLWorkbook wb = new XLWorkbook())
        {
            wb.AddWorksheet(listStudent, "Student List");
            using (MemoryStream ms = new MemoryStream())
            {
                wb.SaveAs(ms);
                var currentDate = DateTime.Now.ToString("yyyyMMdd");
                var fileName = $"StudentList_{currentDate}.xlsx";
                return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }
    }
   /// <summary>
    /// Gets paginated information of course.
    /// </summary>
    [HttpGet("course")]
    public async Task<IActionResult> GetListAllCourse(string sortBy, bool sortDesc, int pageSize = 20,
        int pageIndex = 1)
    {
        try
        {
            var result = await _courseService.GetInfoCourse(pageSize, pageIndex, sortBy, sortDesc);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Changes the status of a specific course by ID.
    /// </summary>
    [HttpPatch("course/{id}/status")]
    public async Task<IActionResult> ChangeStatusCourse([FromForm] ChangeStatusCourseRequest changeStatusCourseRequest,
        int id)
    {
        try
        {
            var message = await _courseService.UpdateStatusCourse(changeStatusCourseRequest, id);
            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating the status", details = ex.Message });
        }
    }

    /// <summary>
    /// Exports course data to an Excel file.
    /// </summary>
    [HttpGet("course/export")]
    public async Task<IActionResult> ExportCoursesToExcel()
    {
        var fileContents = await _courseService.ExportCourseToExcelAsync();
        var exportDate = DateTime.Now.ToString("yyyyMMdd");
        var fileName = $"ManageCourse_{exportDate}.xlsx";
        var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return File(fileContents, mimeType, fileName);
    }

    /// <summary>
    /// Creates a comment for a specific instructor by ID.
    /// </summary>
    [HttpPost("course/{id}/comments")]
    public async Task<IActionResult> CreateCommentToCourse([FromForm] CommentDTO commentDto, int id)
    {
        try
        {
            var check = await _courseService.CreateCommentToCourse(commentDto, id);
            return Ok(check);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while adding the comment", Details = ex.Message });
        }
    }

    /// <summary>
    /// Gets paginated information of course.
    /// </summary>
    [HttpGet("course/{id}/detail")]
    public async Task<IActionResult> ManageCourseDetail(int id)
    {
        try
        {
            var result = await _courseService.ManageCourseDetail(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Creates a comment for a specific instructor by ID.
    /// </summary>
    [HttpPost("discount/create")]
    public async Task<IActionResult> CreateDiscountForInstructor([FromForm] CreateDiscountDTO createDiscountDto)
    {
        try
        {
            var discount = await _discountService.CreateDiscount(createDiscountDto);
            return Ok(discount);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while adding the comment", Details = ex.Message });
        }
    }
    
    /// <summary>
    /// Send discount to all instructor.
    /// </summary>
    [HttpPost("discount/{id}/send")]
    public async Task<IActionResult> SenDiscountToInstructor([FromForm] SendDiscountDTO sendDiscountDto,int id)
    {
        try
        {
            var discount = await _discountService.SendDiscount(sendDiscountDto,id);
            return Ok(discount);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while adding the comment", Details = ex.Message });
        }
    }

    [HttpPatch("admin/hide/{id}")]
    public async Task<IActionResult> HideReportFeedback(int id)
    {
        try
        {
            var result = await _reportFeedbackService.acceptReportFeedback(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPatch("admin/unhide/{id}")]
    public async Task<IActionResult> UnhideReportFeedback(int id)
    {
        try
        {
            var result = await _reportFeedbackService.rejectReportFeedback(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}

