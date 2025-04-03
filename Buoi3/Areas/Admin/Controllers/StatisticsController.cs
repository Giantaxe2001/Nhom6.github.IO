using Buoi3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buoi3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var model = new RevenueStatisticFilterViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Statistics = GetRevenueStatistics(startDate, endDate)
            };

            return View(model);
        }

        private List<RevenueStatisticViewModel> GetRevenueStatistics(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.OrderDetails
                                .Include(od => od.Order)
                                .Include(od => od.Product) // Include Product information
                                .Where(od => (!startDate.HasValue || od.Order.OrderDate >= startDate) &&
                                              (!endDate.HasValue || od.Order.OrderDate <= endDate))
                                .GroupBy(od => new { Day = od.Order.OrderDate.Day, Month = od.Order.OrderDate.Month, od.ProductId, od.Product.Name }) // Group by ProductId and Product Name
                                .Select(g => new RevenueStatisticViewModel
                                {
                                    Day= g.Key.Day,
                                    Month = g.Key.Month,                              
                                    ProductId = g.Key.ProductId,
                                    ProductName = g.Key.Name, // Set ProductName from group key
                                    Quantity = g.Sum(od => od.Quantity), // Calculate total quantity sold
                                    TotalAmount = g.Sum(od => od.Price * od.Quantity)
                                })
                                .ToList();

            return query;
        }
    }
}
