using Curus.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Interfaces
{
    public interface IStudentOrderRepository
    {
        Task<StudentOrder> GetOrderByIdAsync(int orderId);
        Task AddOrderAsync(StudentOrder order);
    }
}
