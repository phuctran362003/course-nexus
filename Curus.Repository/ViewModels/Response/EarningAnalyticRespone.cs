using Curus.Repository.ViewModels.Enum;

namespace Curus.Repository.ViewModels.Response;

public class EarningAnalyticRespone
{
    public decimal TotalEarning { get; set; }
    public decimal PayoutEarning { get; set; }
    public decimal MaintainMoney { get; set; }
    public List<CourseEarning> CourseEarning { get; set; }
    public List<HistoryTransfer> HistoryTransfers { get; set; }
}

public class CourseEarning
{
    public int CourseId { get; set; }
    public decimal Earning { get; set; }
}

public class HistoryTransfer
{
    public DateTime TransferDate { get; set; }
    public decimal Amount { get; set; }
    public PayoutStatus Status { get; set; }
}