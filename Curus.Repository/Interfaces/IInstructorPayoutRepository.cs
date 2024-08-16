using Curus.Repository.Entities;

namespace Curus.Repository.Interfaces;

public interface IInstructorPayoutRepository
{
    Task AddPayoutRequest(InstructorPayout payoutRequest);
    Task<InstructorPayout> GetPayoutRequestById(int payoutRequestId);
    Task UpdatePayoutRequest(InstructorPayout payoutRequest);
    Task<List<InstructorPayout>> GetInstructorPayoutByInstructorId(int instructorId);
}