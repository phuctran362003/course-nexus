using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels.Response
{
    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set;}
        public int Course { get; set; }
        public double RatingPoint {  get; set; }
    }
}
