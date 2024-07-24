using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtlobLap.Models
{
        public enum OrderStatus
        {
            New,
            InProgress,
            Arrived,
            Rejected,
            Refunded
        }

    public class Order
    {
        // Properties
        [Key]
        public int ID { get; set; }
        public required int CustomerId { get; set; }
        public required int SellerId { get; set; }

        public required DateTime Date { get; set; }
        public required DateTime ArrivalDate { get; set; } 

        [Display(Name = "حالة الطلب")]
        public OrderStatus Status { get; set; } = OrderStatus.New;


        // Navigation Properties
        public List<OrderItem>? OrderItems { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser? Customer{ get; set; }
        [ForeignKey(nameof(SellerId))]
        public ApplicationUser? Seller{ get; set; }
    }

}
