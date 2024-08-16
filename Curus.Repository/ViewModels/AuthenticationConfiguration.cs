namespace Curus.Repository.ViewModels;

public class AuthenticationConfiguration
{
    public string? AccessTokenSecret { get; set; }
    public double AccessTokenExpirationMinutes { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }

}