using Curus.Repository.Entities;

namespace Curus.Repository.Interfaces;

public interface IDiscountRepository
{
    Task<bool> CreateDiscount(Discount? discount);

    Task<Discount?> GetDiscountById(int id);

    Task<bool> UpdateDiscount(Discount? discount);

    Task<Discount?> GetDiscountByCode(string code);

    Task<bool> CreateHistoryDiscount(HistoryCourseDiscount historyCourseDiscount);

    Task<HistoryCourseDiscount> GetHistoryCourseDiscount(int id, int instructorId);
    
    Task<List<int?>> GetListCourseByDiscountId(int id);
}