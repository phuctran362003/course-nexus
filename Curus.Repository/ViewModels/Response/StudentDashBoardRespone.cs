namespace Curus.Repository.ViewModels.Response;

public class StudentDashBoardRespone
{
    public int UserId { get; set; }
    
}

public class Dashboard
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string InstructorId { get; set; }
    public double Rating { get; set; }
    public decimal Price { get; set; }
}

public class SuggestCourse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string InstructorId { get; set; }
    public double Rating { get; set; }
    public decimal Price { get; set; }
}