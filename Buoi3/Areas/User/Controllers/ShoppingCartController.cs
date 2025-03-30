using Buoi3.Helper;
using Buoi3.Models;
using Buoi3.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using NuGet.Protocol.Core.Types;
using System.Security.Cryptography;
using System.Text;


namespace Buoi3.Areas.User.Controllers
{
    [Area("User")]
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICartRepository _cartRepository;
        public ShoppingCartController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ICartRepository cartRepository)
        {
            _context = context;
            _userManager = userManager;
            _cartRepository = cartRepository;
        }
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            // Check if the product already exists in the cart
            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Increase quantity based on the provided quantity
            }
            else
            {
                var cartItem = new CartItem
                {
                    Image = product.ImageUrl,
                    ProductId = productId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity // Set initial quantity based on the provided quantity
                };

                cart.AddItem(cartItem); // Add new item to the cart
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Calculate total items in the cart
            int totalItems = cart.Items.Sum(item => item.Quantity);

            // Update ViewData or TempData to pass totalItems to the view
            ViewData["CartItemCount"] = totalItems.ToString();

			return RedirectToAction("Shop", "Home");
		}

        [Authorize]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var user = _userManager.GetUserAsync(User).Result;

            var viewModel = new CheckoutViewModel
            {
                ShoppingCart = cart,
                UserInfo = user,
                Order = new Order()
            };

            return View(viewModel);
        }

        [HttpPost]
        
        public async Task<IActionResult> Checkout(CheckoutViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Product");
            }
            
            viewModel.Order.UserId = userId;
            viewModel.Order.OrderDate = DateTime.UtcNow;
            viewModel.Order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            viewModel.Order.Status = OrderStatus.Processing;
            viewModel.Order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price,                
            }).ToList();

            _context.Orders.Add(viewModel.Order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");
			int totalItems = cart.Items.Sum(item => item.Quantity);
            ViewData["CartItemCount"] = totalItems;
			return RedirectToAction("OrderComplete", new { orderId = viewModel.Order.Id });
        }
        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
        public IActionResult OrderComplete(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return RedirectToAction("thankyou", "Product");
            }
            return View(order);
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                cart.RemoveItem(productId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index", "ShoppingCart");
        }


        public IActionResult UpdateCartItem(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                cart.UpdateItem(productId, quantity);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index", "ShoppingCart"); // Redirect to the cart page or any other appropriate page
        }

    }
}
