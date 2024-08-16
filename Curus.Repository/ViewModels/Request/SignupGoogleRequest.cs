using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels.Request
{
    public class SignupGoogleRequest
    {
        public DateTime Birthday { get; set; }

        public string Address { get; set; }

        public string FullName { get; set; } = "redis name!";

     

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number.")]

        // public string Type { get; set; }

        public string PhoneNumber { get; set; }
    }
}
