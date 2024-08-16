namespace Curus.Repository.ViewModels.Response;

public class UserResponse<T> 
{
    public string Message { get; set; }
    public T Data { get; set; }

    public UserResponse(string message, T data)
    {
        Message = message;
        Data = data;
    }
}