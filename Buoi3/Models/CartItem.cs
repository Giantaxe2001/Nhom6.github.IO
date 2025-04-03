namespace Buoi3.Models
{
    public class CartItem
    {
        
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public string? Image { get; set; }
     
    }
    public class CartItemUpdateModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
