using Buoi3.Models;
using System.Collections.Generic;
namespace Buoi3.Repositories
{
    public interface ICartRepository
    {
        CartItem GetCartItemById(int productId);
        void UpdateCartItem(CartItem cartItem);
        // Add other methods as needed, e.g., AddCartItem, RemoveCartItem, etc.
    }
}
