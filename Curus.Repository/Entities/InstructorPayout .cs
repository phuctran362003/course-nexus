using Curus.Repository.ViewModels.Enum;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Curus.Repository.Entities
{
            public class InstructorPayout : BaseEntity<int>
            {
                public DateTime PayoutDate { get; set; } // Nullable since it may not be set initially
                public DateTime RequestDate { get; set; }
                public DateTime? RejectionDate { get; set; } // Nullable since it may not be set
                public decimal PayoutAmount { get; set; }
                public PayoutStatus PayoutStatus { get; set; }
                public string? RejectionReason { get; set; } // Nullable since it may not be set

                // Foreign Keys
                [ForeignKey("Instructor")]
                public int InstructorId { get; set; }

                // Navigation Properties
                public virtual User Instructor { get; set; } // Correct navigation property to User
                public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>(); // Initialize to avoid null reference issues
            }


}
