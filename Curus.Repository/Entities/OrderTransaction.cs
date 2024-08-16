using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class OrderTransaction : BaseEntity<int>
    { 
        public DateTime TransactionDate { get; set; }
        public string TransactionStatus { get; set; }
        public string PaymentMethod { get; set; }

        //Key
        public int OrderId { get; set; }

        //Navigation properties
        public virtual Order Order { get; set; }
    }
}
