namespace Curus.Repository.ViewModels.Response;

public class BaseResponse
{
    protected BaseResponse(int error, string message)
    {
        Error = error;
        Message = message;
    }
    
    public int Error { get; set; }
    public string Message { get; set; }
}