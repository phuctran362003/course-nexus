using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    
    public class HistoryCourseDiscount : BaseEntity<int>
    {
        public decimal? DiscountPercentage { get; set; }

        //FK
        public int? InstructorId { get; set; }     
        public int? CourseId { get; set; }    
        public int? DiscountId { get; set; }

        // Navigation Properties
        public virtual User Instructor { get; set; }
        public virtual Course Course { get; set; }
        public virtual Discount Discount { get; set; }
    }


}
