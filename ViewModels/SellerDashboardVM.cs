using OtlobLap.Models;

namespace OtlobLap.ViewModels
{
    public enum FilterOption
    {
        Today,
        ThisYear,
        ThisMonth
    }
    public class SellerDashboardVM
    {
        public int Sales { get; set; }
        public int IncreasedSalesPercent { get; set; }
        public decimal Revenue { get; set; }
        public int IncreasedRevenuePercent { get; set; }
        public int Customers{ get; set; }
        public int ProductsAvailable{ get; set; }
        public int ProductsOut { get; set; }
        public int Reviews { get; set; }
        public decimal ReviewsAvg { get; set; }

        public required List<SaleDto> RecentSales { get; set; }
        public required List<MostSellingDto> MostSellings { get; set; }

    }
}
