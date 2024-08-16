using Curus.Repository.ViewModels.Enum;

namespace Curus.Repository.ViewModels;

public class ManageInstructorDTO
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public UserStatus? Status { get; set; }
    public int ActivatedCourses { get; set; }
    public int TotalCourses { get; set; }
    public decimal TotalEarnedMoney { get; set; }
    public decimal TotalPayout { get; set; }
    public double RatingNumber { get; set; }
    public List<CommentUserDetail> AdminComment { get; set; }
}