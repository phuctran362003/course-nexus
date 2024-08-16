using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Curus.Repository.ViewModels.Enum;

namespace Curus.Repository.Entities
{
    public class Chapter : BaseEntity<int>
    {
        public string Content { get; set; }
        public string? Thumbnail { get; set; }
        public int Order { get; set; }
        public TimeSpan Duration { get; set; }
        public ChapterType Type { get; set; } // Convert this to Enum
        
        public bool? IsStart { get; set; }

        // Foreign Key
        public int CourseId { get; set; }

        // Navigation Property
        public virtual Course Course { get; set; }
        public virtual ICollection<Report> Reports { get; set; } = new HashSet<Report>();
        public virtual BackupChapter BackupChapter { get; set; }
    }

}
