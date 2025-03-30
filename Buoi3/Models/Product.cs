using System.ComponentModel.DataAnnotations;

namespace Buoi3.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; } = new List<ProductImage>();
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int PoppularId { get; set; }
        public Poppular? Poppular { get; set; }

    }
    
}
