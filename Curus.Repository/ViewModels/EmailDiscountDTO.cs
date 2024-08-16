namespace Curus.Repository.ViewModels;

public class EmailDiscountDTO
{
    public decimal? DiscountPercentage { get; set; }
    public string? DiscountCode { get; set; }
    public DateTime? ExpireDateTime { get; set; }
}