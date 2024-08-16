namespace Curus.Repository.ViewModels.Response;

public class ErrorResponse : BaseResponse
{
    public ErrorResponse(int error, string message) : base(error, message)
    {
    }
}