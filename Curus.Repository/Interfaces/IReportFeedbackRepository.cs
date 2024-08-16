using Curus.Repository.Entities;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

namespace Curus.Repository.Interfaces;

public interface IReportFeedbackRepository
{
    Task<ReportFeedback> GetReportFeedbackByFeedbackId(int id);
    Task<bool> CreateReportFeedback(ReportFeedback reportFeedback);

    Task<ReportFeedback> GetReportFeedbackById(int id);
    Task<bool> EditReportFeedback(ReportFeedback reportFeedback);
}