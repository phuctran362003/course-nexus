using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Curus.Repository.ViewModels.Enum;

namespace Curus.Repository.Entities
{
    public class Report : BaseEntity<int>
    {

        public string? Content { get; set; }
        public string? Attachment { get; set; }
        public ReportStatus? Status { get; set; }

        //Key
        public int UserId { get; set; }
        public int CourseId { get; set; }
        
        public int? ChapterId { get; set; }

        //Navigation Property
        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
        public virtual Chapter Chapter { get; set; }
    }
}
