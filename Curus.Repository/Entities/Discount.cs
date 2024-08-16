using Curus.Repository.ViewModels;

namespace Curus.Repository.Entities
{
    // admin tạo discount cho instructor
    
    //c2: instructor manage with Date
    
    //admin create Discount
    public class Discount : BaseEntity<int>
    {
        public decimal DiscountPercentage { get; set; } 
        public string DiscountCode { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public bool isAvalaible { get; set; }
        public DiscountStatus? DiscountStatus { get; set; }

        public virtual ICollection<HistoryCourseDiscount> HistoryCourseDiscounts { get; set; } = new HashSet<HistoryCourseDiscount>();
    }

}
