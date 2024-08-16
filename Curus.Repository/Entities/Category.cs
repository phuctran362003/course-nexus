using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;

public class Category : BaseEntity<int>
{
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public CategoryStatus Status { get; set; }

    // Navigation Properties
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new HashSet<Category>();
    public virtual ICollection<CourseCategory> CourseCategories { get; set; } = new HashSet<CourseCategory>();
}
