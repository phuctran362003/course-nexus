using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;

namespace Curus.Service.Interfaces;

public interface IReportFeedbackService
{
    Task<UserResponse<object>> reportReviewToAdmin(int id, ReportFeedbackRequest reportFeedbackRequest);

    Task<UserResponse<object>> acceptReportFeedback(int id);

    Task<UserResponse<object>> rejectReportFeedback(int id);
}