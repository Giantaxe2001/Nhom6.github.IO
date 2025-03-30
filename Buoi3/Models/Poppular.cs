using System.ComponentModel.DataAnnotations;

namespace Buoi3.Models
{
	public class Poppular
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public List<Product>? Products { get; set; }
	}
}
