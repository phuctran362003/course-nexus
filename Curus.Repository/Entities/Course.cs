using System.ComponentModel.DataAnnotations;
using Curus.Repository.Entities;

public class Course : BaseEntity<int>
{
    public string Name { get; set; }
    public string ShortSummary { get; set; }
    public string Description { get; set; }
    public string Thumbnail { get; set; }
    
    [Range(99999, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 90000")]
    public decimal Price { get; set; } = 0;
    public decimal OldPrice { get; set; }
    public string Status { get; set; }
    public string Version { get; set; }
    public double? Point { get; set; }
    public string? Reason { get; set; }
    public bool AllowComments { get; set; }
    public bool AdminModified { get; set; }

    // Foreign Keys
    public int InstructorId { get; set; }
    public int? StudentInCourseId { get; set; }
    public int? FeedbackId { get; set; }
    public int? ReportId { get; set; }
    public int? BackupCourseId { get; set; }
    public int? BackupChapterId { get; set; }
    public int? CommentCourseId { get; set; }

    // Navigation Properties
    public virtual User Instructor { get; set; }
    public virtual OrderDetail OrderDetail { get; set; }
    public virtual ICollection<Chapter> Chapters { get; set; } = new HashSet<Chapter>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
    public virtual ICollection<Report> Reports { get; set; } = new HashSet<Report>();
    public virtual BackupCourse BackupCourse { get; set; }
    public virtual ICollection<CommentCourse> Comments { get; set; } = new HashSet<CommentCourse>();
    public virtual ICollection<CourseCategory> CourseCategories { get; set; } = new HashSet<CourseCategory>();
    public virtual ICollection<HistoryCourse> HistoryCourses { get; set; } = new HashSet<HistoryCourse>();
    public virtual ICollection<BookmarkedCourse> BookmarkedCourses { get; set; } = new HashSet<BookmarkedCourse>();
    public virtual ICollection<Discount> Discounts { get; set; } = new HashSet<Discount>(); // Added relationship to Discount
    public virtual ICollection<HistoryCourseDiscount> HistoryCourseDiscounts { get; set; } = new HashSet<HistoryCourseDiscount>(); // Added relationship to HistoryCourseDiscount
}





