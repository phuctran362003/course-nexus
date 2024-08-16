using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class BackupCourse : BaseEntity<int>
    {
        public string Name { get; set; }
        public string ShortSummary { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public decimal Price { get; set; } = 0; 
        public decimal? OldPrice { get; set; } 
        public string Status { get; set; }
        public string Version { get; set; }
        public double? Point { get; set; }
        public bool AllowComments { get; set; } 
        
        //Key
        public int CourseId { get; set; }

        // Navigation Properties
        public virtual Course Course { get; set; }
        public virtual ICollection<BackupChapter> BackupChapters { get; set; } = new HashSet<BackupChapter>();
    }

}
