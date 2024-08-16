using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class StudentOrder : BaseEntity<int>
    {
       
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }

        // Foreign Keys
        public int UserId { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    }





}
