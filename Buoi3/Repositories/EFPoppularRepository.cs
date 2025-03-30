using Buoi3.Models;
using Microsoft.EntityFrameworkCore;

namespace Buoi3.Repositories
{
	public class EFPoppularRepository : IPoppularRepository
	{
		private readonly ApplicationDbContext _context;

		public EFPoppularRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Poppular>> GetAllAsync()
		{
			return await _context.poppulars.ToListAsync();
		}

		public async Task<Poppular> GetByIdAsync(int id)
		{
			return await _context.poppulars.FindAsync(id);
		}

		public async Task AddAsync(Poppular poppular)
		{
			_context.poppulars.Add(poppular);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Poppular poppular)
		{
			_context.poppulars.Update(poppular);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var pop = await _context.poppulars.FindAsync(id);
			_context.poppulars.Remove(pop);
			await _context.SaveChangesAsync();
		}
	}
}

