using Buoi3.Models;
using Microsoft.EntityFrameworkCore;

namespace Buoi3.Repositories
{
    public class EFOrderDetailRepository : IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public EFOrderDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetails.ToListAsync();
        }

        public async Task<OrderDetail> GetByIdAsync(int id)
        {
            return await _context.OrderDetails.FindAsync(id);
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }
    }
}

