using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.ViewModels;

namespace OtlobLap.Services
{

    public interface ISellerHomeService
    {
        Task<List<MostSellingDto>> GetBestSellingProductsAsync(int sellerId);
        Task<Tuple<int, int>> GetSalesCountAsync(int sellerId, FilterOption filterOption);
        Task<Tuple<decimal, decimal>> GetRevenueAsync(int sellerId, FilterOption filterOption);
        Task<SellerDashboardVM> GetSellerInfoAsync(int sellerId);
        Task<int> GetCustomersCountAsync(int sellerId);
        Task<List<SaleDto>> GetRecentSales(int sellerId);
        Task<Tuple<int, int>> GetProductsCount (int sellerId);
        Task<Tuple<int, decimal>> GetReviewsInfo (int sellerId);
     }


    public class SellerHomeService (AppDbContext context) : ISellerHomeService
    {
 
        private readonly AppDbContext _context  = context;



        public async Task<List<MostSellingDto>> GetBestSellingProductsAsync(int sellerId)
        {
            var bestSellingProducts = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.SellerId == sellerId)
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.ProductId) // Group by product ID to aggregate sales data
                .Select(g => new MostSellingDto
                {
                    ProductName = g.First().Product.Name,
                    Price = g.First().Product.Price, // You may need to adjust this based on your data 
                    ProductPreviewUrl = g.First().Product.MainProductImage,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Price)
                })
                .OrderByDescending(dto => dto.UnitsSold) // Order by units sold, or any other criteria for "best-selling"
                .ToListAsync();

            return bestSellingProducts;
        }



        public async Task<int> GetCustomersCountAsync(int sellerId)
        {
            return await _context.Orders
                .Where(o => o.SellerId == sellerId)
                .Select(o => o.CustomerId)
                .Distinct()
                .CountAsync();
        }

        public async Task<Tuple<int, int>> GetProductsCount(int sellerId)
        {
            var products = await _context.Products
                .Where(p => p.SellerID == sellerId)
                .ToListAsync();

            int available = products.Count(p => p.AvailableInStock > 0);
            int stockOut = products.Count(p => p.AvailableInStock == 0);
            
            return Tuple.Create(available, stockOut);
        }



        public async Task<List<SaleDto>> GetRecentSales(int sellerId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.SellerId == sellerId)
                    .OrderByDescending(o => o.Date)
                    .SelectMany(o => o.OrderItems.Select(oi => new SaleDto
                    {
                        OrderId = o.ID,
                        CustomerName = o.Customer != null ? o.Customer.Name : null,
                        ProductName = oi.Product != null ? oi.Product.Name : null,
                        Price = oi.Product != null && oi.Product.Discount != null ? Utility.CalculateDiscountedPrice(oi.Price, oi.Product.Discount.Percentage) : oi.Price,
                        Status = o.Status
                    })).Take(10)

                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving recent sales: {ex.Message}");
                return new List<SaleDto>(); // Return an empty list or null as appropriate
            }
        }

        public async Task<Tuple<decimal, decimal>> GetRevenueAsync(int sellerId, FilterOption filterOption)
        {
            var Dates = FormatDateOptions(filterOption);
            var (currDate, prevDate) = (Dates.Item1, Dates.Item2);

            var newRevenue = await _context.Orders.Include(o => o.OrderItems)
                .Where(o => o.SellerId == sellerId && o.Status == OrderStatus.Arrived && o.Date >= currDate.Item1 && o.Date <= currDate.Item2)
                .SelectMany(o => o.OrderItems)
                .SumAsync(o => o.Price);

            var oldRevenue = await _context.Orders.Include(o => o.OrderItems)
                .Where(o => o.SellerId == sellerId && o.Status == OrderStatus.Arrived && o.Date >= prevDate.Item1 && o.Date <= prevDate.Item2)
                .SelectMany(o => o.OrderItems)
                .SumAsync(o => o.Price);

            return Tuple.Create(newRevenue, oldRevenue);
        }

        public async Task<Tuple<int, decimal>> GetReviewsInfo(int sellerId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.product)
                .Where(r => r.product.SellerID == sellerId)
                .ToListAsync();

            int total = reviews.Count;
            decimal average = total > 0 ? reviews.Sum(r => r.Rating) / (decimal)total : 0;

            return Tuple.Create(total, average);
        }


        public async Task<Tuple<int,int>> GetSalesCountAsync(int sellerId, FilterOption filterOption) {
            var Dates = FormatDateOptions(filterOption);
            var (currDate, prevDate) = (Dates.Item1, Dates.Item2);

            var newSales = await _context.Orders.Include(o => o.OrderItems).Where(o => o.SellerId == sellerId &&o.Status == OrderStatus.Arrived && o.Date >= currDate.Item1 && o.Date <= currDate.Item2).SelectMany (o => o.OrderItems)
                .CountAsync();

            var oldSales = await _context.Orders.Include(o => o.OrderItems).Where(o => o.SellerId == sellerId &&o.Status == OrderStatus.Arrived && o.Date >= prevDate.Item1 && o.Date <= prevDate.Item2).SelectMany (o => o.OrderItems)
                .CountAsync();

            return Tuple.Create(newSales,oldSales);
        }

        public async Task<SellerDashboardVM> GetSellerInfoAsync(int sellerId)
        {
            try
            {
                var sales = await GetSalesCountAsync(sellerId, FilterOption.Today);
                var revenueInfo = await GetRevenueAsync(sellerId, FilterOption.Today);
                var customers = await GetCustomersCountAsync(sellerId);
                var recentSales = await GetRecentSales(sellerId);
                var mostSelling = await GetBestSellingProductsAsync(sellerId);
                var reviewsInfo = await GetReviewsInfo(sellerId);
                var productsInfo = await GetProductsCount(sellerId);

                var increasedSalesPercent = CalculatePercentageChange(sales.Item2, sales.Item1);
                var increasedRevenuePercent = CalculatePercentageChange((int)revenueInfo.Item2, (int)revenueInfo.Item1);

                return new SellerDashboardVM
                {
                    Sales = sales.Item1,
                    IncreasedSalesPercent = increasedSalesPercent,
                    Revenue = revenueInfo.Item1,
                    IncreasedRevenuePercent = increasedRevenuePercent,
                    Customers = customers,
                    RecentSales = recentSales,
                    MostSellings = mostSelling,
                    Reviews = reviewsInfo.Item1,
                    ReviewsAvg = reviewsInfo.Item2,
                    ProductsAvailable = productsInfo.Item1,
                    ProductsOut = productsInfo.Item2
                };
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw; // Re-throw the exception or handle it appropriately
            }
        }


        private int CalculatePercentageChange(int oldValue, int newValue)
        {
            return oldValue == 0 ? (newValue > 0 ? 100 : 0) : ((newValue - oldValue) * 100 / oldValue);
        }


        private Tuple<Tuple<DateTime, DateTime>, Tuple<DateTime, DateTime>> FormatDateOptions(FilterOption filterOption)
        {
            DateTime currentStartDate, currentEndDate, previousStartDate, previousEndDate;

            switch (filterOption)
            {
                case FilterOption.Today:
                    currentStartDate = DateTime.Today;
                    currentEndDate = DateTime.Today.AddDays(1).AddTicks(-1); // End of today
                    previousStartDate = currentStartDate.AddDays(-1); // Previous day
                    previousEndDate = currentEndDate.AddDays(-1); // Previous day
                    break;
                case FilterOption.ThisYear:
                    currentStartDate = new DateTime(DateTime.Today.Year, 1, 1);
                    currentEndDate = DateTime.Today.AddDays(1).AddTicks(-1); // End of today
                    previousStartDate = currentStartDate.AddYears(-1); // Previous year
                    previousEndDate = currentEndDate.AddYears(-1); // Previous year
                    break;
                case FilterOption.ThisMonth:
                    currentStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    currentEndDate = DateTime.Today.AddDays(1).AddTicks(-1); // End of today
                    previousStartDate = currentStartDate.AddMonths(-1); // Previous month
                    previousEndDate = currentEndDate.AddMonths(-1); // Previous month
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterOption));
            }

            // Create tuples for current and previous date ranges
            var currentDateRange = Tuple.Create(currentStartDate, currentEndDate);
            var previousDateRange = Tuple.Create(previousStartDate, previousEndDate);

            return Tuple.Create(currentDateRange, previousDateRange);
        }

    }

}
