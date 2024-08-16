using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class Role : BaseEntity<int>
    {
        public string? RoleName { get; set; }        

        //Navigation Property
        public virtual ICollection<User>? Users { get; set; }
    }
}
