using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels
{
    public class UpdateUserDTO
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDay { get; set; }
    }
}
