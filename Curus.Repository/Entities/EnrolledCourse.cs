using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class EnrolledCourse : BaseEntity<int>
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public int? InstructorId { get; set; }
        public virtual ICollection<Course>? Courses { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }

}
