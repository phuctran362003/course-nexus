using System.ComponentModel.DataAnnotations;
using Curus.Repository.Entities;
using Curus.Repository.ViewModels.Enum;
using Microsoft.AspNetCore.Http;

namespace Curus.Repository.ViewModels;

public class ReviewCourseDTO 
{

    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public List<ViewCategoryNameDTO>? CategoryName { get; set; }

    public string Description { get; set; }
    public string Thumbnail { get; set; }
    public string ShortSummary { get; set; }
    public bool AllowComments { get; set; } 
    public decimal Price { get; set; } = 0;
    
    public Decimal EarnedMoney { get; set; }
    
    public string InstructorName { get; set; }
    public List<ReviewChapterDTO> Chapters { get; set;}
    
    public List<CommentUserDetail> StudentComment { get; set; }

}

public class ReviewChapterDTO
{
    public int Order { get; set; }
    public TimeSpan Duration { get; set; }
    public string Content { get; set; }
    public string Thumbnail { get; set; }
    public ChapterType? Type { get; set; }
}