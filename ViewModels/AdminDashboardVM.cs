
namespace OtlobLap.ViewModels
{
    public class AdminDashboardVM
    {
        public List<TopSellerDto>? TopSellerRevenue { get; set; }
        public List<TopSellerDto>? TopSellerRated { get; set; }
        public List<SaleDto>? RecentSales { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalCategories{ get; set; }
        public int TotalRoles{ get; set; }
        public int TotalOrders { get; set; } // Consider including this for completeness
        public int TotalSellers { get;  set; } 
    }

}
