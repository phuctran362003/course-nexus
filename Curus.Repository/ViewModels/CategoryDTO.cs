using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Curus.Repository.ViewModels.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Curus.Repository.ViewModels
{
    public class CategoryDTO
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public CategoryStatus Status { get; set; }
    }
}
