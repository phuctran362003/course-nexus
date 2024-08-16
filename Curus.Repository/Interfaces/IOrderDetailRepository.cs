using Curus.Repository.Entities;

namespace Curus.Repository.Interfaces;

public interface IOrderDetailRepository
{
    Task<List<OrderDetail>> GetOrderDetailByCourseId(int id);
}