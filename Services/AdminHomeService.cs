using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.ViewModels;

namespace OtlobLap.Services
{
    public interface IAdminHomeService
    {
        Task<List<MostSellingDto>> GetBestSellingProductsAsync();
        Task<List<TopSellerDto>> GetMostSellersAsync(); // according to their revenues
        Task<List<TopSellerDto>> GetMostReliableSellersAsync(); // according to their reviews 

        Task<int> GetRolesCountAsync();
        Task<int> GetCustomersCountAsync();
        Task<int> GetSellersCountAsync();
        Task<int> GetProductsCountAsync();
        Task<int> GetCategoriesCountAsync();
        Task<int> GetOrdersCountAsync();
        Task<List<SaleDto>> GetRecentSales();
        Task<AdminDashboardVM> GetAdminInfoAsync();

    }


    public class AdminHomeService : IAdminHomeService
    {
        private readonly AppDbContext _context;

        public AdminHomeService(AppDbContext context)
        {
            _context = context;
        }




        #region small box
        public async Task<int> GetRolesCountAsync()
        {
            return await _context.Roles.CountAsync();
        }

        public async Task<int> GetCustomersCountAsync()
        {
            return await _context.Users.Where(u=>u.Role == Utility.Roles.Customer.ToString()).CountAsync();
        }

        public async Task<int> GetSellersCountAsync()
        {
            return await _context.Users.Where(u => u.Role == Utility.Roles.Seller.ToString()).CountAsync();
        }

        public async Task<int> GetProductsCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<int> GetCategoriesCountAsync()
        {
            return await _context.Categories.CountAsync();
        }

        public async Task<int> GetOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

     

        #endregion

        #region DataTables
        public async Task<List<SaleDto>> GetRecentSales()
        {
            var recentSales = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.Date)
                .Take(10) // Assuming you want to fetch the 10 most recent sales
                .SelectMany(o => o.OrderItems.Select(oi => new SaleDto
                {
                    OrderId = o.ID,
                    CustomerName = o.Customer != null ? o.Customer.Name: null,
                    ProductName = oi.Product != null ? oi.Product.Name : null,
                    Price = oi.Product != null && oi.Product.Discount != null ? Utility.CalculateDiscountedPrice(oi.Price, oi.Product.Discount.Percentage) : oi.Price,
                    Status = o.Status
                }))
                .ToListAsync();

            return recentSales;
        }
        public async Task<List<TopSellerDto>> GetMostSellersAsync()
        {
            var topSellers = await _context.TopSellerDto.FromSqlRaw("EXEC GetSellersWithHighestRevenues").ToListAsync();

            return topSellers;
        }
        public async Task<List<TopSellerDto>> GetMostReliableSellersAsync()
        {
            var reliableSellers = await _context.TopSellerDto.FromSqlRaw("EXEC GetMostReliableSellers").ToListAsync();
            return reliableSellers;
        }
        public async Task<List<MostSellingDto>> GetBestSellingProductsAsync()
        {
            var bestSellingProducts = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new MostSellingDto
                {
                    ProductName = g.First().Product.Name,
                    Price = g.First().Product.Price,
                    ProductPreviewUrl = g.First().Product.MainProductImage,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Price)
                })
                .OrderByDescending(dto => dto.UnitsSold)
                .Take(10)
                .ToListAsync();

            return bestSellingProducts;
        }

        public async Task<AdminDashboardVM> GetAdminInfoAsync()
        {

            var model = new AdminDashboardVM
            {
                TopSellerRated = GetMostReliableSellersAsync().Result,
                TopSellerRevenue = GetMostSellersAsync().Result,
                RecentSales = GetRecentSales().Result,
                TotalProducts = GetProductsCountAsync().Result ,
                TotalOrders = GetOrdersCountAsync().Result ,
                TotalCustomers = GetCustomersCountAsync().Result ,
                TotalCategories = GetCategoriesCountAsync().Result ,
                TotalRoles = GetRolesCountAsync().Result,
                TotalSellers = GetSellersCountAsync().Result ,
            };
            return model;
        }


        #endregion


    }

}
