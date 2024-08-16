using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels
{
    public class BookMarkedCourseDTO
    {
        public string CourseName { get; set; }
        public string CourseImageThumb { get; set; }
        public decimal CoursePrice { get; set; }
        public decimal Rating { get; set; }
        public string CourseSumary { get; set; }
        
        public List<ViewCategoryNameDTO>? CategoryName { get; set; }
        public int Instructor { get; set; }
    }
}
