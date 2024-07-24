using OtlobLap.CustomValidations;
using System.ComponentModel.DataAnnotations;
using static OtlobLap.Models.Order;

namespace OtlobLap.Models
{
    public class OrderItem
    {
        // Properties
        [Key]
        public int ID { get; set; }

        [Display(Name = "الكمية")]
        [AvailableInStockValidation("Product")]
        public int Quantity { get; set; }

        [Display(Name = "السعر")]
        [Range(0.01, double.MaxValue, ErrorMessage = "لابد أن يكون السعر أعلى من صفر")]
        public decimal Price { get; set; }

        [Display(Name = "رقم المنتج")]
        public int ProductId { get; set; }

        [Display(Name = "حالة الطلب")]
        public OrderStatus? Status { get; set; }

        [Display(Name = "رقم الطلب")]
        public int? OrderID { get; set; }

       
        public  int CustomerId { get; set; }

        public int? CartId {  get; set; }

        // Navigation Property for many-to-many relationship
        public Cart? Cart { get; set; }

        // Navigation Property
        [Display(Name = "المنتج")]
        public Product? Product { get; set; }

        [Display(Name = "الطلب")]
        public Order? Order { get; set; }

        [Display(Name = "العميل")]
        public ApplicationUser? Customer { get; set; }
    }


}
