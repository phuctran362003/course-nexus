using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class BookmarkedCourse : BaseEntity<int>
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }
        
        public virtual Course? Course { get; set; }
        public virtual User? User { get; set; }
    }
}
