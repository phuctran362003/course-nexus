using Curus.Repository.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels
{
    public class UserRegistrationDTO
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        [Required]
        [Phone]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
        [Required]
        [PasswordValidation]
        public string Password { get; set; }
    }
}
