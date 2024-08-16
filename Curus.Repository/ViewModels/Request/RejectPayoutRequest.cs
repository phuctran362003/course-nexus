namespace Curus.Repository.ViewModels.Request;

public class RejectPayoutRequest
{
    public int PayoutRequestId { get; set; }
    public string Reason { get; set; }
}