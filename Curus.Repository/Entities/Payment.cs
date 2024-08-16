using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class Payment : BaseEntity<int>
    {
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentUrl { get; set; }
        public string? TransactionId { get; set; } // Add this property

        // Foreign Keys
        public int UserId { get; set; }
        public int? StudentOrderId { get; set; }
        public int? InstructorPayoutId { get; set; } // Make this nullable

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual StudentOrder StudentOrder { get; set; }
        public virtual InstructorPayout? InstructorPayout { get; set; } // Make this nullable
    }







}
