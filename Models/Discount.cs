using System.ComponentModel.DataAnnotations;
namespace OtlobLap.Models
{
    public class Discount
    {
        public int Id { get; set; }

        [Range(1, 100)]
        public required int Percentage { get; set; }
        public int ProductId { get; set; }
        public required DateTime EndDate {  get; set; }
        
        public  bool IsActive { get; set; } = true;
        public Product?Product { get; set; }
    }
}