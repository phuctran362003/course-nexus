using Curus.Repository.ViewModels.Enum;
using Microsoft.AspNetCore.Http;

namespace Curus.Repository.ViewModels.Response;

public class ChapterRespone
{
    public int CourseId { get; set; }
    public ChapterType Type { get; set; } 
    public string? Thumbnail { get; set; }
    public string Content { get; set; }
    public TimeSpan Duration { get; set; }
    public int Order { get; set; }
}