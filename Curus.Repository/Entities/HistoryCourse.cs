namespace Curus.Repository.Entities;

public class HistoryCourse : BaseEntity<int>
{
    public string? Description { get; set; }
    
    //FK
    public int CourseId { get; set; }
    public int UserId { get; set; }
    
    // Navigation Property
    public virtual Course Course { get; set; }
    public virtual User User { get; set; }
}