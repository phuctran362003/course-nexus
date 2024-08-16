using Curus.Repository.ViewModels.Enum;
using Microsoft.AspNetCore.Http;

namespace Curus.Repository.ViewModels;

public class UpdateChapterDTO
{
    public IFormFile? Content { get; set; }
    public IFormFile? Thumbnail { get; set; }
    
    public int? Order { get; set;}
    public ChapterType Type { get; set; } 
}