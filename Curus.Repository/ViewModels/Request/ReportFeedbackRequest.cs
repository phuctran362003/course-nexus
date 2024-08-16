using System.ComponentModel.DataAnnotations;

namespace Curus.Repository.ViewModels.Request;

public class ReportFeedbackRequest
{
    [Required]
    public string Reason { get; set; }
}