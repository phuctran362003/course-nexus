using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels
{
    public class InstructorPayoutDTO
    {
        public DateTime? PayoutDate { get; set; }
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
        public decimal PayoutAmount { get; set; }
    }
}
