using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Curus.Repository.ViewModels;

public class FeedbackCourseDTO
{
    public string Content { get; set; }
    public IFormFile? Attachment { get; set; }
    public int ReviewPoint { get; set; }
}
