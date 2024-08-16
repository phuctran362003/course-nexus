using Curus.Repository.Entities;


public class Feedback : BaseEntity<int>
{
    public string Content { get; set; }
    public int? Ordering { get; set; }
    public string? Attachment { get; set; }
    public int ReviewPoint { get; set; }
    public bool IsMarkGood { get; set; }

    // Foreign key properties
    public int CourseId { get; set; }
    public int UserId { get; set; }

    // Navigation properties
    public ReportFeedback ReportFeedback { get; set; }
    public User User { get; set; }
    public Course Course { get; set; }
}

