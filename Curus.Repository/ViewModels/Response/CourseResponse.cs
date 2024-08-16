using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels.Response
{
    public class CourseResponse
    {
        [MaxLength(100, ErrorMessage = "Course name must not be more than 100 characters !")]
        [Required(ErrorMessage = "Must not empty !")]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Thumbnail { get; set; }
        [MaxLength(225, ErrorMessage = "Course name must not be more than 225 characters !")]

        public string ShortSummary { get; set; }

        public bool AllowComments { get; set; }

        public decimal Price { get; set; } = 0;
    }
}