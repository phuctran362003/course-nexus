using Curus.Repository.ViewModels.Enum;

namespace Curus.Repository.ViewModels.Response;

public class CategoryResponseDTO
{
    public int Id { get; set; }
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public CategoryStatus Status { get; set; }
    public ParentCategoryResponseDTO? ParentCategory { get; set; } // Nullable for optional parent category
}

