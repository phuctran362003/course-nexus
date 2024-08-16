using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Curus.Repository.ViewModels;

public class CourseEditDTO
{
    [MaxLength(100,ErrorMessage = "Course name must not be more than 100 characters !")]
    public string? Name { get; set; }
    
    public List<int>? CategoryIds { get; set; }
    public string? Description { get; set; }
    public IFormFile? Thumbnail { get; set; }
    [MaxLength(225,ErrorMessage = "Course name must not be more than 225 characters !")]

    public string? ShortSummary { get; set; }

    public bool AllowComments { get; set; } 
    [Range(99999, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 90000")]
    public decimal Price { get; set; } = 0;
}