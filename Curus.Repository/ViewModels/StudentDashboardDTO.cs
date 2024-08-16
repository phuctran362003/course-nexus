namespace Curus.Repository.ViewModels;

public class StudentDashboardDTO
{
    public int UserId { get; set; }
    public GeneralInformation GeneralInformation { get; set; }
    public List<SuggestCourse> SuggestCourses { get; set; }
}

public class GeneralInformation
{
    public int TotalPaidCourse { get; set; }
    public int InprogressCourse { get; set; }
    public int DoneCourse { get; set; }
}

public class SuggestCourse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int InstructorId { get; set; }
    public decimal Price { get; set; }
    public double? Rating { get; set; }
}