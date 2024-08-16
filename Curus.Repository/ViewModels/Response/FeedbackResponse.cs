using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels.Response
{
    public class FeedbackResponse
    {
        public string Content { get; set; }
        public int ReviewPoint { get; set; }
        public string UserName { get; set; }
        public string CourseName { get; set; }
    }
}
