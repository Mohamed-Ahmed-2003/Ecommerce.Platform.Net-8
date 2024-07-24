using Microsoft.AspNetCore.Mvc;

namespace OtlobLap.ViewModels
{
    public class TopSellerDto
    {
        public required string SellerName { get; set; }
        public  byte []? UserImage { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalSales { get; set; }
        public int TotalReviews { get; set; }
        public int AvgRating { get; set; }
    }

}
