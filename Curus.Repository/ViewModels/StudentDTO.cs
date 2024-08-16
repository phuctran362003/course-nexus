namespace Curus.Repository.ViewModels;

public class StudentDTO
{
    public string PhoneNumber { get; set; }
    
    public string FullName { get; set; }

    public string Email { get; set; }
    
    public int JoinedCourse { get; set; }
    
    public int NumberInProgressCourse { get; set; }
}