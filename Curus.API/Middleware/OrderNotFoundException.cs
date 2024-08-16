namespace Curus.API.Middleware;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(string message) : base(message)
    {
    }
}