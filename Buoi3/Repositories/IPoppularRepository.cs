using Buoi3.Models;

namespace Buoi3.Repositories
{
	public interface IPoppularRepository
	{
		Task<IEnumerable<Poppular>> GetAllAsync();
		Task<Poppular> GetByIdAsync(int id);
		Task AddAsync(Poppular poppular);
		Task UpdateAsync(Poppular poppular);
		Task DeleteAsync(int id);
	}
}
