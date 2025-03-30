using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Buoi3.Models
{
    public class Order
    {
        [Key]   
        public int Id { get; set; }
        
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string RecipentName { get; set; }
        public string ShippingAddress { get; set; }
        public string Notes { get; set; }
        public string PhoneNumber { get; set; }
        public OrderStatus Status { get; set; }
        public string UserId { get; set; }      
		public IdentityUser? User { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }

    }
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
        // Add more statuses as needed
    }
}
