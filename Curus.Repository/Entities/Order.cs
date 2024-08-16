using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class Order : BaseEntity<int>
    {
       
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }

        //Key 
        public int UserId { get; set; }

        //Niga
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual ICollection<OrderTransaction> OrderTransactions { get; set; }
    }
}
