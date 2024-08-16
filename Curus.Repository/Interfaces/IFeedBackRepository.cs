using Curus.Repository.Entities;

namespace Curus.Repository.Interfaces;

public interface IFeedBackRepository
{
    Task<List<Feedback>> GetFeedbackByCourseId(int id);

    Task<Feedback> GetFeedbackById(int id);

    Task<bool> UpdateFeedback(Feedback feedback);

    Task<Feedback> GetFeedBackByUserId(int id, int courseId);
}