using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Curus.Repository.ViewModels;

public class CourseDTO
{
    
    [MaxLength(100,ErrorMessage = "Course name must not be more than 100 characters !")]
    [Required(ErrorMessage = "Must not empty !")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = ("CategoryId field must not be empty !"))]
    public List<int> CategoryIds { get; set; }
    public string Description { get; set; }
    public IFormFile Thumbnail { get; set; }
    [MaxLength(225,ErrorMessage = "Course name must not be more than 225 characters !")]

    public string ShortSummary { get; set; }

    public bool AllowComments { get; set; } 
    [Range(99999, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 90000")]
    public decimal Price { get; set; } = 0;

    // public decimal? OldPrice { get; set; }
    // public string Attachments { get; set; }
}
