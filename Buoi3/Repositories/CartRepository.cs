using Buoi3.Models;

namespace Buoi3.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly List<CartItem> _cartItems; // In-memory storage, replace with actual storage mechanism

        public CartRepository()
        {
            _cartItems = new List<CartItem>();
            // Initialize with sample data or fetch from a database
        }

        public CartItem GetCartItemById(int productId)
        {
            return _cartItems.FirstOrDefault(item => item.ProductId == productId);
        }

        public void UpdateCartItem(CartItem cartItem)
        {
            var existingItem = _cartItems.FirstOrDefault(item => item.ProductId == cartItem.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity = cartItem.Quantity;
                // Optionally, update other properties as needed
            }
        }

        // Implement other methods such as AddCartItem, RemoveCartItem, etc. based on your application needs
    }
}
