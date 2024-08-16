using System.ComponentModel.DataAnnotations;
using Curus.Repository.ViewModels.Enum;
using Microsoft.AspNetCore.Http;

namespace Curus.Repository.ViewModels;

public partial class ChapterDTO
{
    public int CourseId { get; set; }
    public ChapterType Type { get; set; } 
    public IFormFile? Thumbnail { get; set; }
    public IFormFile Content { get; set; }
    public int Order { get; set; }
    
}