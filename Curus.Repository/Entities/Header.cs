using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class Header : BaseEntity<int>
    {
        public string BranchName { get; set; } = "CURSUS";
        public string SupportHotline { get; set; } = "0123456789";
    }

}
