using Curus.Repository.Entities;

namespace Curus.Repository.ViewModels;

public class StudentOrderDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderStatus { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderDetailDTO> OrderDetails { get; set; } 
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}

public class OrderDetailDTO
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public decimal Price { get; set; }
}
