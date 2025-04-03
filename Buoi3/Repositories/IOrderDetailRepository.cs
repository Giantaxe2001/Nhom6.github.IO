using Buoi3.Models;

namespace Buoi3.Repositories
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetail>> GetAllAsync();
        Task<OrderDetail> GetByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
    }
}
