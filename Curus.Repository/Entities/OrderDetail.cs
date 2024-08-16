using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class OrderDetail : BaseEntity<int>
    {
        // Foreign Keys
        public int CourseId { get; set; }
        public int StudentOrderId { get; set; }
        public decimal CoursePrice { get; set; } 

        // Navigation Properties
        public virtual Course Course { get; set; }
        public virtual StudentOrder StudentOrder { get; set; }
    }

}
