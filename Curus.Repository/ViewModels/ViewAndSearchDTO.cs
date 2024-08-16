using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels
{
    public class ViewAndSearchDTO
    {
        [MaxLength(100, ErrorMessage = "Course name must not be more than 100 characters !")]
        [Required(ErrorMessage = "Must not empty !")]
        public string Name { get; set; }
        public string InstructorName { get; set; }
        public string Thumbnail { get; set; }
        public decimal Price { get; set; } = 0;
        public double? Point { get; set; }

        [MaxLength(225, ErrorMessage = "Course name must not be more than 225 characters !")]
        public string ShortSummary { get; set; }
        public List<ViewCategoryNameDTO>? CategoryName { get; set; }

        public string Description { get; set; }
        public int StudentsInCourse { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
