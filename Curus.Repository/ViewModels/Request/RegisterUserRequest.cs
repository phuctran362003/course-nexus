using System.ComponentModel.DataAnnotations;

namespace Curus.Repository.ViewModels.Request;

public class RegisterUserRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string PhoneNumber { get; set; }
}