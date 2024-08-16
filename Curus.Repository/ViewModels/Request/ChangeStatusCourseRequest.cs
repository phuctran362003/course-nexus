using System.ComponentModel.DataAnnotations;
using Curus.Repository.ViewModels.Enum;

namespace Curus.Repository.ViewModels.Request;

public class ChangeStatusCourseRequest
{
    public string Reason { get; set; }
    public ChangeStatus ChangeByTime { get; set; }
    public int? DeactivationPeriod { get; set; }

}