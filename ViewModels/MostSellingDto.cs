using OtlobLap.Models;

namespace OtlobLap.ViewModels
{
    public class MostSellingDto
    {
        public string ProductPreviewUrl { get; set; } = "no-image.jpg";
        public required string   ProductName { get; set; }
        public decimal Price { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }

}
