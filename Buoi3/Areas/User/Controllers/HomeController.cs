using Buoi3.Models;
using Buoi3.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace Buoi3.Areas.User.Controllers
{
    [Area("User")]
    

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IProductRepository _productRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly IOrderDetailRepository _orderDetailRepository;
		private readonly ApplicationDbContext _Context;
       
        public HomeController(IProductRepository productRepository, ICategoryRepository categoryRepository, ILogger<HomeController> logger,ApplicationDbContext Context)
		{
			_productRepository = productRepository;
			_categoryRepository = categoryRepository;
			_logger = logger;
			_Context = Context;
  
        }
		public class CheckoutViewModel
		{
			public IEnumerable<Product> Products { get; set; }
			public IEnumerable<Category> Categories { get; set; }
		}
		// Hiển thị danh sách sản phẩm
		public async Task<IActionResult> Index()
		{
			var products = await _productRepository.GetAllAsync();
			foreach (var product in products)
			{
				if (product.CategoryId != null)
				{
					product.Category = await _categoryRepository.GetByIdAsync(product.CategoryId);
				}
			}
			return View(products);
		}

       

		public IActionResult blog()
		{
			return View();
		}

		public IActionResult contact()
		{
			return View();
		}

		
		public IActionResult services()
		{
			return View();
		}
		public async Task<IActionResult> shop()
		{
			
			var categories = await _categoryRepository.GetAllAsync();
			ViewBag.Categories = categories;

			var products = await _productRepository.GetAllAsync();
            foreach (var product in products)
            {
                if (product.CategoryId != null)
                {
                    product.Category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                }
            }
			var viewModel = new CheckoutViewModel
			{
				Products = products,
				Categories = categories
			};
			return View(viewModel);
        }
		public async Task<IActionResult> shopCategory(int id)
		{

			var categories = await _categoryRepository.GetAllAsync();
			ViewBag.Categories = categories;
			var products = await _productRepository.GetProductsByCategoryIdAsync(id);
			var viewModel = new CheckoutViewModel
			{
				Products = products,
				Categories = categories
			};
			return View(viewModel);
			
           
        }
		public IActionResult Details (int id)
		{
			Product products = _Context.Products.Include(sp => sp.Category).FirstOrDefault(sp=>sp.Id == id);
			return View(products);
		}
		public IActionResult thankyou()
		{
			return View();
		}
		[Authorize]
        public async Task<IActionResult> MyOrders()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Retrieve orders for the current user
			var userOrders = _Context.Orders.Where(o => o.UserId == userId).ToList();

			return View(userOrders);
        }
		public class OrderDetailsViewModel
		{

			public Order Order { get; set; }
			public IEnumerable<OrderDetail> OrderDetails { get; set; }
		}
			public ActionResult OrderDetails(int id)
			{
				var order = _Context.Orders.Find(id);
				if (order == null)
				{
					TempData["ErrorMessage"] = "Order not found.";
					return RedirectToAction("Index"); // Redirect to the list of orders
				}

				return View(order); // Pass the order to the view
			}
		
			public ActionResult OrderDelete(int id)
			{
				// Retrieve the order from the database
				var order = _Context.Orders.Find(id);
				if (order == null)
				{
				TempData["ErrorMessage"] = "Order not found.";
				return RedirectToAction("MyOrders");
			}

				// Remove the order from the database
				_Context.Orders.Remove(order);
				_Context.SaveChanges();

				return RedirectToAction("MyOrders"); // Redirect to the list of orders
			}

        // Other actions...

        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("SearchResults", Enumerable.Empty<Product>());
            }

            var products = _Context.Products
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .ToList();

            return View("SearchResults", products);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public class ContactFormViewModel
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Message { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(ContactFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var senderEmail = new MailAddress("your-email@gmail.com", "Your Name");
                var receiverEmail = new MailAddress("petcareda@gmail.com", "Receiver");
                var password = "your-email-password"; // Use secure method to store and retrieve the password
                var sub = "Contact Form Submission";
                var body = $"First Name: {model.FirstName}\nLast Name: {model.LastName}\nEmail: {model.Email}\nMessage: {model.Message}";
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };

                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = sub,
                    Body = body
                })
                {
                    await smtp.SendMailAsync(mess);
                }

                return RedirectToAction("Success"); // Redirect to a success page
            }

            return View("Contact", model); // Return to the form with validation messages
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
