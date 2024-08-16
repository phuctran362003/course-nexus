using System.IdentityModel.Tokens.Jwt;
using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using OfficeOpenXml;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using DocumentFormat.OpenXml.Bibliography;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Curus.Service.Services;

public class InstructorService : IInstructorService
{
    private readonly IUserRepository _userRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IEmailService _emailService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICourseRepository _courseRepository;
    private readonly IStudentInCourseRepository _studentInCourseRepository;
    private readonly IInstructorPayoutRepository _instructorPayoutRepository;
    private readonly IHistoryCourseRepository _historyCourseRepository;
    private readonly IFeedBackRepository _feedBackRepository;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly ILogger<InstructorService> _logger;


    public InstructorService(IUserRepository userRepository, IInstructorRepository instructorRepository,
        IEmailService emailService, IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository,
        IStudentInCourseRepository studentInCourseRepository, IInstructorPayoutRepository instructorPayoutRepository,
        IHistoryCourseRepository historyCourseRepository, IFeedBackRepository feedBackRepository,
        ILogger<InstructorService> logger, IOrderDetailRepository orderDetailRepository)
    {
        _userRepository = userRepository;
        _instructorRepository = instructorRepository;
        _emailService = emailService;
        _httpContextAccessor = httpContextAccessor;
        _courseRepository = courseRepository;
        _studentInCourseRepository = studentInCourseRepository;
        _instructorPayoutRepository = instructorPayoutRepository;
        _historyCourseRepository = historyCourseRepository;
        _feedBackRepository = feedBackRepository;
        _logger = logger;
        _orderDetailRepository = orderDetailRepository;
    }

    public InstructorService(IUserRepository userRepository, IInstructorRepository instructorRepository,
        IEmailService emailService, ILogger<InstructorService> logger)
    {
        _userRepository = userRepository;
        _instructorRepository = instructorRepository;
        _emailService = emailService;
        _logger = logger;
    }


    //SPRINT 2

    /// <summary>
    /// Approves an instructor.
    /// </summary>
    /// <param name="instructorId">The ID of the instructor to approve.</param>
    /// <returns></returns>
    public async Task<UserResponse<object>> ApproveInstructorAsync(int instructorId)
    {
        var user = await _userRepository.GetUserById(instructorId);
        if (user == null)
        {
            _logger.LogWarning("User not found !");
            return new UserResponse<object>("User not found !", null);
        }

        if (user.Status == UserStatus.Active || user.Status == UserStatus.Rejected)
        {
            _logger.LogWarning("User has already been active or rejected !");
            return new UserResponse<object>("User has already been active or rejected !", null);
        }

        user.IsVerified = true;
        user.Status = UserStatus.Active; // Using the enum value
        await _userRepository.UpdateAsync(user);
        await _emailService.SendApprovalEmailAsync(user.Email);
        return new UserResponse<object>("Approve Instructor success",
            user); // Return the user or some data indicating success
    }

    /// <summary>
    /// Rejects an instructor.
    /// </summary>
    /// <param name="rejectDto">The rejection details.</param>
    /// <returns></returns>
    public async Task<UserResponse<object>> RejectInstructorAsync(ApproveRejectInstructorDTO rejectDto)
    {
        var user = await _instructorRepository.GetUserByIdAsync(rejectDto.InstructorId);
        if (user == null)
        {
            _logger.LogWarning("User not found !");
            return new UserResponse<object>("User not found !", null);
        }

        if (user.Status == UserStatus.Active || user.Status == UserStatus.Rejected)
        {
            _logger.LogWarning("User has already been approved or rejected !");
            return new UserResponse<object>("User has already been approved or rejected !", null);
        }

        user.Status = UserStatus.Rejected; // Using the enum value
        await _userRepository.UpdateAsync(user);

        await _emailService.SendRejectionEmailAsync(user.Email, rejectDto.Reason);
        return new UserResponse<object>("Reject success", user); // Return the user or some data indicating success
    }


    /// <summary>
    /// Gets the list of pending instructors.
    /// </summary>
    /// <returns>List of pending instructors.</returns>
    public async Task<UserResponse<object>> GetPendingInstructorsAsync()
    {
        var pendingInstructors = await _instructorRepository.GetPendingInstructorsAsync();
        var data = pendingInstructors.Select(instructor => new PendingInstructorDTO
        {
            UserId = instructor.UserId,
            FullName = instructor.FullName,
            Email = instructor.Email,
            PhoneNumber = instructor.PhoneNumber,
            Status = instructor.Status,
            CertificationFilePath = instructor.InstructorData?.Certification
        }).ToList();
        if (data.Count == 0)
        {
            return new UserResponse<object>("Not have any instructor in pending", null);
        }

        return new UserResponse<object>("This is pending list instructor", data);
    }

    /// <summary>
    /// Gets the details of an instructor.
    /// </summary>
    /// <param name="id">The ID of the instructor.</param>
    /// <returns>The instructor details.</returns>
    public async Task<UserResponse<object>> GetInstructorDataAsync(int id)
    {
        //get instructor
        var user = await _instructorRepository.GetUserByIdAsync(id);
        ManageInstructorDTO instructor;
        //check if user is null
        if (user == null)
        {
            _logger.LogWarning("User not found !");
            return new UserResponse<object>("User not found !", null);
        }

        //get course of instructor and calculate total and active
        var courses = await _instructorRepository.GetAllCourseOfInstructor(user.UserId);
        var totalCourses = courses.Count;
        var activatedCourses = courses?.Count(c => c.Status == "Activated") ?? 0;

        //get all data with instructorId and course is active in studentInCourse table 
        var numberStudentInCourse = await _instructorRepository.GetStudentInCourse(user.UserId);

        //get comment from other function
        List<CommentUserDetail> adminComment = await ViewCommentDetail(user.UserId);

        //check if no data in StudentInCourse, return data need calculate is 0
        if (numberStudentInCourse.Count == 0)
        {
            instructor = new ManageInstructorDTO()
            {
                Id = user.UserId,
                Email = user.Email,
                Name = user.FullName,
                Phone = user.PhoneNumber,
                Status = user.Status,
                ActivatedCourses = activatedCourses,
                TotalCourses = totalCourses,
                TotalEarnedMoney = 0,
                TotalPayout = 0,
                RatingNumber = 0,
                AdminComment = adminComment
            };
            return new UserResponse<object>("This is instructor data", instructor);
        }

        //calculate totalEarned by price of course * numberOfStudent learn this course / total number of student
        decimal totalEarned = numberStudentInCourse
            .SelectMany(s => s.Courses)
            .Sum(c => c.Price * numberStudentInCourse.Count(s => s.CourseId == c.Id));
        int totalStudent = numberStudentInCourse
            .SelectMany(s => s.Courses)
            .Count();
        totalEarned /= totalStudent;

        //calculate totalPayout = 95% totalEarned
        decimal totalPayout = totalEarned * 90 / 100;

        //calculate rating of instructor
        double rating = 0;
        foreach (var studentInCourse in numberStudentInCourse)
        {
            rating += studentInCourse.Rating;
        }

        rating /= numberStudentInCourse.Count;

        instructor = new ManageInstructorDTO()
        {
            Id = user.UserId,
            Email = user.Email,
            Name = user.FullName,
            Phone = user.PhoneNumber,
            ActivatedCourses = activatedCourses,
            TotalCourses = totalCourses,
            TotalEarnedMoney = totalEarned,
            TotalPayout = totalPayout,
            RatingNumber = rating,
            AdminComment = adminComment
        };
        return new UserResponse<object>("This is instructor data", instructor);
    }

    /// <summary>
    /// Gets the details of all instructors.
    /// </summary>
    /// <returns>List of all instructor details.</returns>
    public async Task<UserResponse<object>> GetAllInstructorDataAsync(int pageIndex, int pageSize)
    {
        var user = await _instructorRepository.GetAllInstructorAsync();

        //check if list instructor is null
        if (user == null)
        {
            return new UserResponse<object>("List instructor not found", null);
        }

        var list = await GetAllInstructorData(user);

        if (pageIndex != 0)
        {
            int totalCount = list.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);


            // Extract the specific page
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        return new UserResponse<object>("This is list instructor data", list);
    }

    public async Task<UserResponse<object>> SubmitCourse(int id)
    {
        //take id of admin by access token
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();
        if (token == null)
            throw new Exception("Token not found");
        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        if (jwtToken == null)
            throw new Exception("Invalid token");
        var tokenId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning("Course does not exist !");
            return new UserResponse<object>("Course does not exist !", null);
        }

        if (!course.Status.Equals("Pending"))
        {
            _logger.LogWarning("This course has already been submitted !");
            return new UserResponse<object>("This course has already been submitted !", null);
        }

        var existingChapters = await _courseRepository.GetChaptersByCourseId(id);
        var orders = existingChapters.Select(c => c.Order).ToList();

        if (!orders.Any())
        {
            _logger.LogWarning("There must be at least one chapter!");
            return new UserResponse<object>("There must be at least one chapter!", null);
        }

        var isCheckDuplicate = orders.GroupBy(x => x).Where(g => g.Count() > 1).Any();
        if (isCheckDuplicate)
        {
            _logger.LogWarning("Order numbers must be unique!");
            return new UserResponse<object>("Order numbers must be unique!", null);
        }

        orders.Sort();
        if (orders[0] != 1)
        {
            _logger.LogWarning("Order numbers must begin by 1!");
            return new UserResponse<object>("Order numbers must begin by 1!", null);
        }

        for (int i = 0; i < orders.Count - 1; i++)
        {
            if (orders[i] + 1 != orders[i + 1])
            {
                _logger.LogWarning("Order numbers must be consecutive!");
                return new UserResponse<object>("Order numbers must be consecutive!", null);
            }
        }

        course.Status = "Submitted";
        course.CreatedDate = DateTime.Now;
        var isCheckUpdateCourse = await _instructorRepository.UpdateCourseAsync(course);
        if (isCheckUpdateCourse)
        {
            var admins = await _userRepository.GetUsersByRole(1);
            var emailContent = new EmailSubmitDTO
            {
                CourseId = course.Id,
                Name = course.Name,
            };

            var emailTasks =
                admins.Select(admin => _emailService.SendSubmitCourseEmailAsync(admin.Email, emailContent));
            await Task.WhenAll(emailTasks);

            // Create and save the history course
            HistoryCourse historyCourse = new HistoryCourse()
            {
                CreatedDate = DateTime.Now,
                Description = "This Course is submitted to admin",
                CourseId = id,
                UserId = tokenId,
            };
            bool checkHistory = await _historyCourseRepository.CreateHistoryCourse(historyCourse);
            if (!checkHistory)
            {
                return new UserResponse<object>("Something wrong when save history course", null);
            }

            return new UserResponse<object>("Submitted course successfully", null);
        }

        // Handle failed course submission

        HistoryCourse historyCourseFailed = new HistoryCourse()
        {
            CreatedDate = DateTime.Now,
            Description = "This Course submission failed",
            CourseId = id,
            UserId = course.Instructor.UserId,
        };
        bool checkHistoryFailed = await _historyCourseRepository.CreateHistoryCourse(historyCourseFailed);
        if (!checkHistoryFailed)
        {
            return new UserResponse<object>("Something wrong when save history course", null);
        }

        return new UserResponse<object>("Failed to submit course!", null);
    }


    public async Task<UserResponse<object>> ReviewCourse(int id)
    {
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning("Course is not exist !");
            return new UserResponse<object>("Course is not exist !", null);
        }

        var chapters = await _courseRepository.GetChaptersByCourseId(id);

        var showCourse = new ReviewCourseDTO()
        {
            Id = course.Id,
            Name = course.Name,
            Status = course.Status,
            Description = course.Description,
            Thumbnail = course.Thumbnail,
            ShortSummary = course.ShortSummary,
            AllowComments = course.AllowComments,
            Price = course.Price,
            Chapters = chapters.Select(chapter => new ReviewChapterDTO()
            {
                Order = chapter.Order,
                Duration = chapter.Duration,
                Content = chapter.Content,
                Thumbnail = chapter.Thumbnail,
                Type = chapter.Type
            }).ToList()
        };

        return new UserResponse<object>("Review Successfully", showCourse);
    }

    /// <summary>
    /// Changes the status of an instructor.
    /// </summary>
    /// <param name="contentEmailDto">The email content details.</param>
    /// <param name="id">The ID of the instructor.</param>
    /// <returns>Status update message.</returns>
    public async Task<UserResponse<object>> ChangeStatusInstructor(ContentEmailDTO contentEmailDto, int id)
    {
        var user = await _instructorRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User not found !");
            return new UserResponse<object>("User not found !", null);
        }

        if (user.Status == UserStatus.Pending)
        {
            _logger.LogWarning("This user is in pending status !");
            return new UserResponse<object>("This user is in pending status !", null);
        }

        if (user.Status == UserStatus.Active)
        {
            await _emailService.SendDeactiveEmailAsync(user.Email, contentEmailDto.content);
            user.Status = UserStatus.Inactive;
        }
        else if (user.Status == UserStatus.Inactive)
        {
            await _emailService.SendActiveEmailAsync(user.Email);
            user.Status = UserStatus.Active;
        }
        else
        {
            _logger.LogWarning("Invalid status change request !");
            return new UserResponse<object>("Invalid status change request", null);
        }

        await _instructorRepository.UpdateUserAsync(user);
        return new UserResponse<object>("Status updated successfully", null);
    }


    /// <summary>
    /// Creates a comment for an instructor.
    /// </summary>
    /// <param name="commentDto">The comment details.</param>
    /// <param name="id">The ID of the instructor.</param>
    /// <returns>Comment creation result and message.</returns>
    public async Task<(bool, string)> CreateCommentToInstructor(CommentDTO commentDTO, int id)
    {
        // Get user by id and add to commentUser table
        var user = await _instructorRepository.GetUserByIdAsync(id);

        // Check if userId is in instructor role
        if (user == null)
        {
            _logger.LogWarning($"No user with id {id} in instructor role");
            return (false, $"No user with id {id} in instructor role");
        }

        var adminId = 1;

        if (user.UserId == adminId)
        {
            _logger.LogWarning("Cannot comment to admin !");
            return (false, "Cannot comment to admin");
        }

        var comment = new CommentUser()
        {
            Content = commentDTO.content,
            UserId = user.UserId,
            CommentedById = adminId
        };
        await _instructorRepository.CreateCommentUserAsync(comment);
        return (true, "Add comment success");
    }

    /// <summary>
    /// Views comments by instructor ID.
    /// </summary>
    /// <param name="id">The ID of the instructor.</param>
    /// <returns>List of comments.</returns>
    public async Task<List<ViewCommentDTO>> ViewCommentById(int id)
    {
        var comment = await _instructorRepository.GetCommentById(id);
        List<ViewCommentDTO> list = new List<ViewCommentDTO>();
        foreach (var commentUser in comment)
        {
            ViewCommentDTO viewCommentDto = new ViewCommentDTO()
            {
                Id = commentUser.Id,
                comment = commentUser.Content
            };
            list.Add(viewCommentDto);
        }

        return list;
    }

    /// <summary>
    /// Edits a comment by comment ID.
    /// </summary>
    /// <param name="commentDto">The comment details.</param>
    /// <param name="id">The ID of the comment.</param>
    /// <returns>Boolean indicating success.</returns>
    public async Task<bool> EditCommentByCommentId(CommentDTO commentDto, int id)
    {
        var commentCheck = await _instructorRepository.GetCommentByCommentId(id);
        if (commentCheck == null)
        {
            return false;
        }

        commentCheck.Content = commentDto.content;
        await _instructorRepository.EditCommentByCommentId(commentCheck);
        return true;
    }

    /// <summary>
    /// Deletes a comment by comment ID.
    /// </summary>
    /// <param name="id">The ID of the comment.</param>
    /// <returns>Boolean indicating success.</returns>
    public async Task<bool> DeleteCommentByCommentId(int id)
    {
        var commentCheck = await _instructorRepository.GetCommentByCommentId(id);
        if (commentCheck == null)
        {
            return false;
        }

        await _instructorRepository.DeleteCommentByCommentId(commentCheck);
        return true;
    }

    /// <summary>
    /// Exports instructor data to an Excel file.
    /// </summary>
    /// <returns>Byte array of the Excel file.</returns>
    public async Task<UserResponse<byte[]>> ExportToExcelAsync(int pageIndex, int pageSize)
    {
        var user = await _instructorRepository.GetAllInstructorAsync();
        if (user == null || !user.Any())
        {
            return new UserResponse<byte[]>("Not have any instructor", null);
        }

        var instructors = await GetAllInstructorData(user);
        if (instructors == null || !instructors.Any())
        {
            return new UserResponse<byte[]>("Not have any instructor data", null);
        }

        List<ManageInstructorDTO> instructorData = instructors;
        if (pageIndex != 0)
        {
            int totalCount = instructorData.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);


            // Extract the specific page
            instructorData = instructorData.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("ManageInstructor");

            // File name and export date
            worksheet.Cells[1, 1].Value = "Exported File";
            worksheet.Cells[1, 2].Value = "ManageInstructor.xlsx";
            worksheet.Cells[2, 1].Value = "Export Date";
            worksheet.Cells[2, 2].Value = DateTime.Now.ToString("yyyy-MM-dd");

            // Headers
            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Name";
            worksheet.Cells[1, 4].Value = "Phone";
            worksheet.Cells[1, 5].Value = "Status";
            worksheet.Cells[1, 6].Value = "Number of Activated Courses";
            worksheet.Cells[1, 7].Value = "Total Courses";
            worksheet.Cells[1, 8].Value = "Total Earned Money";
            worksheet.Cells[1, 9].Value = "Total of Payout";
            worksheet.Cells[1, 10].Value = "Rating Number";

            var rowIndex = 2;

            foreach (var instructor in instructorData)
            {
                worksheet.Cells[rowIndex, 1].Value = instructor.Id;
                worksheet.Cells[rowIndex, 2].Value = instructor.Email;
                worksheet.Cells[rowIndex, 3].Value = instructor.Name;
                worksheet.Cells[rowIndex, 4].Value = instructor.Phone;
                worksheet.Cells[rowIndex, 5].Value = instructor.Status;
                worksheet.Cells[rowIndex, 6].Value = instructor.ActivatedCourses;
                worksheet.Cells[rowIndex, 7].Value = instructor.TotalCourses;
                worksheet.Cells[rowIndex, 8].Value = instructor.TotalEarnedMoney;
                worksheet.Cells[rowIndex, 9].Value = instructor.TotalPayout;
                worksheet.Cells[rowIndex, 10].Value = instructor.RatingNumber;

                rowIndex++;
            }

            worksheet.Cells["A1:J1"].Style.Font.Bold = true;
            worksheet.Cells.AutoFitColumns();

            return new UserResponse<byte[]>("This is file excel", package.GetAsByteArray());
        }
    }

    public async Task<UserResponse<object>> ChangeStatusCourse(int id,
        ChangeStatusCourseRequest changeStatusCourseRequest)
    {
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning("Can not find this course !");
            return new UserResponse<object>("Can not find this course !", null);
        }

        if (course.Status.Equals("Pending"))
        {
            _logger.LogWarning("Can not change status this course !");
            return new UserResponse<object>("Can not change status this course !", null);
        }

        course.Status = course.Status == UserStatus.Active.ToString()
            ? UserStatus.Inactive.ToString()
            : UserStatus.Active.ToString();
        var isCheckUpdateCourse = await _courseRepository.EditCourse(course);
        
        HistoryCourse historyCourse = new HistoryCourse()
        {
            CourseId = id,
            Description = $"Change status to {course.Status}",
            CreatedDate = DateTime.Now,
            UserId = course.InstructorId
        };

        bool checkHistory = await _historyCourseRepository.CreateHistoryCourse(historyCourse);
        TimeSpan? deactivationPeriod = null;

        if (changeStatusCourseRequest.DeactivationPeriod != null && changeStatusCourseRequest.ChangeByTime == null)
        {
            return new UserResponse<object>("Please choose what you change by", null);
        }
        else
        {
            switch ((int)changeStatusCourseRequest.ChangeByTime)
            {
                case 0:
                    deactivationPeriod =
                        new TimeSpan(365 * (int)changeStatusCourseRequest.DeactivationPeriod, 0, 0, 0, 0);
                    break;
                case 1:
                    deactivationPeriod = new TimeSpan(7 * (int)changeStatusCourseRequest.DeactivationPeriod, 0, 0, 0);
                    break;
                case 2:
                    deactivationPeriod = new TimeSpan((int)changeStatusCourseRequest.DeactivationPeriod, 0, 0, 0);
                    break;
                default:
                    deactivationPeriod = new TimeSpan((int)changeStatusCourseRequest.DeactivationPeriod, 0, 0);
                    break;
            }
        }

        if (deactivationPeriod.HasValue && course.Status.Equals("Inactive"))
        {
            BackgroundJob.Schedule(() => ReActivateCourse(id), deactivationPeriod.Value);
            return new UserResponse<object>(
                $"Inactive course success, this course will active in {deactivationPeriod.Value} {changeStatusCourseRequest.ChangeByTime}",
                null);
        }

        var students = await _courseRepository.GetStudentInCourse(id);
        if (students == null)
        {
            _logger.LogWarning("Change status success but no one enroll this course !");
            return new UserResponse<object>("Change status success but no one enroll this course !", null);
        }

        foreach (var student in students)
        {
            var user = await _userRepository.GetAllUserById(id);
            if (user != null)
            {
                await _emailService.SendChangeStatusEmailAsync(user.Email, changeStatusCourseRequest.Reason);
            }
        }

        return new UserResponse<object>("Change status success", null);
    }


    [AutomaticRetry(Attempts = 3)] // Optional: Retry the job 3 times in case of failure
    public async Task ReActivateCourse(int courseId)
    {
        var course = await _courseRepository.GetCourseById(courseId);
        course.Status = "Active";
        await _courseRepository.EditCourse(course);
    }

    public async Task<UserResponse<object>> EarningAnalytics()
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();

        if (token == null)
            throw new Exception("Token not found");

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new Exception("Invalid token");

        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

        // Initialize variables
        decimal total = 0;
        decimal payoutEarning = 0;

        // Get data from db
        var courses = await _studentInCourseRepository.GetCourseByInstructorId(userId);

        if (courses == null)
        {
            _logger.LogWarning("Please have student in course to earn money !");
            return new UserResponse<object>("Please have student in course to earn money !", null);
        }

        // Group course by courseId
        var groupedCourses = courses.GroupBy(c => c.CourseId)
            .Select(g => new
            {
                CourseId = g.Key,
                Ratings = g.Select(c => c.Rating).ToList(),
                Users = g.Select(c => c.UserId).Distinct().ToList(),
                Instructors = g.Select(c => c.InstructorId).Distinct().ToList()
            }).ToList();


        // Calculate course earning and total earning
        List<CourseEarning> listEarning = new List<CourseEarning>();
        foreach (var groupedCourse in groupedCourses)
        {
            var course = await _orderDetailRepository.GetOrderDetailByCourseId(groupedCourse.CourseId);
            CourseEarning courseEarning = new CourseEarning()
            {
                CourseId = groupedCourse.CourseId,
                Earning = (course.Select(c => c.CoursePrice).Sum()) * 75 / 100
            };
            listEarning.Add(courseEarning);
            total += courseEarning.Earning;
        }

        // Get transfer
        var transfer = await _instructorPayoutRepository.GetInstructorPayoutByInstructorId(userId);
        if (transfer == null)
        {
            EarningAnalyticRespone noTransfer = new EarningAnalyticRespone()
            {
                TotalEarning = total,
                PayoutEarning = payoutEarning,
                MaintainMoney = total - payoutEarning,
                CourseEarning = listEarning,
            };
            _logger.LogWarning("You don't have any history transfer !");
            return new UserResponse<object>("You don't have any history transfer !", noTransfer);
        }

        // Calculate history transfer
        List<HistoryTransfer> listTransfer = new List<HistoryTransfer>();
        foreach (var instructorPayout in transfer)
        {
            HistoryTransfer historyTransfer = new HistoryTransfer()
            {
                TransferDate = instructorPayout.PayoutDate,
                Amount = instructorPayout.PayoutAmount,
                Status = instructorPayout.PayoutStatus
            };
            listTransfer.Add(historyTransfer);
            decimal moneyPayout = instructorPayout.PayoutStatus == PayoutStatus.Approved ? historyTransfer.Amount : 0;
            payoutEarning += moneyPayout;
        }

        EarningAnalyticRespone response = new EarningAnalyticRespone()
        {
            TotalEarning = total,
            PayoutEarning = payoutEarning,
            MaintainMoney = total - payoutEarning, //Balance aacount
            CourseEarning = listEarning,
            HistoryTransfers = listTransfer
        };
        return new UserResponse<object>("This is data of earning analytics", response);
    }


    public async Task<UserResponse<object>> ViewReview(int id, int pageSize, int pageIndex)
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();

        if (token == null)
            throw new Exception("Token not found");

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new Exception("Invalid token");

        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

        //check if course is created by instructor
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning("This course is unavailable !");
            return new UserResponse<object>("This course is unavailable !", null);
        }

        if (course.InstructorId != userId)
        {
            _logger.LogWarning("You don't have permission to view review of this course !");
            return new UserResponse<object>("You don't have permission to view review of this course !", null);
        }

        //check this course have feedbacks or not
        var feedBacks = await _feedBackRepository.GetFeedbackByCourseId(id);
        if (feedBacks.Count == 0)
        {
            _logger.LogWarning("This course don't have any review !");
            return new UserResponse<object>("This course don't have any review !", null);
        }

        //add data to respone
        List<UnHideReviewRespone> unHideReviewRespones = new List<UnHideReviewRespone>();
        List<HideReviewRespone> hideReviewRespones = new List<HideReviewRespone>();

        foreach (var feedBack in feedBacks)
        {
            if (feedBack.IsDelete == true)
            {
                HideReviewRespone hideReviewRespone = new HideReviewRespone()
                {
                    Id = feedBack.Id,
                    Email = feedBack.User.Email,
                    Content = feedBack.Content,
                    Rating = feedBack.ReviewPoint,
                    ReasonHide = feedBack.ReportFeedback.ReportReason
                };
                hideReviewRespones.Add(hideReviewRespone);
            }
            else
            {
                UnHideReviewRespone item = new UnHideReviewRespone()
                {
                    Id = feedBack.Id,
                    Email = feedBack.User.Email,
                    Content = feedBack.Content,
                    Rating = feedBack.ReviewPoint,
                    IsGoodFeedBack = feedBack.IsMarkGood
                };
                unHideReviewRespones.Add(item);
            }
        }

        //Paging
        if (unHideReviewRespones.Count < pageSize * pageIndex && pageIndex != 1)
        {
            _logger.LogWarning("No data in this page");
            return new UserResponse<object>("No data in this page", null);
        }

        unHideReviewRespones = unHideReviewRespones.Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewReviewRespone data = new ViewReviewRespone()
        {
            HideReviewRespones = hideReviewRespones,
            UnHideReviewRespones = unHideReviewRespones,
        };

        return new UserResponse<object>("This is review of course", data);
    }

    public async Task<UserResponse<object>> toggleMarkGoodReview(int id)
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();

        if (token == null)
            throw new Exception("Token not found");

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new Exception("Invalid token");

        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

        //check if course is created by instructor or not
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            return new UserResponse<object>("This course is unavailable", null);
        }

        if (course.InstructorId != userId)
        {
            return new UserResponse<object>("You don't have permission to mark this course", null);
        }

        //change status of mark good feedback
        bool newMark = true;
        var feedback = await _feedBackRepository.GetFeedbackById(id);
        if (feedback == null)
        {
            return new UserResponse<object>("This feedback is not available", null);
        }

        newMark = feedback.IsMarkGood != true;
        feedback.IsMarkGood = newMark;
        bool checkMark = await _feedBackRepository.UpdateFeedback(feedback);

        if (!checkMark)
        {
            return new UserResponse<object>("Something wrong when mark this feedback", null);
        }

        //Return
        if (newMark)
        {
            return new UserResponse<object>("Mark this feedback to good feedback success", null);
        }
        else
        {
            return new UserResponse<object>("Unmark this good feedback success", null);
        }
    }

    /// <summary>
    /// Views comment details by instructor ID.
    /// </summary>
    /// <param name="id">The ID of the instructor.</param>
    /// <returns>List of comment details.</returns>
    private async Task<List<CommentUserDetail>> ViewCommentDetail(int id)
    {
        var comment = await _instructorRepository.GetCommentById(id);
        List<CommentUserDetail> list = new List<CommentUserDetail>();
        foreach (var commentUser in comment)
        {
            CommentUserDetail viewCommentDto = new CommentUserDetail()
            {
                comment = commentUser.Content
            };
            list.Add(viewCommentDto);
        }

        return list;
    }

    private async Task<List<ManageInstructorDTO>> GetAllInstructorData(List<User> user)
    {
        List<ManageInstructorDTO> list = new List<ManageInstructorDTO>();
        foreach (var instructor in user)
        {
            //get course of instructor and calculate total and active
            var courses = await _instructorRepository.GetAllCourseOfInstructor(instructor.UserId);
            int totalCourses = courses.Count;
            int activatedCourses = courses?.Count(c => c.Status == "Active") ?? 0;

            //get all data with instructorId and course is active in studentInCourse table 
            var numberStudentInCourse = await _instructorRepository.GetStudentInCourse(instructor.UserId);
            decimal totalEarned;
            int totalStudent;
            if (numberStudentInCourse != null)
            {
                totalEarned = 0;
            }
            else
            {
                totalEarned = numberStudentInCourse
                    .SelectMany(s => s.Courses)
                    .Sum(c => c.Price * numberStudentInCourse.Count(s => s.CourseId == c.Id));
                totalStudent = numberStudentInCourse
                    .SelectMany(s => s.Courses)
                    .Count();
                totalEarned /= totalStudent;
            }

            //get comment from other function
            List<CommentUserDetail> adminComment = await ViewCommentDetail(instructor.UserId);

            // check if no data in StudentInCourse, return data need calculate is 0
            if (!numberStudentInCourse.Any())
            {
                list.Add(new ManageInstructorDTO()
                {
                    Id = instructor.UserId,
                    Email = instructor.Email,
                    Name = instructor.FullName,
                    Phone = instructor.PhoneNumber,
                    Status = instructor.Status,
                    ActivatedCourses = activatedCourses,
                    TotalCourses = totalCourses,
                    TotalEarnedMoney = 0,
                    TotalPayout = 0,
                    RatingNumber = 0,
                    AdminComment = adminComment
                });
                continue;
            }

            //calculate totalEarned by price of course * numberOfStudent learn this course / total number of student


            //calculate totalPayout = 95% totalEarned
            decimal totalPayout = totalEarned * 95 / 100;

            //calculate rating of instructor
            double rating = 0;
            foreach (var studentInCourse in numberStudentInCourse)
            {
                rating += studentInCourse.Rating;
            }

            rating /= numberStudentInCourse.Count;

            list.Add(new ManageInstructorDTO()
            {
                Id = instructor.UserId,
                Email = instructor.Email,
                Name = instructor.FullName,
                Phone = instructor.PhoneNumber,
                Status = instructor.Status,
                ActivatedCourses = activatedCourses,
                TotalCourses = totalCourses,
                TotalEarnedMoney = totalEarned,
                TotalPayout = totalPayout,
                RatingNumber = rating,
                AdminComment = adminComment
            });
        }

        //sort by id descending
        list.Sort((x, y) => y.Id.CompareTo(x.Id));

        return list;
    }
}