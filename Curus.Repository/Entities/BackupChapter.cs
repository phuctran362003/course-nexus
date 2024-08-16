using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Curus.Repository.ViewModels.Enum;

namespace Curus.Repository.Entities
{
    public class BackupChapter : BaseEntity<int>
    {
        public string Content { get; set; }
        public string? Thumbnail { get; set; }
        public int Order { get; set; }
        public TimeSpan Duration { get; set; }
        public ChapterType? Type { get; set; }

        // Foreign Keys
        public int BackupCourseId { get; set; }
        public int ChapterId { get; set; }

        // Navigation Properties
        public virtual BackupCourse BackupCourse { get; set; }
        public virtual Chapter Chapter { get; set; }
    }


}
