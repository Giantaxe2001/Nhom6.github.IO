using Microsoft.AspNetCore.Identity;

namespace Buoi3.Models
{
    public class CheckoutViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }
        public IdentityUser UserInfo { get; set; }
        public Order Order { get; set; }
    }
}
