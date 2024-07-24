using OtlobLap.Models;

namespace OtlobLap.ViewModels
{
    public class SaleDto
    {
        public int OrderId { get; set; }
        public required string CustomerName { get; set; }
        public required string ProductName { get; set; }
        public decimal Price { get; set; }
        public  OrderStatus Status { get; set; }
    }

}
