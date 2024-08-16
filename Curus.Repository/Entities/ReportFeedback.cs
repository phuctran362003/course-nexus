using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class ReportFeedback : BaseEntity<int>
    {
        public string ReportReason { get; set; }
        public bool IsHidden { get; set; }
        public int FeedbackId { get; set; }
        public int UserId { get; set; } 

        // Navigation properties
        public Feedback Feedback { get; set; }
        public User User { get; set; }        
    }
}
