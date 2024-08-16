using Curus.Repository.ViewModels.Enum;

namespace Curus.Repository.ViewModels;

public class SendDiscountDTO
{
    public TimeDiscountType TimeStyle { get; set; }
    public double ExpireAmount { get; set; }
}