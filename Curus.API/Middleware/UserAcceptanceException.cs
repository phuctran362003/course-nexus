namespace Curus.API.Middleware;

public class UserAcceptanceException : Exception
{
    public UserAcceptanceException(string message) : base(message)
    {
    }

    public UserAcceptanceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
