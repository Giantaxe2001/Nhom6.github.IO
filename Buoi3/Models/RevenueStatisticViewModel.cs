using System.ComponentModel.DataAnnotations;
namespace Buoi3.Models
{


    public class RevenueStatisticViewModel
    {
        [Key]
        public int Id { get; set; } // Primary key

        public int Day { get; set; }
        public int Month { get; set; }
        public int ProductId { get; set; } // ID của sản phẩm
        public string ProductName { get; set; } // Tên của sản phẩm
        public int Quantity { get; set; } // Số lượng đã bán
        public decimal TotalAmount { get; set; }
    }

    public class RevenueStatisticFilterViewModel
    {
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public List<RevenueStatisticViewModel> Statistics { get; set; }
    }
}
