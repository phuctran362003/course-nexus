using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class CommentCourse : BaseEntity<int>
    {
        public int UserId { get; set; } //chứa ID của user mà đc comment
        public int CourseId { get; set; } 
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool ByAdmin { get; set; } = false;

        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
    }
}
