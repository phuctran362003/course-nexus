using System.Drawing.Printing;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.Repositories;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace Curus.Service.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBlobService _blobService;
    private readonly IEmailService _emailService;
    private readonly IBackUpCourseRepository _backUpCourseRepository;
    private readonly IBackUpChapterRepository _backUpChapterRepository;
    private readonly IHistoryCourseRepository _historyCourseRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IFeedBackRepository _feedBackRepository;
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<CourseService> _logger;

    public CourseService(ICourseRepository courseRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ICategoryRepository categoryRepository, IBlobService blobService, IEmailService emailService, IBackUpCourseRepository backUpCourseRepository, IBackUpChapterRepository backUpChapterRepository, IHistoryCourseRepository historyCourseRepository, IChapterRepository chapterRepository, IFeedBackRepository feedBackRepository, IReportRepository reportRepository, ILogger<CourseService> logger)
    {
        _courseRepository = courseRepository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _blobService = blobService;
        _emailService = emailService;
        _backUpCourseRepository = backUpCourseRepository;
        _backUpChapterRepository = backUpChapterRepository;
        _historyCourseRepository = historyCourseRepository;
        _chapterRepository = chapterRepository;
        _feedBackRepository = feedBackRepository;
        _reportRepository = reportRepository;
        _logger = logger;
    }


    public async Task<UserResponse<object>> CreateCourse(CourseDTO courseDto)
{
    var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
        .Last();

    if (token == null)
        throw new Exception("Token not found");

    var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

    if (jwtToken == null)
        throw new Exception("Invalid token");

    var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

    var user = await _userRepository.GetAllUserById(userId);

    if (user == null)
    {
        _logger.LogWarning("User does not exist");
        return new UserResponse<object>("User does not exist", null);
    }

    if (!user.Status.Equals(UserStatus.Active))
    {
        _logger.LogWarning("User is not approved by admin");
        return new UserResponse<object>("User is not approved by admin", null);
    }

    var getAllCategoryParent = await _categoryRepository.GetAllCategory();

    var isCheckCourseName = await _courseRepository.GetCourseByName(courseDto.Name);
    if (isCheckCourseName != null)
    {
        _logger.LogWarning("Course name should be unique");
        return new UserResponse<object>("Course name should be unique", null);
    }
    foreach (var categoryId in courseDto.CategoryIds)
    {
        var isCheckCategory = await _categoryRepository.GetCategoryById(categoryId);
        if (isCheckCategory == null)
        {
            _logger.LogWarning($"Category with ID {categoryId} does not exist!");
            return new UserResponse<object>($"Category with ID {categoryId} does not exist!", null);
        }
        if (isCheckCategory.ParentCategory == null)
        {
            foreach (var categorParentId in getAllCategoryParent)
            {
                if (categorParentId.HasValue && categorParentId == categoryId)
                {
                    _logger.LogWarning($"Should use subcategories in this category {isCheckCategory.Id}");
                    return new UserResponse<object>($"Should use subcategories in this category {isCheckCategory.Id}", null);
                }
            }
        }
    }
    if (courseDto.CategoryIds.Count > 2)
    {
        _logger.LogWarning("A course cannot have more than 2 categories");
        return new UserResponse<object>("A course cannot have more than 2 categories", null);
    }
    
    var isCheckThumbnail = Path.GetExtension(courseDto.Thumbnail.FileName).ToLower();
    if (isCheckThumbnail != ".png" && isCheckThumbnail != ".jpg" && isCheckThumbnail != ".jpeg")
    {
        _logger.LogWarning("Incorrect format thumbnail file!");
        return new UserResponse<object>("Incorrect format thumbnail file!", null);
    }

    var course = new Course()
    {
        Name = courseDto.Name,
        Description = courseDto.Description,
        Price = courseDto.Price,
        OldPrice = courseDto.Price,
        Status = "Pending",
        Version = "1.0",
        Point = 0,
        InstructorId = userId,
        ShortSummary = courseDto.ShortSummary,
        AllowComments = courseDto.AllowComments,
        CreatedDate = DateTime.UtcNow,
        Thumbnail = await _blobService.UploadFileAsync(courseDto.Thumbnail),
    };

    var isCheckCreateCourse = await _courseRepository.CreateCourse(course);
    if (!isCheckCreateCourse)
    {
        _logger.LogWarning("Created course failed!");
        return new UserResponse<object>("Created course failed!", null);
    }
    

    foreach (var categoryId in courseDto.CategoryIds)
    {
        var courseCategory = new CourseCategory
        {
            CourseId = course.Id,
            CategoryId = categoryId
        };
        await _categoryRepository.CreateCourseCategory(courseCategory);
    }

    return new UserResponse<object>("Created course successfully", null);
}

    public async Task<List<Chapter>> GetChaptersByCourseId(int id)
    {
        return await _courseRepository.GetChaptersByCourseId(id);
    }

    public async Task<UserResponse<object>> EditDraft(int id, CourseEditDTO courseDto)
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();

        if (token == null)
            throw new Exception("Token not found");

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new Exception("Invalid token");

        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning("This course is not exist !");
            return new UserResponse<object>("This course is not exist !", null);
        }

        if (!userId.Equals(course.InstructorId))
        {
            _logger.LogWarning("This user can not edit this course !");
            return new UserResponse<object>("This user can not edit this course !", null);
        }

        if (courseDto.CategoryIds != null)
        {
            foreach (var categoryId in courseDto.CategoryIds)
            {
                var isCheckCategory = await _categoryRepository.GetCategoryById(categoryId);
                if (isCheckCategory == null)
                {
                    _logger.LogWarning($"Category with ID {categoryId} does not exist !");
                    return new UserResponse<object>($"Category with ID {categoryId} does not exist !", null);
                }
            }
        }
        
        if (course.Status.Equals("Submitted"))
        {
            _logger.LogWarning("This course is submitted, can not edit !");
            return new UserResponse<object>("This course is submitted, can not edit !", null);
        }

        if (courseDto.Name == null && courseDto.AllowComments == null && courseDto.Description == null && courseDto.Thumbnail ==null && courseDto.CategoryIds == null && courseDto.Price == null && courseDto.ShortSummary ==null)
        {
            return new UserResponse<object>("You need to change at least 1", null);
        }
        
        var existCourse = await _backUpCourseRepository.GetBackUpCourseByCourseId(id);
        if (existCourse.Price != existCourse.OldPrice)
        {
            return new UserResponse<object>("This course can not edit in discount period !", null);
        }
        string version;
        if (existCourse == null)
        {
            version = "1.0";
        }
        else
        {
            if (int.Parse(existCourse.Version.Split(".")[1]) == 9)
            {
                version = int.Parse(existCourse.Version.Split(".")[0] + 1) + ".0";
            }
            else
            {
                version = int.Parse(existCourse.Version.Split(".")[0]) + "." + int.Parse(existCourse.Version.Split(".")[1] + 1);
            }
        }
        course.Name = courseDto.Name == null ? course.Name : courseDto.Name;
        course.Description = courseDto.Description == null ? course.Description : courseDto.Description;
        course.Price = courseDto.Price == null ? course.OldPrice : courseDto.Price;
        course.OldPrice = course.Price;
        course.Status = "Pending";
        course.Version = version;
        course.Point = 0;
        course.ShortSummary = courseDto.ShortSummary == null ? course.ShortSummary : courseDto.ShortSummary;
        if (courseDto.AllowComments == null)
        {
            course.AllowComments = course.AllowComments;
        }
        else
        {
            course.AllowComments = courseDto.AllowComments == true ? false : true;
        }
        if (courseDto.Thumbnail != null)
        {
            course.Thumbnail = await _blobService.UploadFileAsync(courseDto.Thumbnail);
        }
        var isCheckEditCourse = await _courseRepository.EditCourse(course);
        if (!isCheckEditCourse)
        {
            _logger.LogWarning("Save draft fail !");
            return new UserResponse<object>("Save draft fail !", null);
        }

        if (courseDto.CategoryIds != null)
        {
            await _categoryRepository.DeleteCourseCategory(id);
            foreach (var categoryId in courseDto.CategoryIds)
            {
                var courseCategory = new CourseCategory
                {
                    CourseId = course.Id,
                    CategoryId = categoryId
                };
                await _categoryRepository.CreateCourseCategory(courseCategory);
            }
        }

        return new UserResponse<object>("Save draft success", course);
    }

    public async Task<UserResponse<object>> ReviewRejectCourse()
    {
        List<RejectCourseRespone> list = new List<RejectCourseRespone>();
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();

        if (token == null)
            throw new Exception("Token not found");

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new Exception("Invalid token");

        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);
        var allCourse = await _courseRepository.GetCourseByInstructorId(userId);
        var courses = allCourse.Where(c => c.Status == "Rejected").ToList();
        if (courses == null)
        {
            _logger.LogWarning("This instructor not have any course or not have reject course !");
            return new UserResponse<object>("This instructor not have any course or not have reject course !", null);
        }

        foreach (var course in courses)
        {
            //tracking chapter in course
            List<ReviewRejectChapterDTO>? chapters = new List<ReviewRejectChapterDTO>();
            var chapter = await _courseRepository.GetChaptersByCourseId(course.Id);
            foreach (var item in chapter)
            {
                ReviewRejectChapterDTO chapterDto = new ReviewRejectChapterDTO()
                {
                    CourseId = item.CourseId,
                    Content = item.Content,
                    Thumbnail = item.Thumbnail,
                    Order = item.Order,
                    Duration = item.Duration,
                    Type = item.Type,
                };
                chapters.Add(chapterDto);
            }

            List<ReviewRejectChapterDTO> sortedChapters = chapters.OrderBy(c => c.Order).ToList();


            //tracking reason
            var historyCourse = await _historyCourseRepository.GetAllHistoryOfCourseByCourseid(course.Id);
            List<HistoryCourseDTO> listHistory = new List<HistoryCourseDTO>();
            foreach (var history in historyCourse)
            {
                HistoryCourseDTO historyCourseDto = new HistoryCourseDTO()
                {
                    Date = history.CreatedDate,
                    Description = history.Description,
                };
                listHistory.Add(historyCourseDto);
            }


            RejectCourseRespone rejectCourseRespone = new RejectCourseRespone()
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Thumbnail = course.Thumbnail,
                ShortSummary = course.ShortSummary,
                InstructorId = course.InstructorId,
                Point = course.Point,
                Price = course.Price,
                Chapters = sortedChapters,
                History = listHistory
            };
            list.Add(rejectCourseRespone);
        }

        return new UserResponse<object>("This is your list reject course", list);

    }
    
public async Task<UserResponse<object>> UpdateStatusCourse(ChangeStatusCourseRequest changeStatusCourseRequest,
        int id)
    {
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning("Course not found !");
            return new UserResponse<object>("Course not found !", null);
        }

        var instructor = await _userRepository.GetAllUserById(course.InstructorId);

        if (course.Status.Equals("Pending"))
        {
            _logger.LogWarning("This course is in pending status !");
            return new UserResponse<object>("This course is in pending status !", null);
        }

        if (course.Status.Equals("Active"))
        {
            await _emailService.SendChangeStatusEmailAsync(instructor.Email, changeStatusCourseRequest.Reason);
            course.Status = "Inactive";
            course.Reason = changeStatusCourseRequest.Reason;
            course.AdminModified = true;
        }
        else if (course.Status.Equals("Inactive"))
        {
            await _emailService.SendChangeStatusEmailAsync(instructor.Email, changeStatusCourseRequest.Reason);
            course.Status = "Active";
        }
        else
        {
            _logger.LogWarning("Invalid status change request !");
            return new UserResponse<object>("Invalid status change request !", null);
        }

        course.Status = course.Status == UserStatus.Active.ToString() ? UserStatus.Inactive.ToString() : UserStatus.Active.ToString();
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
                    deactivationPeriod = new TimeSpan(365 * (int)changeStatusCourseRequest.DeactivationPeriod, 0, 0, 0, 0);
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
                $"Inactive course success, this course will active in {changeStatusCourseRequest.DeactivationPeriod} {changeStatusCourseRequest.ChangeByTime}",
                null);
        }

        if (!isCheckUpdateCourse)
        {
            return new UserResponse<object>("Update course fail", null);
        }

        return new UserResponse<object>("Update course successfully", null);
    }

    [AutomaticRetry(Attempts = 3)] // Optional: Retry the job 3 times in case of failure
    public async Task ReActivateCourse(int courseId)
    {
        var course = await _courseRepository.GetCourseById(courseId);
        course.Status = "Active";
        await _courseRepository.EditCourse(course);
    }

    public async Task<UserResponse<object>> ViewSubmitCourseByAdmin()
    {
        var courses = await _courseRepository.GetCourseByStatus();
        if (courses == null)
        {
            return new UserResponse<object>("No course is submitted", null);
        }

        ReviewCourseDTO showCourse = new ReviewCourseDTO();

        foreach (var course in courses)
        {
            var chapters = await _courseRepository.GetChaptersByCourseId(course.Id);
            showCourse = new ReviewCourseDTO()
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
        }

        return new UserResponse<object>("This is course waiting to approve/reject", showCourse);
    }

   public async Task<byte[]> ExportCourseToExcelAsync()
{
    var courseData = await GetInfoCourse(0, 0, null, false);
    var exportDate = DateTime.Now.ToString("yyyyMMdd");
    var worksheetName = $"ManageCourse_{exportDate}";

    using (var package = new ExcelPackage())
    {
        var worksheet = package.Workbook.Worksheets.Add(worksheetName);
        
        // Headers
        worksheet.Cells[4, 1].Value = "Id";
        worksheet.Cells[4, 2].Value = "Course Name";
        worksheet.Cells[4, 3].Value = "Instructor Name";
        worksheet.Cells[4, 4].Value = "Category Name";
        worksheet.Cells[4, 5].Value = "Number Of Student";
        worksheet.Cells[4, 6].Value = "Version";
        worksheet.Cells[4, 7].Value = "Total Of Purchase";
        worksheet.Cells[4, 8].Value = "Rating Number";

        var rowIndex = 5;

        foreach (var course in courseData)
        {
            worksheet.Cells[rowIndex, 1].Value = course.Id;
            worksheet.Cells[rowIndex, 2].Value = course.Name;
            worksheet.Cells[rowIndex, 3].Value = course.InstructorName;
            List<ViewCategoryNameDTO> categoryNameList = await ViewCategoryName(course.Id);
            var categoryNames = categoryNameList.Select(c => c.CategoryName).ToArray();
            worksheet.Cells[rowIndex, 4].Value = string.Join(", ", categoryNames);
            worksheet.Cells[rowIndex, 5].Value = course.NumberOfStudent;
            worksheet.Cells[rowIndex, 6].Value = course.Version;
            worksheet.Cells[rowIndex, 7].Value = course.TotalOfPurchased;
            worksheet.Cells[rowIndex, 8].Value = course.Rating;

            rowIndex++;
        }

        worksheet.Cells["A4:H4"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }
}

   
    public async Task<List<ManageCourseDTO>> GetInfoCourse(int pageSize, int pageIndex, string sortBy, bool sortDesc)
    {
        IEnumerable<Course> courses;

        if (pageSize == 0 && pageIndex == 0)
        {
            courses = await _courseRepository.GetAllCoursesAsync(sortBy, sortDesc);
        }
        else
        {
            courses = await _courseRepository.GetPendingCoursesAsync(pageSize, pageIndex, sortBy, sortDesc);
        }

        var courseDtoList = new List<ManageCourseDTO>();
        foreach (var course in courses)
        {
            var instructor = await _userRepository.GetAllUserById(course.InstructorId);
            List<ViewCategoryNameDTO> categoryName = await ViewCategoryName(course.Id);
            List<CommentUserDetail> adminComment = await ViewCommentDetail(course.Id);

            var countStudent = await _courseRepository.CountStudentInCourses(course.Id);
            var earnedMoney = course.Price * countStudent;
            var courseDto = new ManageCourseDTO()
            {
                Id = course.Id,
                Name = course.Name,
                InstructorName = instructor.FullName,
                CategoryName = categoryName,
                NumberOfStudent = countStudent,
                Version = course.Version,
                TotalOfPurchased = earnedMoney,
                Rating = 3.5,
                AdminComment = adminComment
            };
            courseDtoList.Add(courseDto);
        }

        return courseDtoList;
    }

    public async Task<UserResponse<object>> CreateCommentToCourse(CommentDTO commentDTO, int id)
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

        // Get user by id and add to commentUser table
        var course = await _courseRepository.GetCourseById(id);

        // Check if userId is in instructor role
        if (course == null)
        {
            return new UserResponse<object>($"No course with id {id} is exist", null);
        }

        var comment = new CommentCourse()
        {
            Content = commentDTO.content,
            UserId = tokenId,
            CourseId = id,
            ByAdmin = true,
        };
        var isCheckComment = await _courseRepository.CreateCourseComment(comment);
        if (!isCheckComment)
        {
            return new UserResponse<object>("Comment to course fail !", null);
        }
        return new UserResponse<object>("Comment to course successfully", null);
    }

    public async Task<List<CommentUserDetail>> ViewCommentDetail(int id)
    {
        var comment = await _courseRepository.GetCourseCommentById(id);
        List<CommentUserDetail> list = new List<CommentUserDetail>();
        foreach (var commentCourse in comment)
        {
            if (commentCourse.ByAdmin == true)
            {
                CommentUserDetail viewCommentDto = new CommentUserDetail()
                {
                    comment = commentCourse.Content
                };
                list.Add(viewCommentDto);
            }
        }

        return list;
    }

    public async Task<List<ViewCategoryNameDTO>> ViewCategoryName(int id)
    {
        var categories = await _categoryRepository.GetCategoryNameByCourseId(id);
        List<ViewCategoryNameDTO> list = new List<ViewCategoryNameDTO>();
        foreach (var category in categories)
        {
            ViewCategoryNameDTO CategoryName = new ViewCategoryNameDTO()
            {
                CategoryName = category.CategoryName
            };
            list.Add(CategoryName);
        }
        return list;
    }

    public async Task<UserResponse<object>> ManageCourseDetail(int id)
    {
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning($"No course with id {id} is exist");
            return new UserResponse<object>($"No course with id {id} is exist", null);
        }

        var user = await _userRepository.GetAllUserById(course.InstructorId);

        var chapters = await _courseRepository.GetChaptersByCourseId(id);
        List<ViewCategoryNameDTO> categoryName = await ViewCategoryName(course.Id);
        List<CommentUserDetail> studentComment = await ViewStudentCommentDetail(course.Id);

        var countStudent = await _courseRepository.CountStudentInCourses(course.Id);
        var earnedMoney = course.Price * countStudent;

        var showCourse = new ReviewCourseDTO()
        {
            Id = course.Id,
            Name = course.Name,
            CategoryName = categoryName,
            Status = course.Status,
            Description = course.Description,
            Thumbnail = course.Thumbnail,
            ShortSummary = course.ShortSummary,
            AllowComments = course.AllowComments,
            Price = course.Price,
            EarnedMoney = earnedMoney,
            InstructorName = user.FullName,
            StudentComment = studentComment,


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

    public async Task<List<CommentUserDetail>> ViewStudentCommentDetail(int id)
    {
        var comment = await _courseRepository.GetCourseCommentById(id);
        List<CommentUserDetail> list = new List<CommentUserDetail>();
        foreach (var commentCourse in comment)
        {
            if (commentCourse.ByAdmin == false)
            {
                CommentUserDetail viewCommentDto = new CommentUserDetail()
                {
                    comment = commentCourse.Content
                };
                list.Add(viewCommentDto);
            }
        }

        return list;
    }

    public async Task<UserResponse<object>> StudentCreateCommentCourse(CommentDTO commentDTO, int id)
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

        // Get user by id and add to commentUser table
        var course = await _courseRepository.GetCourseById(id);

        // Check if userId is in instructor role
        if (course == null)
        {
            return new UserResponse<object>($"No course with id {id} is exist", null);
        }

        var comment = new CommentCourse()
        {
            Content = commentDTO.content,
            UserId = tokenId,
            CourseId = id,
        };
        var isCheckComment = await _courseRepository.CreateCourseComment(comment);
        if (!isCheckComment)
        {
            return new UserResponse<object>("Comment to course fail !", null);
        }

        return new UserResponse<object>("Comment to course successfully", null);
    }

    public async Task<UserResponse<object>> ReportCourseById(ReportCourseDTO reportCourseDto, int id)
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

        if (reportCourseDto.Content == null)
        {
            return new UserResponse<object>("Content field is require !", null);
        }
        
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning($"No course with id {id} is exist !");
            return new UserResponse<object>($"No course with id {id} is exist !", null);
        }

        if (course.Status == "Pending")
        {
            _logger.LogWarning($"Course with id {id} is Pending !");
            return new UserResponse<object>($"Course with id {id} is Pending !", null);
        }

        var isCheckStudentInCourse = await _courseRepository.GetStudentInCourseById(tokenId,id);
        if (isCheckStudentInCourse == null)
        {
            _logger.LogWarning("You just can report your enrolled course !");
            return new UserResponse<object>("You just can report your enrolled course !", null);
        }
        
        var user = await _userRepository.GetAllUserById(course.InstructorId);

        if (reportCourseDto.Attachment != null)
        {
            var isCheckThumbnail = Path.GetExtension(reportCourseDto.Attachment.FileName).ToLower();
            if (isCheckThumbnail != ".png" && isCheckThumbnail != ".jpg" && isCheckThumbnail != ".jpeg")
            {
                _logger.LogWarning("Incorrect format thumbnail file !");
                return new UserResponse<object>("Incorrect format thumbnail file !", null);
            }
        }

        var thumbnail = reportCourseDto.Attachment != null
            ? await _blobService.UploadFileAsync(reportCourseDto.Attachment)
            : course.Thumbnail;
        

        var studentReport = new Report()
        {
            Content = reportCourseDto.Content,
            Attachment = thumbnail,
            Status = ReportStatus.Submitted,
            CourseId = course.Id,
            UserId = tokenId
        };
        
        
        var isCheckStudentReport = await _reportRepository.GetReportByUserId(tokenId, id);
        if (isCheckStudentReport != null)
        {
            _logger.LogWarning("You are already report this course !");
            return new UserResponse<object>("You are already report this course !", null);
        }
        
        var isCheckReport = await _courseRepository.CreateReportCourse(studentReport);
        if (!isCheckReport)
        {
            _logger.LogWarning("Report fail !");
            return new UserResponse<object>("Report fail !", null);
        }

        await _emailService.SendReportStudentMail(user.Email, reportCourseDto.Content);
        return new UserResponse<object>("Report successfully", null);
    }

    public async Task<UserResponse<object>> ReportChapterById(ReportCourseDTO reportCourseDto, int id)
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

        if (reportCourseDto.Content == null)
        {
            return new UserResponse<object>("Content field is require !", null);
        }

        var chapter = await _chapterRepository.GetChapterById(id);
        if (chapter == null)
        {
            _logger.LogWarning($"No chapter with id {id} is exist !");
            return new UserResponse<object>($"No chapter with id {id} is exist !", null);
        }

        var course = await _courseRepository.GetCourseById(chapter.CourseId);
        if (course == null)
        {
            _logger.LogWarning("This course is not exist !");
            return new UserResponse<object>("This course is not exist !", null); 
        }
        var user = await _userRepository.GetAllUserById(course.InstructorId);
        
        var isCheckStudentInCourse = await _courseRepository.GetStudentInCourseById(tokenId,course.Id);
        if (isCheckStudentInCourse == null)
        {
            _logger.LogWarning("You just can report your enrolled course !");
            return new UserResponse<object>("You just can report your enrolled course !", null);
        }

        if (reportCourseDto.Attachment != null)
        {
            var isCheckThumbnail = Path.GetExtension(reportCourseDto.Attachment.FileName).ToLower();
            if (isCheckThumbnail != ".png" && isCheckThumbnail != ".jpg" && isCheckThumbnail != ".jpeg")
            {
                _logger.LogWarning("incorrect format thumbnail file !");
                return new UserResponse<object>("incorrect format thumbnail file !", null);
            }
        }

        var attachment = reportCourseDto.Attachment != null
            ? await _blobService.UploadFileAsync(reportCourseDto.Attachment)
            : null;
        
        var studentReport = new Report()
        {
            Content = reportCourseDto.Content,
            Attachment = attachment,
            Status = ReportStatus.Submitted,
            CourseId = course.Id,
            ChapterId = id, 
            UserId = tokenId
        };
        
        var isCheckStudentReport = await _reportRepository.GetReportByChapterId(tokenId,id);
        if (isCheckStudentReport != null)
        {
            _logger.LogWarning("You are already report this course !");
            return new UserResponse<object>("You are already report this course !", null);
        }
        

        var isCheckReport = await _courseRepository.CreateReportCourse(studentReport);
        if (!isCheckReport)
        {
            _logger.LogWarning("Report fail !");
            return new UserResponse<object>("Report fail !", null);
        }

        await _emailService.SendReportStudentMail(user.Email, reportCourseDto.Content);
        return new UserResponse<object>("Report successfully", null);
    }

    public async Task<UserResponse<object>> ApproveCourseByAdmin(int id)
    {
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            return new UserResponse<object>("This course is not available", null);
        }

        if (course.Status != "Submitted")
        {
            return new UserResponse<object>("Cannot approve this course", null);
        }

        
        // Backup course
        var checkBackUpCourse = await _backUpCourseRepository.GetBackUpCourseByCourseId(course.Id);
        bool checkBackUp;

        BackupCourse backupCourse = new BackupCourse()
        {
            Name = course.Name,
            ShortSummary = course.ShortSummary,
            Description = course.Description,
            Thumbnail = course.Thumbnail,
            Price = course.Price,
            Status = "Active",
            Version = course.Version,
            Point = course.Point,
            AllowComments = course.AllowComments,
            CourseId = course.Id
        };

        
        // Check course have chapters?
        var chapters = await _courseRepository.GetChaptersByCourseId(id);
        if (chapters == null || !chapters.Any())
        {
            _logger.LogWarning("The course has no chapters !");
            return new UserResponse<object>("The course has no chapters !", null);
        }
        
        // Backup Course
        if (checkBackUpCourse == null)
        {
            // CREATE
            checkBackUp = await _backUpCourseRepository.CreateBackUpCourse(backupCourse);
        }
        else
        {
            // EDIT
            // Update all fields
            checkBackUpCourse.Name = course.Name;
            checkBackUpCourse.ShortSummary = course.ShortSummary;
            checkBackUpCourse.Description = course.Description;
            checkBackUpCourse.Thumbnail = course.Thumbnail;
            checkBackUpCourse.Price = course.Price;
            checkBackUpCourse.Status = "Active";
            checkBackUpCourse.Version = course.Version;
            checkBackUpCourse.Point = course.Point;
            checkBackUpCourse.AllowComments = course.AllowComments;

            checkBackUp = await _backUpCourseRepository.EditBackUpCourse(checkBackUpCourse);
        }

        if (!checkBackUp)
        {
            _logger.LogWarning("Error occurred while backing up the course !");
            return new UserResponse<object>("Error occurred while backing up the course", null);
        }

        // Backup chapters
        chapters = await _courseRepository.GetChaptersByCourseId(id);
        if (chapters == null || !chapters.Any())
        {
            _logger.LogWarning("The course has no chapters");
            return new UserResponse<object>("The course has no chapters", null);
        }

        foreach (var chapter in chapters)
        {
            var checkBackUpChapter = await _backUpChapterRepository.GetBackUpChapterByChapterId(chapter.Id);
            BackupChapter backupChapter = new BackupChapter()
            {
                Content = chapter.Content,
                Thumbnail = chapter.Thumbnail,
                Order = chapter.Order,
                Duration = chapter.Duration,
                Type = chapter.Type,
                ChapterId = chapter.Id,
                BackupCourseId = backupCourse.Id
            };

            if (checkBackUpChapter == null)
            {
                // CREATE
                checkBackUp = await _backUpChapterRepository.CreateBackUpChapter(backupChapter);
            }
            else
            {
                // EDIT
                checkBackUpChapter.Content = chapter.Content;
                checkBackUpChapter.Thumbnail = chapter.Thumbnail;
                checkBackUpChapter.Order = chapter.Order;
                checkBackUpChapter.Duration = chapter.Duration;
                checkBackUpChapter.Type = chapter.Type;

                checkBackUp = await _backUpChapterRepository.EditBackUpChapter(checkBackUpChapter);
            }

            if (!checkBackUp)
            {
                _logger.LogWarning("Error occurred while backing up the chapters !");
                return new UserResponse<object>("Error occurred while backing up the chapters", null);
            }
        }

        //Save history course
        HistoryCourse historyCourse = new HistoryCourse()
        {
            CreatedDate = DateTime.Now,
            Description = "This course is approved",
            CourseId = id,
            UserId = 1,
        };
        bool checkHistory = await _historyCourseRepository.CreateHistoryCourse(historyCourse);
        if (!checkHistory)
        {
            _logger.LogWarning("Something wrong when save history course");
            return new UserResponse<object>("Something wrong when save history course", null);
        }
        
        
        course.Status = "Active";
        await _courseRepository.EditCourse(course);
        
        // Sending email to instructor
        var instructor = await _userRepository.GetAllUserById(course.InstructorId);
        await _emailService.SendApprovalCourseEmailAsync(instructor.Email);


        return new UserResponse<object>(
            "The course has been successfully approved and backed up. An email has been sent to the instructor.",
            null);
    }


    public async Task<UserResponse<object>> RejectCourseByAdmin(int id, RejectCourseRequest rejectCourseRequest)
    {
        var course = await _courseRepository.GetCourseById(id);

        if (course == null)
        {
            _logger.LogWarning("This course is not available !");
            return new UserResponse<object>("This course is not available !", null);
        }

        if (course.Status != "Submitted")
        {
            _logger.LogWarning("This course can be rejected !");
            return new UserResponse<object>("This course can be rejected !", null);
        }

        var instructor = await _userRepository.GetAllUserById(course.InstructorId);
        var backupCourse = await _backUpCourseRepository.GetBackUpCourseByCourseId(course.Id);
 

        //Save history course
        HistoryCourse historyCourse = new HistoryCourse()
        {
            CreatedDate = DateTime.Now,
            Description = $"This course is rejected by reason: {rejectCourseRequest.Reason}",
            CourseId = id,
            UserId = 1,
        };
        bool checkHistory = await _historyCourseRepository.CreateHistoryCourse(historyCourse);
        if (!checkHistory)
        {
            _logger.LogWarning("Something wrong when save history course !");
            return new UserResponse<object>("Something wrong when save history course !", null);
        }
        
        course.Status = "Rejected";
        await _courseRepository.EditCourse(course);

        await _emailService.SendRejectionCourseEmailAsync(instructor.Email, rejectCourseRequest.Reason);

        return new UserResponse<object>("This course is rejected success when create", id);
    }


    public async Task<UserResponse<object>> GetListEnrolledCourse(int PAGE_SIZE = 20, int page = 1)
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token == null)
            return new UserResponse<object>("Token not found", null);

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return new UserResponse<object>("Invalid token", null);
        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

        var enrolledCourse = await _courseRepository.ListEnrolledCourse(userId);
        
        var validPageSizes = new List<int> { 20, 50, 100 };

        if (!validPageSizes.Contains(PAGE_SIZE))
        {
            _logger.LogWarning($"Invalid PAGE_SIZE value. Valid options are: {string.Join(", ", validPageSizes)}");
            throw new ArgumentException($"Invalid PAGE_SIZE value. Valid options are: {string.Join(", ", validPageSizes)}");
        }
        var listEnrolledCourses = new List<EnrolledCourseDTO>();
        var result = PaginatedList<StudentInCourse>.Create(enrolledCourse, page, PAGE_SIZE);

        foreach (var c in result)
        {
            var courseId = c.CourseId;
            var getCourse = await _courseRepository.GetCourseById(courseId);
            List<ViewCategoryNameDTO> categoryName = await ViewCategoryName(getCourse.Id);
            var enrolledCourseDTO = new EnrolledCourseDTO()
            {
                CourseName = getCourse.Name,
                CourseImageThumb = getCourse.Thumbnail,
                CourseSumary = getCourse.ShortSummary,
                CategoryName = categoryName,
                Instructor = getCourse.InstructorId,
                Progress = c.Progress,
                Rating = getCourse.Point
            };
            listEnrolledCourses.Add(enrolledCourseDTO);
        }
        return new UserResponse<object>("Get list student enrolled course successfully", listEnrolledCourses);
    }

    public async Task<UserResponse<object>> ReviewCourseById(FeedbackCourseDTO feedbackCourseDto, int id)
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

        
        if (feedbackCourseDto.ReviewPoint == null)
        {
            return new UserResponse<object>("Review point is require !", null);
        }
        if (feedbackCourseDto.ReviewPoint > 5 || feedbackCourseDto.ReviewPoint < 0)
        {
            return new UserResponse<object>("Review point  must between 1 and 5 !", null);
        }
        
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning($"No course with id {id} is exist !");
            return new UserResponse<object>($"No course with id {id} is exist !", null);
        }

        var backupCourse = await _backUpCourseRepository.GetBackUpCourseByCourseId(id);
        if (backupCourse == null)
        {
            _logger.LogWarning($"No backupCourse with courseId {id} is exist !");
            return new UserResponse<object>($"No backupCourse with courseId {id} is exist !", null);
        }
        
        var isCheckStudentInCourse = await _courseRepository.GetStudentInCourseById(tokenId,id);
        if (isCheckStudentInCourse == null)
        {
            _logger.LogWarning("You just can review your enrolled course !");
            return new UserResponse<object>("You just can review your enrolled course !", null);
        }   
        var listFeedback = await _courseRepository.GetAllFeedback();
        foreach (var feedback in listFeedback)
        {
            if (feedback.CourseId == course.Id && feedback.UserId == tokenId)
            {
                _logger.LogWarning("You have already given feedback on this course !");
                return new UserResponse<object>("You have already given feedback on this course !", null);
            }
        }
        if (course.Status == "Pending")
        {
            _logger.LogWarning($"Course with id {id} is Pending !");
            return new UserResponse<object>($"Course with id {id} is Pending !", null);
        }

        if (feedbackCourseDto.Attachment != null)
        {
            var isCheckThumbnail = Path.GetExtension(feedbackCourseDto.Attachment.FileName).ToLower();
            if (isCheckThumbnail != ".png" && isCheckThumbnail != ".jpg" && isCheckThumbnail != ".jpeg")
            {
                _logger.LogWarning("Incorrect format thumbnail file !");
                return new UserResponse<object>("Incorrect format thumbnail file !", null);
            }
        }
        var attachment = feedbackCourseDto.Attachment != null 
            ? await _blobService.UploadFileAsync(feedbackCourseDto.Attachment) 
            : null;

        var studentFeedback = new Feedback()
        {
            Content = feedbackCourseDto.Content,
            Attachment = attachment,
            CourseId = course.Id,
            UserId = tokenId,
            ReviewPoint = feedbackCourseDto.ReviewPoint
        };

        var isCheckStudentFeedback = await _feedBackRepository.GetFeedBackByUserId(tokenId, id);
        if (isCheckStudentFeedback != null)
        {
            _logger.LogWarning("You are already reviewed this course !");
            return new UserResponse<object>("You are already reviewed this course !", null);
        }

        if (isCheckStudentInCourse.Progress != 100)
        {
            _logger.LogWarning("You need finished this course to review !");
            return new UserResponse<object>("You need finished this course to review !", null);
        }
        
        //Get StudentInCourse By UserId and CourseId
        var studentInCourse = await _courseRepository.GetStudentInCourseById(tokenId, course.Id);
        
        //Update Ratting in StudentInCourse
        studentInCourse.Rating = feedbackCourseDto.ReviewPoint;
        await _courseRepository.UpdateStudentInCourse(studentInCourse);
        
        var point = Math.Round((await _courseRepository.GetStudentInCourseByCourseId(id)), 1);
        
        //Update Point in Course
        course.Point = point;
        var updateCourse = await _courseRepository.EditCourse(course);
        
        //Update Point in BackupCourse
        backupCourse.Point = point;
        
        //Check create feedback
        var isCheckFeedback = await _courseRepository.CreateFeedbackCourse(studentFeedback);
        if (!isCheckFeedback)
        {
            return new UserResponse<object>("Feedback fail", null);
        }
        
        return new UserResponse<object>("Feedback successfully", null);
    }

    

    public async Task<UserResponse<object>> GetListBookmarkedCourse(int PAGE_SIZE = 20, int page = 1)
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token == null)
            return new UserResponse<object>("Token not found", null);

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return new UserResponse<object>("Invalid token", null);
        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

        var bookmarkedCourse = await _courseRepository.ViewListBookmarkedCourse(userId);

        if (bookmarkedCourse is null)
            return new UserResponse<object>("Bookmarked Courses is null", null);

        var validPageSizes = new List<int> { 20, 50, 100 };

        if (!validPageSizes.Contains(PAGE_SIZE))
        {
            throw new ArgumentException($"Invalid PAGE_SIZE value. Valid options are: {string.Join(", ", validPageSizes)}");
        }
        var listBookmarked = new List<BookMarkedCourseDTO>();
        var result = PaginatedList<BookmarkedCourse>.Create(bookmarkedCourse, page, PAGE_SIZE);

        foreach(var c in result)
        {
            var courseId = c.CourseId;
            var getCourse = await _courseRepository.GetCourseById(courseId);
            List<ViewCategoryNameDTO> categoryName = await ViewCategoryName(getCourse.Id);
            var bookMarkedCourseDTO = new BookMarkedCourseDTO()
            {
                CourseName = getCourse.Name,
                CourseImageThumb = getCourse.Thumbnail,
                CoursePrice = getCourse.Price,
                CourseSumary = getCourse.ShortSummary,
                CategoryName = categoryName,
                Instructor = getCourse.InstructorId
            };
            listBookmarked.Add(bookMarkedCourseDTO);
        }
        return new UserResponse<object>("Get list bookmark successfully", listBookmarked);
    }



    public async Task<UserResponse<object>> GetCoursesAsync(int instructorId, int pageSize, int pageIndex, string sortBy, bool sortDesc)
    {
        var courses = await _courseRepository.GetCoursesAsync(instructorId, pageSize, pageIndex, sortBy, sortDesc);

        var insId = await _courseRepository.GetInstructorByIdAsync(instructorId);

        if (insId == null)
        {
            return new UserResponse<object>("Instructor not found.", null);
        }

        if (courses == null || !courses.Any())
        {
            return new UserResponse<object>("No active courses found for the given instructor ID", null);
        }

        var courseDto = courses.Select(c => new CourseResponse
        {
            Name = c.Name,
            Description = c.Description,
            Thumbnail = c.Thumbnail,
            ShortSummary = c.ShortSummary,
            AllowComments = c.AllowComments,
            Price = c.Price
        }).ToList();

        return new UserResponse<object>("Success", courseDto);
    }

    public async Task<UserResponse<object>> GetCoursesByInstructorAsync(int pageSize, int pageIndex, string sortBy, bool sortDesc, CourseStatus? status)
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token == null)
            return new UserResponse<object>("Token not found", null);

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return new UserResponse<object>("Invalid token", null);

        var instructorId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

        IEnumerable<Course> courses;
        if (status is not null)
        {
            courses = await _courseRepository.GetCoursesByInstructorAsync(instructorId, pageSize, pageIndex, sortBy, sortDesc, status.Value);
        }
        else
        {
            courses = await _courseRepository.GetCoursesByInstructorAsync(instructorId, pageSize, pageIndex, sortBy, sortDesc);
        }

        if (courses == null || !courses.Any())
        {
            return new UserResponse<object>("No courses found for the instructor.", null);
        }
        
        var courseDto = courses.Select(c => new CourseResponse
        {
            Name = c.Name,
            Description = c.Description,
            Thumbnail = c.Thumbnail,
            ShortSummary = c.ShortSummary,
            AllowComments = c.AllowComments,
            Price = c.Price
        }).ToList();

        return new UserResponse<object>("Success", courseDto);
    }

    public async Task<UserResponse<object>> GetAllActiveCoursesAsync(int pageSize, int pageIndex)
    {
        var courses = await _courseRepository.GetActiveCoursesAsync(pageSize, pageIndex);

        if (courses == null || !courses.Any())
        {
            return new UserResponse<object>("No course found.", null);
        }

        var courseDto = new List<ViewAndSearchDTO>();

        foreach (var c in courses)
        {
            var instructor = await _courseRepository.GetInstructorByIdAsync(c.InstructorId);
            List<ViewCategoryNameDTO> categoryName = await ViewCategoryName(c.Id);
            var studentCount = await _courseRepository.CountStudentsInCourseAsync(c.Id);



            courseDto.Add(new ViewAndSearchDTO
            {
                Name = c.Name,
                Thumbnail = c.Thumbnail,
                InstructorName = instructor?.FullName,
                Price = c.Price,
                Point = c.Point,
                ShortSummary = c.ShortSummary,
                CategoryName = categoryName,
                Description = c.Description,
                StudentsInCourse = studentCount,
                CreatedDate = c.CreatedDate
            });
        }

        return new UserResponse<object>("Success", courseDto);
    }

    public async Task<UserResponse<object>> SearchCoursesAsync(Search? searchBy, string search, int pageSize, int pageIndex)
    {
        List<Course> courses;

        if (searchBy == null && string.IsNullOrEmpty(search))
        {
            courses = await _courseRepository.GetActiveCoursesAsync(pageSize, pageIndex);
        }
        else if (searchBy == null && !string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            courses = await _courseRepository.GetActiveCoursesAsync(pageSize, pageIndex);
            courses = courses.Where(c => c.Name.ToLower().Contains(search)).ToList();
        }
        else if (searchBy is not null && !string.IsNullOrEmpty(search))
        {
            search = search.ToLower();

            if ((int)searchBy == 0)
            {
                var categoryIds = await _courseRepository.GetCategoryIdsByNameAsync(search);
                courses = await _courseRepository.GetCoursesByCategoryIdsAsync(categoryIds, pageSize, pageIndex);
            }
            else if ((int)searchBy == 1)
            {
                courses = await _courseRepository.GetCoursesByInstructorNameAsync(search, pageSize, pageIndex);
            }
            else
            {
                courses = await _courseRepository.GetActiveCoursesAsync(pageSize, pageIndex);
                courses = courses.Where(c => c.Name.ToLower().Contains(search)).ToList();
            }
        }
        else
        {
            courses = await _courseRepository.GetActiveCoursesAsync(pageSize, pageIndex);
        }

        if (courses == null || !courses.Any())
        {
            return new UserResponse<object>("No courses found for the given criteria.", null);
        }

        var courseDto = new List<ViewAndSearchDTO>();

        foreach (var c in courses)
        {
            var instructor = await _courseRepository.GetInstructorByIdAsync(c.InstructorId);
            List<ViewCategoryNameDTO> categoryName = await ViewCategoryName(c.Id);
            var studentCount = await _courseRepository.CountStudentsInCourseAsync(c.Id);


            courseDto.Add(new ViewAndSearchDTO
            {
                Name = c.Name,
                Thumbnail = c.Thumbnail,
                InstructorName = instructor?.FullName,
                Price = c.Price,
                Point = c.Point,
                ShortSummary = c.ShortSummary,
                CategoryName = categoryName,
                Description = c.Description,
                StudentsInCourse = studentCount,
                CreatedDate = c.CreatedDate
            });
        }

        return new UserResponse<object>("Success", courseDto);
    }

    public async Task<UserResponse<object>> GetTopCoursesAsync()
    {
        const int count = 10;
        var courses = await _courseRepository.GetTopCoursesAsync(count);

        if (courses == null || !courses.Any())
        {
            return new UserResponse<object>("No courses found.", null);
        }

        var courseDto = new List<ViewAndSearchDTO>();

        foreach (var c in courses)
        {
            var instructor = await _courseRepository.GetInstructorByIdAsync(c.InstructorId);
            List<ViewCategoryNameDTO> categoryName = await ViewCategoryName(c.Id);
            var studentCount = await _courseRepository.CountStudentsInCourseAsync(c.Id);


            courseDto.Add(new ViewAndSearchDTO
            {
                Name = c.Name,
                Thumbnail = c.Thumbnail,
                InstructorName = instructor?.FullName,
                Price = c.Price,
                Point = c.Point,
                ShortSummary = c.ShortSummary,
                CategoryName = categoryName,
                Description = c.Description,
                StudentsInCourse = studentCount,
                CreatedDate = c.CreatedDate
            });
        }

        return new UserResponse<object>("Success", courseDto);
    }

    public async Task<UserResponse<object>> GetTopCategoriesAsync()
    {
        const int count = 3;
        var categories = await _courseRepository.GetTopCategoriesAsync(count);

        if (categories == null || !categories.Any())
        {
            return new UserResponse<object>("No categories found.", null);
        }

        var categoryDto = new List<CategoryResponse>();
        

        foreach (var c in categories)
        {
            var categoryName = await _categoryRepository.GetCategoryById(c.CategoryId);
            categoryDto.Add(new CategoryResponse
            {
                CategoryId = c.CategoryId,
                CategoryName = categoryName.CategoryName,
                Course = c.Course,
                RatingPoint = c.RatingPoint,
            });
        }
        return new UserResponse<object>("Success", categoryDto);
    }

    public async Task<UserResponse<object>> GetTopFeedbacksAsync()
    {
        const int count = 10;
        var feedbacks = await _courseRepository.GetTopFeedbacksAsync(count);
    
        if (feedbacks == null || !feedbacks.Any())
        {
            return new UserResponse<object>("No feedbacks found.", null);
        }
    
        var feedbackDto = feedbacks.Select(f => new FeedbackResponse
        {
            Content = f.Content,
            ReviewPoint = f.ReviewPoint,
            UserName = f.User.FullName,
            CourseName = f.Course.Name
        }).ToList();
    
        return new UserResponse<object>("Success", feedbackDto);
    }

    public async Task<HeaderDTO> GetHeaderAsync()
    {
        var header = await _courseRepository.GetHeaderAsync();
        if (header == null)
        {
            header = new Header();
        }
        
        return new HeaderDTO
        {
            BranchName = header.BranchName,
            SupportHotline = header.SupportHotline
        };  
    }

    public async Task<FooterDTO> GetFooterAsync()
    {
        var footer = await _courseRepository.GetFooterAsync();
        if (footer == null)
        {
            footer = new Footer();
        }

        return new FooterDTO
        {
            PhoneNumber = footer.PhoneNumber,
            Address = footer.Address,
            WorkingTime = footer.WorkingTime,
            Privacy = footer.Privacy,
            Team_of_use = footer.Term_of_use
        };
    }

    public async Task<UserResponse<HeaderDTO>> UpdateHeaderAsync(HeaderDTO headerDto)
    {
        var header = await _courseRepository.GetHeaderAsync();
        if (header == null)
        {
            header = new Header();
        }

        header.BranchName = headerDto.BranchName;
        header.SupportHotline = headerDto.SupportHotline;

        var updatedHeader = await _courseRepository.UpdateHeaderAsync(header);

        var responseDto = new HeaderDTO
        {
            BranchName = updatedHeader.BranchName,
            SupportHotline = updatedHeader.SupportHotline
        };

        return new UserResponse<HeaderDTO>("Success", responseDto);
    }

    public async Task<UserResponse<FooterDTO>> UpdateFooterAsync(FooterDTO footerDto)
    {
        var footer = await _courseRepository.GetFooterAsync();
        if (footer == null)
        {
            footer = new Footer();
        }

        footer.PhoneNumber = footerDto.PhoneNumber;
        footer.Address = footerDto.Address;
        footer.WorkingTime = footerDto.WorkingTime;
        footer.Privacy = footerDto.Privacy;
        footer.Term_of_use = footerDto.Team_of_use;

        var updatedFooter = await _courseRepository.UpdateFooterAsync(footer);

        var responseDto = new FooterDTO
        {
            PhoneNumber = updatedFooter.PhoneNumber,
            Address = updatedFooter.Address,
            WorkingTime = updatedFooter.WorkingTime,
            Privacy = updatedFooter.Privacy,
            Team_of_use = updatedFooter.Term_of_use
        };

        return new UserResponse<FooterDTO>("Success", responseDto);
    }

    public async Task<UserResponse<object>> GetTopPurchasedCoursesAsync(DateTime? startDate, DateTime? endDate)
    {
        {
            const int count = 5;
            var topCourse = await _courseRepository.GetTopPurchasedCoursesAsync(count);

            if (topCourse == null || !topCourse.Any())
            {
                return new UserResponse<object>( "No courses found.", null);
            }

            var courseDto = new List<ViewAndSearchDTO>();

            foreach (var courseId in topCourse)
            {
                var course = await _courseRepository.GetCourseById(courseId);

                if (startDate.HasValue && endDate.HasValue)
                {
                    if (course.CreatedDate < startDate || course.CreatedDate > endDate)
                    {
                        continue;
                    }
                }
                else if (startDate.HasValue)
                {
                    if (course.CreatedDate < startDate)
                    {
                        continue;
                    }
                }
                else if (endDate.HasValue)
                {
                    if (course.CreatedDate > endDate)
                    {
                        continue;
                    }
                }

                var instructor = await _courseRepository.GetInstructorByIdAsync(course.InstructorId);
                List<ViewCategoryNameDTO> categoryName = await ViewCategoryName(course.Id);
                var studentCount = await _courseRepository.CountStudentsInCourseAsync(courseId);

                courseDto.Add(new ViewAndSearchDTO
                {
                    Name = course.Name,
                    CreatedDate = course.CreatedDate,
                    Thumbnail = course.Thumbnail,
                    InstructorName = instructor?.FullName,
                    Point = course.Point,
                    Price = course.Price,
                    StudentsInCourse = studentCount,
                    ShortSummary = course.ShortSummary,
                    CategoryName = categoryName,
                    Description = course.Description
                });
            }

            return new UserResponse<object>( "Success", courseDto);
        }
    }

    public async Task<UserResponse<object>> GetTopBadCoursesAsync(DateTime? startDate, DateTime? endDate)
    {
        const int count = 5;
        var courses = await _courseRepository.GetTopBadCoursesAsync(count);

        if (courses == null || !courses.Any())
        {
            return new UserResponse<object>( "No courses found.", null);
        }

        var courseDto = new List<ViewAndSearchDTO>();

        foreach (var c in courses)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                if (c.CreatedDate < startDate || c.CreatedDate > endDate)
                {
                    continue;
                }
            }
            else if (startDate.HasValue)
            {
                if (c.CreatedDate < startDate)
                {
                    continue;
                }
            }
            else if (endDate.HasValue)
            {
                if (c.CreatedDate > endDate)
                {
                    continue;
                }
            }

            var instructor = await _courseRepository.GetInstructorByIdAsync(c.InstructorId);
            List<ViewCategoryNameDTO> categoryName = await ViewCategoryName(c.Id);

            courseDto.Add(new ViewAndSearchDTO
            {
                Name = c.Name,
                CreatedDate = c.CreatedDate,
                Thumbnail = c.Thumbnail,
                InstructorName = instructor?.FullName,
                Price = c.Price,
                Point = c.Point,
                ShortSummary = c.ShortSummary,
                CategoryName = categoryName,
                Description = c.Description
            });
        }

        return new UserResponse<object>( "Success", courseDto);
    }

    public async Task<UserResponse<object>> GetTopInstructorPayoutsAsync(DateTime? startDate, DateTime? endDate)
    {
        const int count = 5;
        var payouts = await _courseRepository.GetTopInstructorPayoutsAsync(count);

        if (payouts == null || !payouts.Any())
        {
            return new UserResponse<object>("No payouts found.", null);
        }

        var payoutDto = new List<InstructorPayoutDTO>();

        foreach (var p in payouts)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                if (p.PayoutDate < startDate || p.PayoutDate > endDate)
                    continue;
            }
            else if (startDate.HasValue)
            {
                if (p.PayoutDate < startDate)
                    continue;
            }
            else if (endDate.HasValue)
            {
                if (p.PayoutDate > endDate)
                    continue;
            }

            var instructor = await _courseRepository.GetInstructorByIdAsync(p.InstructorId);

            payoutDto.Add(new InstructorPayoutDTO
            {
                InstructorId = p.InstructorId,
                InstructorName = instructor.FullName,
                PayoutAmount = p.PayoutAmount,
                PayoutDate = p.PayoutDate
            });
        }

        return new UserResponse<object>( "Success", payoutDto);
    }

    public async Task<List<ViewCourseDTO>> ViewCourseName(int instructorId)
    {
        var courses = await _courseRepository.GetCoursesByInstructorIdAsync(instructorId);
        List<ViewCourseDTO> list = new List<ViewCourseDTO>();

        foreach (var course in courses)
        {
            ViewCourseDTO courseDTO = new ViewCourseDTO()
            {
                Id = course.Id,
                Name = course.Name
            };
            list.Add(courseDTO);
        }

        return list;
    }
}