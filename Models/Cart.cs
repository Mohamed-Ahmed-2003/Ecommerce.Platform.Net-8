using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtlobLap.Models
{
    public class Cart
    {
        // Properties
        [Key]
        public int ID { get; set; }
        public required int CustomerId { get; set; }

        [InverseProperty("Cart")]
        public ApplicationUser? Customer { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }


}








