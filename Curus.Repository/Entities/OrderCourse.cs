using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class OrderCourse : BaseEntity<int>
    {
        //Key
        public int CourseId { get; set; }
        public int OrderDetailId { get; set; }

        public ICollection<Course> Courses { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
