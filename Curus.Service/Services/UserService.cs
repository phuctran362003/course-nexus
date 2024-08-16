using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using Curus.Repository.Entities;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Curus.Repository.Helper;
using Curus.Repository.ViewModels.Response;
using Microsoft.Extensions.Configuration;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.User.Student;
using Curus.Service.ResponseDTO;
using SuggestCourse = Curus.Repository.ViewModels.SuggestCourse;
using Microsoft.Extensions.Logging;

namespace Curus.Service.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IAuthRepository _authRepository;
    private readonly IEmailService _emailService;
    private readonly IBlobService _blobService;
    private readonly TokenGenerators _tokenGenerators;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRedisService _redisService;
    private readonly IInstructorRepository _instructorRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IStudentInCourseRepository _studentInCourseRepository;

    public UserService(
        IUserRepository userRepository,
        IEmailService emailService,
        IBlobService blobService,
        IConfiguration configuration,
        IAuthRepository authRepository,
        TokenGenerators tokenGenerators,
        IHttpContextAccessor httpContextAccessor,
        IRedisService redisService,
        IInstructorRepository instructorRepository,
        ICourseRepository courseRepository,
        ICategoryRepository categoryRepository,
        ILogger<UserService> logger,
        IStudentInCourseRepository studentInCourseRepository)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _blobService = blobService;
        _configuration = configuration;
        _authRepository = authRepository;
        _tokenGenerators = tokenGenerators;
        _httpContextAccessor = httpContextAccessor;
        _redisService = redisService;
        _instructorRepository = instructorRepository;
        _courseRepository = courseRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
        _studentInCourseRepository = studentInCourseRepository;
    }

    //login
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<bool> DeleteRefreshToken(int userId)
    {
        return await _authRepository.DeleteRefreshToken(userId);
    }

    public async Task<User> GetByEmail(string email)
    {
        return await _userRepository.GetUserByEmail(email);
    }
    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
    }

    public async Task<UserResponse<object>> GetUserById(int id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            throw new Exception("User is not exist!");
        }

        if (user.RoleId != 2)
        {
            return new UserResponse<object>("This user is not a student !", null);
        }

        StudentDTO userDto = new()
        {
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            JoinedCourse = 2,
            NumberInProgressCourse = 1
        };

        return new UserResponse<object>("Student detail", userDto);
    }


    


   

    

    private string GenerateVerificationToken()
    {
        using (var cryptoProvider = new RNGCryptoServiceProvider())
        {
            byte[] tokenBuffer = new byte[32];
            cryptoProvider.GetBytes(tokenBuffer);
            return Convert.ToBase64String(tokenBuffer); 
        }
    }

   

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }




   
    private string GenerateResetToken()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var byteArray = new byte[32];
            rng.GetBytes(byteArray);
            return Convert.ToBase64String(byteArray);
        }
    }

    public async Task<(bool, string)> CreateCommentByAdmin(CommentDTO commentDto, int id)
    {
        //take id of admin by access token
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();
        if (token == null)
            throw new Exception("Token not found");
        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        if (jwtToken == null)
            throw new Exception("Invalid token");
        var tokenId = jwtToken.Claims.First(claim => claim.Type == "id").Value;

        //get user by id and add to commentUser table
        var user = await _userRepository.GetUserById(id);

        //check if userId is in instructor role
        if (user.RoleId != 2)
        {
            return (false, $"No user with id {id} in instructor role");
        }

        if (!user.Status.Equals("Active"))
        {
            return (false, $"User with id {id} does not active");
        }

        var comment = new CommentUser()
        {
            Content = commentDto.content,
            UserId = user.UserId,
            CommentedById = int.Parse(tokenId)
        };
        await _userRepository.CreateCommentUserAsync(comment);
        return (true, "Add comment success");
    }

    




public async Task<IQueryable<StudentInfoDto>> GetInfoStudent(int PAGE_SIZE = 10, int page = 1)
    {
        var validPageSizes = new List<int> { 20, 50, 100 };

        if (!validPageSizes.Contains(PAGE_SIZE))
        {
            throw new ArgumentException($"Invalid PAGE_SIZE value. Valid options are: {string.Join(", ", validPageSizes)}");
        }

        List<User> studentList = await _userRepository.GetInfoStudent(PAGE_SIZE, page);
        var listStudentInCourse = await _userRepository.GetListStudentInCourse();
        var list = new List<StudentInfoDto>();
        var result = PaginatedList<User>.Create(studentList, page, PAGE_SIZE);
        foreach (var u in result)
        {
            var userId = u.UserId;
            var amountCourse = await _userRepository.CountAmountStudentCourse(userId);
            var studentInfoDto = new StudentInfoDto()
            {
                UserId = u.UserId,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                NumberOfJoinedCourses = amountCourse,
                NumberOfCourseInProgress = 0,
                Courses = listStudentInCourse.Where(sic => sic.UserId == u.UserId)
                                         .SelectMany(sic => sic.Courses)
                                         .Select(course => new CourseViewModel()
                                         {
                                             CourseId = course.Id,
                                             InstructorId = course.InstructorId,
                                             CourseName = course.Name,
                                             Price = course.Price,
                                             Description = course.Description,
                                             Status = course.Status,
                                             Version = course.Version,
                                         }).ToList()
            };
            list.Add(studentInfoDto);
        }
        var orderedResult = list.OrderBy(u => u.UserId).AsQueryable();
        return orderedResult;
    }

    public async Task<GeneralServiceResponseDto> ActivateStudent(int userId)
    {
        var checkStudent = await _userRepository.GetStudentById(userId);
        if (checkStudent == null)
        {
            return new GeneralServiceResponseDto()
            {
                IsSucceed = false,
                StatusCode = 404,
                Message = "Student not found"
            };
        }
        else
        {
            if (checkStudent.RoleId != 2)
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "The user is not a student"
                };
            else
            {
                if (checkStudent.Status == UserStatus.Active)
                {
                    return new GeneralServiceResponseDto()
                    {
                        IsSucceed = false,
                        StatusCode = 400,
                        Message = "The Student Already Activate"
                    };
                }
                else
                {
                    checkStudent.Status = UserStatus.Active;
                    await _userRepository.ActiveStudent(checkStudent);
                    return new GeneralServiceResponseDto()
                    {
                        IsSucceed = true,
                        StatusCode = 200,
                        Message = "Student status has been changed to active"
                    };
                }
            }
        }
    }

    public async Task<string> UpdateStatusUser(ContentEmailDTO contentEmailDTO, int userId)
    {
        var getStudent = await _userRepository.GetStudentById(userId);
        if (getStudent is null)
        {
            _logger.LogWarning("User not found");
            return "User not found";
        }
        if (getStudent.RoleId != 2)
        {
            _logger.LogWarning("User is not a student");
            return "User is not a student";
        }
        if (getStudent.Status == UserStatus.Pending)
        {
            _logger.LogWarning("This user is in pending status");
            return "This user is in pending status";
        }
        if (getStudent.Status == UserStatus.Active)
        {
            await _emailService.SendDeactiveEmailAsync(getStudent.Email, contentEmailDTO.content);
            getStudent.Status = UserStatus.Inactive;    
        }
        else if (getStudent.Status == UserStatus.Inactive)
        {
            await _emailService.SendActiveEmailAsync(getStudent.Email);
            getStudent.Status = UserStatus.Active;
        }
        else
        {
            _logger.LogWarning("Invalid status change request");
            return "Invalid status change request";
        }
        await _instructorRepository.UpdateUserAsync(getStudent);
        return "Status updated successfully";
    }
    
    public async Task<UserResponse<object>> ToggleBookmarkCourse(int courseId)
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token == null)
            return new UserResponse<object>( "Token not found", null);

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return new UserResponse<object>("Invalid token", null);
        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

        var getCourse = await _courseRepository.GetCourseById(courseId);

        if (getCourse == null)
            return new UserResponse<object>("Course is not exist", null);

        var checkBookmarkedCourse = await _userRepository.GetBookmarkedCourse(userId, courseId);

        if (checkBookmarkedCourse is not null)
        {
            await _userRepository.UnBookmarkedCourse(courseId, userId);
            return new UserResponse<object>("Un-Bookmarked Course Successfully", null);
        }
        else
        {
            var bookmark = new BookmarkedCourse()
            {
                UserId = userId,
                CourseId = courseId,
            };
            await _userRepository.AddBookmarkedCourse(bookmark);
            return new UserResponse<object>( "Add Bookmarked Course Successfully", null);
        }
    }

    public async Task<UserResponse<object>> GetStudentDashboard(StudentDashboard studentDashboard)
    {
         var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();

        if (token == null)
            throw new Exception("Token not found");

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new Exception("Invalid token");

        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);
        
        DateTime filter = DateTime.Now.AddMonths(-(int)studentDashboard);
        
        var courses = await _studentInCourseRepository.GetStudentDashboardCourse(userId,filter);
        var suggestCourse = await _courseRepository.GetCourseSuggest();

        var learnedCourseIds = courses.Select(c => c.CourseId).ToList();

        var notLearnedCourses = suggestCourse
            .Where(c => !learnedCourseIds.Contains(c.Id))
            .Take(5)
            .ToList();
        
        List<SuggestCourse> suggestCourses = new List<SuggestCourse>();
        
        foreach (var notLearnedCourse in notLearnedCourses)
        {
            SuggestCourse suggest = new SuggestCourse()
            {
                CourseId = notLearnedCourse.Id,
                CourseName = notLearnedCourse.Name,
                InstructorId = notLearnedCourse.InstructorId,
                Price = notLearnedCourse.Price,
                Rating = notLearnedCourse.Point,
            };
            suggestCourses.Add(suggest);
        }

        GeneralInformation generalInformation = new GeneralInformation()
        {
            TotalPaidCourse = courses.Count,
            InprogressCourse = courses.Where(s => s.Progress < 100 & s.Progress > 0).Count(),
            DoneCourse = courses.Where(s => s.Progress == 100).Count()
        };

        StudentDashboardDTO studentDashboardDto = new StudentDashboardDTO()
        {
            UserId = userId,
            GeneralInformation = generalInformation,
            SuggestCourses = suggestCourses
        };
        
        return new UserResponse<object>("DashboardStudent", studentDashboardDto);

    }

    public async Task SendRemindersAsync()
    {
        var sinceDate = DateTime.UtcNow.AddDays(-76); // 90 - 14 days
        var inactiveStudents = await _userRepository.GetInactiveStudentsAsync(sinceDate);
        var inactiveInstructors = await _userRepository.GetInactiveInstructorsAsync(sinceDate);

        string remindMessage = "In next 14 days, if you don't access, Cursus will delete your account";
        
        var usersToRemind = inactiveStudents.Concat(inactiveInstructors).ToList();
        foreach (var user in usersToRemind)
        {
            await _emailService.SendRemindActiveEmail(user.Email, remindMessage);
        }

    }

    public async Task DeleteInactiveUsersAsync()
    {
        var sinceDate = DateTime.UtcNow.AddDays(-90);
        var inactiveStudents = await _userRepository.GetInactiveStudentsAsync(sinceDate);
        var inactiveInstructors = await _userRepository.GetInactiveInstructorsAsync(sinceDate);

        var usersToDelete = inactiveStudents.Concat(inactiveInstructors).ToList();
        
        if (usersToDelete.Any())
        {
            foreach (var user in usersToDelete)
            {
                user.IsDelete = true;
            }

            await _userRepository.RemoveUsersAsync(usersToDelete);
        }

    }
    public async Task<UserResponse<object>> UpdateUser(UpdateUserDTO updateUserDto)
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token == null)
            return new UserResponse<object>("Token not found", null);

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return new UserResponse<object>("Invalid token", null);

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "id");
        if (userIdClaim == null)
            return new UserResponse<object>("User ID not found in token", null);

        var userId = int.Parse(userIdClaim.Value);

        var getUser = await _userRepository.GetAllUserById(userId);

        if (getUser == null)
            return new UserResponse<object>("User not found", null);

        getUser.FullName = updateUserDto.FullName;
        getUser.Address = updateUserDto.Address;
        getUser.Birthday = updateUserDto.BirthDay;

        await _userRepository.UpdateAsync(getUser);
        return new UserResponse<object>("Update User Successfully", null);
    }

    public async Task<UserResponse<object>> FinishCourse(int courseId)
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token == null)
            return new UserResponse<object>("Token not found", null);

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return new UserResponse<object>("Invalid token", null);
        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

        var getUser = await _userRepository.GetStudentById(userId);
        var getCourse = await _courseRepository.GetCourseById(courseId);
        var getMyCourse = await _studentInCourseRepository.GetCourseByUser(courseId, userId);

        if (getCourse is null)
            return new UserResponse<object>("Course is not exist", null);

        if (getMyCourse is null)
            return new UserResponse<object>("You have not taken the course yet", null);

        if (getMyCourse.IsFinish is true)
            return new UserResponse<object>("Course already finished", null);

        if (getMyCourse.Progress < 100)
            return new UserResponse<object>("You must completed your course before finish!!!", null);

        await _emailService.SendFinishCourse(getUser.Email, "Your course has been finished");
        await _studentInCourseRepository.FinishCourse(courseId, userId);
        return new UserResponse<object>("Finished the course successfully", null);
    }
}
