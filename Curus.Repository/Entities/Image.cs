using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class Image : BaseEntity<int>
    {
        

        public string? PathUrl { get; set; }
        public int CourseId { get; set; }

        public Course? Course { get; set; }


    }
}
