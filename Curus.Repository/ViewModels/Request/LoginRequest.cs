using System.ComponentModel.DataAnnotations;

namespace Curus.Repository.ViewModels.Request;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}