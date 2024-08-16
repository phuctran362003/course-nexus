namespace Curus.Repository.ViewModels;

public class ManageCourseDTO
{
    public int Id { get; set;}
    public string Name { get; set; } 
    
    public List<ViewCategoryNameDTO>? CategoryName { get; set; }

    public string InstructorName { get; set; }
    
    public int  NumberOfStudent { get; set; }
    
    public Decimal TotalOfPurchased { get; set; }

    public string Version { get; set; }
    
    public double Rating { get; set; }
    
    public List<CommentUserDetail> AdminComment { get; set; }

}