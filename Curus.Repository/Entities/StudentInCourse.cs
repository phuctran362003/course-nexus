using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class StudentInCourse : BaseEntity<int>
    {
        //Key
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public int InstructorId { get; set; }
        public int Rating { get; set; }
        public bool IsFinish { get; set; } = false;


        public decimal Progress { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
