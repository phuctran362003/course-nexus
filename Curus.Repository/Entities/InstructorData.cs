using Curus.Repository.ViewModels.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class InstructorData : BaseEntity<int>
    {
        public string? TaxNumber { get; set; }
        public string? CardNumber { get; set; }
        public string? CardName { get; set; }
        public CardProviderEnum CardProvider { get; set; }
        public string? Certification { get; set; }

        // Foreign Keys
        public int UserId { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual ICollection<InstructorPayout> InstructorPayouts { get; set; } = new HashSet<InstructorPayout>();
        public virtual ICollection<CommentUser> Comments { get; set; } = new HashSet<CommentUser>();
        public virtual ICollection<ReportFeedback> ReportFeedbacks { get; set; } = new HashSet<ReportFeedback>();
        public virtual ICollection<Discount> Discounts { get; set; } = new HashSet<Discount>();
        public virtual ICollection<HistoryCourseDiscount> HistoryCourseDiscounts { get; set; } = new HashSet<HistoryCourseDiscount>(); // Added relationship to HistoryCourseDiscount

    }



}
