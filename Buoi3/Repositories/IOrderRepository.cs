using Buoi3.Models;
using Microsoft.EntityFrameworkCore;

namespace Buoi3.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int id);       
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);

	}
}
