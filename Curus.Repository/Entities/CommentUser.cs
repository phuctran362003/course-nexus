using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class CommentUser : BaseEntity<int>
    {
        //chứa userID mà ĐƯỢC ADMIN comment
        public int UserId { get; set; }
        public int CommentedById { get; set; } // chứa ID của admin mà comment

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }
        public virtual User CommentedBy { get; set; }
    }

}
