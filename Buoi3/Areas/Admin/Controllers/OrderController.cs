using Buoi3.Models;
using Buoi3.Repositories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace Buoi3.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class OrderController : Controller
	{
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ApplicationDbContext _context;

        

        public OrderController(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, ApplicationDbContext context)
        {
           
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
            _context = context;
        }


        public IActionResult Index()
        {
            var viewModel = new OrderDetailsViewModel
            {
                Orders = _context.Orders.ToList(), // Fetch all orders from the database
                OrderDetails = _context.OrderDetails.ToList(), // Fetch all order details from the database
            };

            return View(viewModel);
        }


        public IActionResult AddAsync()
        {
            return View();
        }

    
        public class ChangeStatusViewModel
        {
            public int OrderId { get; set; }
            public OrderStatus CurrentStatus { get; set; }
            public SelectList StatusList { get; set; }
        }
    

    public class OrderDetailsViewModel
        {
            public IEnumerable<Order> Orders { get; set; }
            public Order Order { get; set; }
            public IEnumerable<OrderDetail> OrderDetails { get; set; }
            public OrderDetail OrderDetail { get; set; } 
            public int OrderDetailId { get; set; }          
            public int SelectedStateId { get; set; }


        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new ChangeStatusViewModel
            {
                OrderId = order.Id,
                CurrentStatus = order.Status,
                StatusList = new SelectList(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>())
            };

            return View(viewModel);
        }

        // POST: Order/ChangeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(ChangeStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.StatusList = new SelectList(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>());
                return View(model);
            }

            var order = await _orderRepository.GetByIdAsync(model.OrderId);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = model.CurrentStatus;
            await _orderRepository.UpdateAsync(order);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> OrderDetails(int id)
        {
            // Retrieve the order with the specified id including related details and state
            var orderDetails = _context.OrderDetails
                .Where(od => od.OrderId == id)
                .Include(od => od.Product)               
                .ToList();

            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound(); // Return 404 if order is not found
            }

            // Create the view model
            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                OrderDetails = orderDetails,               
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Product)                
                .FirstOrDefaultAsync(od => od.Id == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            var viewModel = new OrderDetailsViewModel
            {
                OrderDetails = new List<OrderDetail> { orderDetail },
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Orders"); // Redirect to a list of orders or appropriate view
            }

            var viewModel = new OrderDetailsViewModel
            {
                OrderDetails = new List<OrderDetail> { orderDetail },   
            };

            return View(viewModel);
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
   


     
    }
}



