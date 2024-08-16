using Curus.Repository.Entities;

public class CourseCategory : BaseEntity<int>
{
    public int CourseId { get; set; }
    public int CategoryId { get; set; }

    // Navigation Properties
    public virtual Course Course { get; set; }
    public virtual Category Category { get; set; }
}
