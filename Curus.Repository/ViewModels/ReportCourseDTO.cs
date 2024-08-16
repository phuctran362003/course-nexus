using Microsoft.AspNetCore.Http;

namespace Curus.Repository.ViewModels;

public class ReportCourseDTO
{
    public string? Content { get; set; }
    public IFormFile? Attachment { get; set; }
}