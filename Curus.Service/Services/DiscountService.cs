using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DiscountStatus = Curus.Repository.ViewModels.DiscountStatus;

namespace Curus.Service.Services;

public class DiscountService : IDiscountService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DiscountService> _logger;
    private static Random _random = new Random();
    private readonly IEmailService _emailService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICourseRepository _courseRepository;
    private readonly IBackUpCourseRepository _backUpCourseRepository;
    


    public DiscountService(IDiscountRepository discountRepository, IUserRepository userRepository,
        ILogger<DiscountService> logger, IEmailService emailService, IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository, IBackUpCourseRepository backUpCourseRepository)
    {
        _discountRepository = discountRepository;
        _userRepository = userRepository;
        _logger = logger;
        _emailService = emailService;
        _httpContextAccessor = httpContextAccessor;
        _courseRepository = courseRepository;
        _backUpCourseRepository = backUpCourseRepository;
    }

    public async Task<UserResponse<object>> CreateDiscount(CreateDiscountDTO createDiscountDto)
    {


        if (createDiscountDto.DiscountPercentage > 100 || createDiscountDto.DiscountPercentage < 0)
        {
            return new UserResponse<object>("Discount percent must in 1 to 100 range !", null);
        }
        
        //Generate random code
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        StringBuilder discountCode = new StringBuilder(5);
        int[] positions = { 0, 1, 2, 3, 4 };
        positions = positions.OrderBy(x => _random.Next()).ToArray();
        char[] result = new char[5];
        for (int i = 0; i < 3; i++)
        {
            result[positions[i]] = letters[_random.Next(letters.Length)];
        }

        for (int i = 3; i < 5; i++)
        {
            result[positions[i]] = digits[_random.Next(digits.Length)];
        }

        discountCode.Append(result);

        //Create new Discount
        var discount = new Discount()
        {
            DiscountCode = discountCode.ToString(),
            DiscountPercentage = createDiscountDto.DiscountPercentage,
            DiscountStatus = DiscountStatus.New,
            isAvalaible = true,
        };

        var isCheckCreateDiscount = await _discountRepository.CreateDiscount(discount);
        if (!isCheckCreateDiscount)
        {
            return new UserResponse<object>("Fail to create discount!", null);
        }

        return new UserResponse<object>("Create discount successfully", null);
    }

    public async Task<UserResponse<object>> SendDiscount(SendDiscountDTO sendDiscountDto, int id)
    {
        var discount = await _discountRepository.GetDiscountById(id);
        if (discount == null)
        {
            return new UserResponse<object>($"Discount with id {id} is not exist", null);
        }
        
        if (!discount.isAvalaible)
        {
            return new UserResponse<object>("This discount has been send to instructor !", null);
        }

        //Get list all instructor
        var listInstructor = await _userRepository.GetUsersByRole(3);
        if (!listInstructor.Any())
        {
            return new UserResponse<object>("Do not have any instructor !", null);
        }

        if (sendDiscountDto.ExpireAmount == null)
        {
            return new UserResponse<object>("Please enter the expire day !", null);
        }

        if (sendDiscountDto.TimeStyle == null)
        {
            return new UserResponse<object>("You must choice time style !", null);
        }
        
        if (sendDiscountDto.TimeStyle == TimeDiscountType.Minute)
        {
            if (sendDiscountDto.ExpireAmount > 60 || sendDiscountDto.ExpireAmount < 0)
            {
                return new UserResponse<object>("Minute expire must in 1 to 60 range !", null);
            }
            BackgroundJob.Schedule(() => ExpireDate(id),
                TimeSpan.FromMinutes(sendDiscountDto.ExpireAmount));
            
            //Update expire time for discount
            discount.ExpireDateTime = DateTime.UtcNow.AddMinutes(sendDiscountDto.ExpireAmount);
            await _discountRepository.UpdateDiscount(discount);
        }
        else if (sendDiscountDto.TimeStyle == TimeDiscountType.Hour)
        {
            if (sendDiscountDto.ExpireAmount > 24 || sendDiscountDto.ExpireAmount < 0)
            {
                return new UserResponse<object>("Hour expire must in 1 to 24 range !", null);
            }
            BackgroundJob.Schedule(() => ExpireDate(id),
                TimeSpan.FromHours(sendDiscountDto.ExpireAmount));
            
            //Update expire time for discount
            discount.ExpireDateTime = DateTime.UtcNow.AddHours(sendDiscountDto.ExpireAmount);
            await _discountRepository.UpdateDiscount(discount);
        }
        else if (sendDiscountDto.TimeStyle == TimeDiscountType.Day)
        {
            if (sendDiscountDto.ExpireAmount > 365 || sendDiscountDto.ExpireAmount < 0)
            {
                return new UserResponse<object>("Day expire must in 1 to 365 range !", null);
            }
            BackgroundJob.Schedule(() => ExpireDate(id),
                TimeSpan.FromDays(sendDiscountDto.ExpireAmount));
            
            //Update expire time for discount
            discount.ExpireDateTime = DateTime.UtcNow.AddDays(sendDiscountDto.ExpireAmount);
            await _discountRepository.UpdateDiscount(discount);
        }

        //Create info email for instructor
        var emailContent = new EmailDiscountDTO()
        {
            DiscountCode = discount.DiscountCode,
            ExpireDateTime = discount.ExpireDateTime,
            DiscountPercentage = discount.DiscountPercentage
        };

        //Send mail with discount code to all instructors  
        var emailTasks =
            listInstructor.Select(instructor => _emailService.SendDiscountEmail(instructor.Email, emailContent));
        await Task.WhenAll(emailTasks);

        return new UserResponse<object>("Send discount code successfully", null);
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ExpireDate(int id)
    {
        var discount = await _discountRepository.GetDiscountById(id);
        discount.isAvalaible = false;
        await _discountRepository.UpdateDiscount(discount);

        //Update lại price cho backupCourse khi hết expire time
        var listCourse = await _discountRepository.GetListCourseByDiscountId(id);
        foreach (var courseId in listCourse)
        {
            var backupCourse = await _backUpCourseRepository.GetBackUpCourseByCourseId(courseId);
            backupCourse.Price = (decimal)backupCourse.OldPrice;
            await _backUpCourseRepository.EditBackUpCourse(backupCourse);
        }
        
    }
    
     public async Task<UserResponse<object>> UseDiscountForCourse(int id, DiscountCourseDTO discountCourseDto)
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

        var user = await _userRepository.GetAllUserById(tokenId);
        if (user.Status != UserStatus.Active)
        {
            return new UserResponse<object>("You are not approved yet !", null);
        }

        var backupCourse = await _backUpCourseRepository.GetBackUpCourseByCourseId(id);
        if (backupCourse == null)
        {
            return new UserResponse<object>("Backup Course with course id {id} is not exits !", null);
        }
        
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning("This course is not exist !");
            return new UserResponse<object>("This course is not exist !", null);
        }

        if (course.InstructorId != tokenId)
        {
            return new UserResponse<object>("You just can use discount for your course !", null);
        }
        if (course.Status != "Active")
        {
            _logger.LogWarning("This course is not active !");
            return new UserResponse<object>("This course is not active !", null);
        }

        var discount = await _discountRepository.GetDiscountByCode(discountCourseDto.DiscountCode);
        
        if (discount == null)
        {
            return new UserResponse<object>("Do not have any discount exist !", null);
        }
        if (!discount.isAvalaible)
        {
            return new UserResponse<object>("This discount is not available !", null);
        }

        var isCheckHistoryDiscount = await _discountRepository.GetHistoryCourseDiscount(discount.Id, tokenId);
        if (isCheckHistoryDiscount != null)
        {
            return new UserResponse<object>("This code have been used !", null);
        }
        
        if (course.InstructorId != tokenId)
        {
            _logger.LogWarning("You just can discount your course !");
            return new UserResponse<object>("You just can discount your course !", null);
        }

        var point = (decimal)(course.OldPrice * (100 - discount.DiscountPercentage) / 100);
        
        //Update backup course
        backupCourse.Price = point;
        var isCheckUpdateBackupCourse = await _backUpCourseRepository.EditBackUpCourse(backupCourse);
        
        //Create History discount
        var historyDiscount = new HistoryCourseDiscount()
        {
            DiscountPercentage = discount.DiscountPercentage,
            CourseId = id,
            InstructorId = tokenId,
            DiscountId = discount.Id
        };
        await _discountRepository.CreateHistoryDiscount(historyDiscount);
        
        if (!isCheckUpdateBackupCourse)
        {
            return new UserResponse<object>("Fail to discount course", null);
        }
        return new UserResponse<object>("Course discount successfully", null);
    }
}