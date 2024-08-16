using System.ComponentModel.DataAnnotations;
using Curus.Repository.ViewModels.Enum;
using Microsoft.AspNetCore.Http;

namespace Curus.Repository.ViewModels;

public class ReviewRejectChapterDTO
{
    public int CourseId { get; set; }
    public ChapterType? Type { get; set; } 
    public string Content { get; set; }
    public string? Thumbnail { get; set; }
    public int Order { get; set; }
    [RegularExpression(@"^(\d+\.?\d*|\.\d+):\d{2}:\d{2}$", ErrorMessage = "Invalid TimeSpan format.")]
    public TimeSpan Duration { get; set; } 
}