using Buoi3.Models;
using Microsoft.EntityFrameworkCore;

namespace Buoi3.Repositories
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public EFOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

	
	}
}
