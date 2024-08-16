using Curus.Repository.Entities;

namespace Curus.Repository.ViewModels;

public class RejectCourseRespone
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public string Description { get; set; }
    public string Thumbnail { get; set; }

    
    public string ShortSummary { get; set; }

    public int InstructorId { get; set; }

    public double? Point { get; set; }

    public decimal Price { get; set; } = 0;
    public List<ReviewRejectChapterDTO> Chapters { get; set; }
    public List<HistoryCourseDTO> History { get; set; }
}