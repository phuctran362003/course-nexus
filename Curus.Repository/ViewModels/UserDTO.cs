using Curus.Repository.Entities;

namespace Curus.Repository.ViewModels;

public class UserDTO
{
    public int UserId { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Password { get; set; }

    public string? RefreshToken { get; set; }

    public Role Role { get; set; }


}