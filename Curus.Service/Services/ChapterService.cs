using System.IdentityModel.Tokens.Jwt;
using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Curus.Service.Services;

public class ChapterService : IChapterService
{
    private readonly IChapterRepository _chapterRepository;
    private readonly IBlobService _blobService;
    private readonly ICourseRepository _courseRepository;
    private readonly IVideoService _videoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ChapterService> _logger;

    public ChapterService(IChapterRepository chapterRepository, IBlobService blobService,
        ICourseRepository courseRepository, IVideoService videoService, IHttpContextAccessor httpContextAccessor, ILogger<ChapterService> logger)
    {
        _chapterRepository = chapterRepository;
        _blobService = blobService;
        _courseRepository = courseRepository;
        _videoService = videoService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<UserResponse<object>> CreateChapter(ChapterDTO chapterDto)
    {
        var course = await _courseRepository.GetCourseById(chapterDto.CourseId);
        if (chapterDto.Content == null)
        {
            _logger.LogWarning("No file uploaded !");
            return new UserResponse<object>("No file uploaded !", null);
        }

        if (chapterDto.Thumbnail != null)
        {
            var isCheckThumbnail = Path.GetExtension(chapterDto.Thumbnail.FileName).ToLower();
            if (isCheckThumbnail != ".png" && isCheckThumbnail != ".jpg" && isCheckThumbnail != ".jpeg")
            {
                _logger.LogWarning("Incorrect format thumbnail file!");
                return new UserResponse<object>("Incorrect format thumbnail file!", null);
            }
        }

        var thumbnail = chapterDto.Thumbnail != null
            ? await _blobService.UploadFileAsync(chapterDto.Thumbnail)
            : course.Thumbnail;

        var isFileCheck = Path.GetExtension(chapterDto.Content.FileName).ToLower();
        if (chapterDto.Type == ChapterType.DocFile)
        {
            if (isFileCheck != ".txt" && isFileCheck != ".docx" && isFileCheck != ".doc")
            {
                _logger.LogWarning("Invalid file type for document!");
                return new UserResponse<object>("Invalid file type for document!", null);
            }
        }
        else if (chapterDto.Type == ChapterType.VideoFile)
        {
            if (isFileCheck != ".mp4" && isFileCheck != ".avi" && isFileCheck != ".mov")
            {
                _logger.LogWarning("Invalid file type for video!");
                return new UserResponse<object>("Invalid file type for video!", null);
            }
        }
        else
        {
            _logger.LogWarning("Invalid chapter type!");
            return new UserResponse<object>("Invalid chapter type!", null);
        }

        if (course == null)
        {
            _logger.LogWarning("This course is not exist !");
            return new UserResponse<object>("This course is not exist !", null);
        }

        if (course.Status == "Submitted")
        {
            _logger.LogWarning("This course was submitted !");
            return new UserResponse<object>("This course was submitted !", null);
        }


        TimeSpan duration = TimeSpan.Zero;
        if (chapterDto.Type == ChapterType.VideoFile)
        {
            duration = await _videoService.GetVideoDuration(chapterDto.Content);
        }

        var chapter = new Chapter()
        {
            CourseId = chapterDto.CourseId,
            Content = await _blobService.UploadFileAsync(chapterDto.Content),
            Thumbnail = thumbnail,
            Order = chapterDto.Order,
            Duration = duration,
            Type = chapterDto.Type,
        };

        var isCheckCreateChapter = await _chapterRepository.CreateChapter(chapter);
        if (isCheckCreateChapter)
        {
            return new UserResponse<object>("Chapter created successfully", new { ChapterId = chapter.Id });
        }

        return new UserResponse<object>("Fail to create new chapter !", null);
    }

    public async Task<UserResponse<object>> UpdateChapter(int id, UpdateChapterDTO updateChapterDto)
    {
        var chapter = await _chapterRepository.GetChapterById(id);

        if (chapter == null)
        {
            _logger.LogWarning("Chapter is not exist !");
            return new UserResponse<object>("Chapter is not exist !", null);
        }

        var isCheckThumbnail = Path.GetExtension(updateChapterDto.Thumbnail.FileName).ToLower();
        if (isCheckThumbnail != ".png" && isCheckThumbnail != ".jpg" && isCheckThumbnail != ".jpeg")
        {
            _logger.LogWarning("Incorrect format thumbnail file !");
            return new UserResponse<object>("Incorrect format thumbnail file !", null);
        }

        var isFileCheck = Path.GetExtension(updateChapterDto.Content.FileName).ToLower();
        if (updateChapterDto.Type == ChapterType.DocFile)
        {
            if (isFileCheck != ".txt" && isFileCheck != ".docx" && isFileCheck != ".doc")
            {
                _logger.LogWarning("Invalid file type for document!");
                return new UserResponse<object>("Invalid file type for document!", null);
            }
        }
        else if (updateChapterDto.Type == ChapterType.VideoFile)
        {
            if (isFileCheck != ".mp4" && isFileCheck != ".avi" && isFileCheck != ".mov")
            {
                _logger.LogWarning("Invalid file type for video!");
                return new UserResponse<object>("Invalid file type for video!", null);
            }
        }
        else
        {
            _logger.LogWarning("Invalid chapter type!");
            return new UserResponse<object>("Invalid chapter type!", null);
        }

        TimeSpan duration = TimeSpan.Zero;
        if (updateChapterDto.Type == ChapterType.VideoFile)
        {
            duration = await _videoService.GetVideoDuration(updateChapterDto.Content);
            chapter.Duration = duration;
        }

        if (updateChapterDto.Content != null)
        {
            if (updateChapterDto.Type == null)
            {
                _logger.LogWarning("Must edit type to edit content !");
                return new UserResponse<object>("Must edit type to edit content !", null);
            }

            chapter.Content = await _blobService.UploadFileAsync(updateChapterDto.Content);
            chapter.Type = updateChapterDto.Type;
        }

        if (updateChapterDto.Thumbnail != null)
        {
            chapter.Thumbnail = await _blobService.UploadFileAsync(updateChapterDto.Thumbnail);
        }

        var course = await _courseRepository.GetCourseById(chapter.CourseId);
        course.Status = "Pending";
        await _courseRepository.EditCourse(course);

        var isCheckUpdateChapter = await _chapterRepository.UpdateChapter(chapter);

        if (isCheckUpdateChapter)
        {
            return new UserResponse<object>("Update chapter successfully", null);
        }

        return new UserResponse<object>("Fail to update chapter", null);
    }

    public async Task<UserResponse<object>> UpdateOrderChapter(int id, UpdateOrderChapterDTO updateOrderChapterDto)
    {
        var chapter = await _chapterRepository.GetChapterById(id);

        if (chapter == null)
        {
            _logger.LogWarning("Chapter is not exist !");
            return new UserResponse<object>("Chapter is not exist !", null);
        }

        chapter.Order = updateOrderChapterDto.Order;


        var course = await _courseRepository.GetCourseById(chapter.CourseId);
        course.Status = "Pending";
        await _courseRepository.EditCourse(course);

        var isCheckUpdateChapter = await _chapterRepository.UpdateChapter(chapter);

        if (isCheckUpdateChapter)
        {
            return new UserResponse<object>("Update chapter successfully", null);
        }

        return new UserResponse<object>("Fail to update chapter", null);
    }

    public async Task<UserResponse<object>> DeleteChapterById(int id)
    {
        var chapter = await _chapterRepository.GetChapterById(id);

        if (chapter == null)
        {
            _logger.LogWarning("Chapter is not exist !");
            return new UserResponse<object>("Chapter is not exist !", null);
        }

        var isCheckDelete = await _chapterRepository.DeleteChapterById(chapter);

        if (isCheckDelete)
        {
            return new UserResponse<object>("Delete chapter successfully", null);
        }

        return new UserResponse<object>("Fail to delete chapter !", null);
    }

    public async Task<UserResponse<object>> StartChapterById(int id)
    {
        //Get UserId form AccessToken
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();

        if (token == null)
            throw new Exception("Token not found");

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new Exception("Invalid token");

        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

        //Get chapter by chapterId
        var chapter = await _chapterRepository.GetChapterById(id);
        if (chapter == null)
        {
            _logger.LogWarning("Chapter is not exist !");
            return new UserResponse<object>("Chapter is not exist !", null);
        }

        //Get course form courseId
        var course = await _courseRepository.GetCourseById(chapter.CourseId);
        if (course == null)
        {
            _logger.LogWarning("You can start chapter in your enrolled course !");
            return new UserResponse<object>("You can start chapter in your enrolled course !", null);
        }
        if (course.Status != "Active")
        {
            _logger.LogWarning("This course is unavailable !");
            return new UserResponse<object>("This course is unavailable !", null);
        }

        var studentInCourse = await _courseRepository.GetStudentInCourseById(userId, course.Id);
        if (studentInCourse == null)
        {
            _logger.LogWarning("You can start chapter in your enrolled course !");
            return new UserResponse<object>("You can start chapter in your enrolled course !", null);
        }

        //Update IsStart in Chapter
        chapter.IsStart = true;
        var updateChapter = await _chapterRepository.UpdateChapter(chapter);
        if (!updateChapter)
        {
            return new UserResponse<object>("Start chapter fail", null);
        }

        //Get list chapter by Course Id
        var listChapter = await _chapterRepository.StartChapterById(chapter.CourseId);
        if (listChapter == null)
        {
            _logger.LogWarning("Course do not have any chapter !");
            return new UserResponse<object>("Course do not have any chapter !", null);
        }

        //Get list chapter have IsStart = true
        var listStartChapter = new List<Chapter>();
        foreach (var startChapter in listChapter)
        {
            if (startChapter.IsStart.HasValue)
            {
                listStartChapter.Add(startChapter);
            }
        }

        //Math percent of each chapter have in course
        var percentOfEachChapter = 100.0 / listChapter.Count();

        //Math percent of course progress
            var percentProgressOfCourse = percentOfEachChapter * listStartChapter.Count();
            
            //Update course progress
            studentInCourse.Progress = (decimal)percentProgressOfCourse;
            var updateProgress = await _courseRepository.UpdateStudentInCourse(studentInCourse);
            
            if (!updateProgress)
            {
                return new UserResponse<object>("Update Progress fail", null);
            }
        var chapterResponse = new ChapterRespone()
        {
            CourseId = chapter.CourseId,
            Content = chapter.Content,
            Type = chapter.Type,
            Thumbnail = chapter.Thumbnail,
            Order = chapter.Order,
            Duration = chapter.Duration
        };
        return new UserResponse<object>("Start chapter successfully", chapterResponse);
        
    }
}