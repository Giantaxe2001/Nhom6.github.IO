using Buoi3.Models;
using Buoi3.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Buoi3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
       
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ApplicationDbContext _Context;
        public HomeController(IProductRepository productRepository, ICategoryRepository categoryRepository, ILogger<HomeController> logger, ApplicationDbContext Context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _Context = Context;
        }
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
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
