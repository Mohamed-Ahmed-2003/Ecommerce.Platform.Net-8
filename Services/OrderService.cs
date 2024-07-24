using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.ViewModels;

namespace OtlobLap.Services
{
    // IService
    public interface IOrderService
    {
        Task CreateOrderAsync(Order order);
        Task<List<Order>> GetOrdersBySellerIdAsync(int sellerId);
        Task<List<Order>> GetOrdersBySellerId_StatusAsync(int sellerId,OrderStatus orderState);
        Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task<List<Order>> GetOrdersByCustomerId_StatusAsync(int customerId, OrderStatus orderState);
        Task<Order> GetOrderByIdAsync (int orderId);

        Task UpdateOrderStatusAsync(int orderId, OrderStatus orderState);

    }

    // Service
    public class OrderService(AppDbContext context ,IProductService productService) : IOrderService
    {
        private readonly AppDbContext _context = context; // Assuming you have a DbContext named ApplicationDbContext
        private readonly IProductService _productService = productService;


        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            foreach (var item in order.OrderItems)
            {
                item.Product.AvailableInStock -= item.Quantity;
                await _productService.UpdateProductAsync(item.Product);   
            }
            await _context.SaveChangesAsync();

        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders.
                              Include(p => p.Seller).
                              Include(o => o.OrderItems).
                              ThenInclude(o => o.Product)
                          .FirstOrDefaultAsync(o => o.ID == orderId) ;
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _context.
                Orders.
                Include(p => p.Seller).
                Include(o => o.OrderItems).
                ThenInclude(o=>o.Product).
                Where(o => o.CustomerId == customerId).ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByCustomerId_StatusAsync(int customerId, OrderStatus orderState)
        {
            return await _context.
               Orders.
               Include(p => p.Seller).
               Include(o => o.OrderItems).
               ThenInclude(o => o.Product).
               Where(o => o.CustomerId == customerId &&o.Status == orderState).ToListAsync();
        }

        public async Task<List<Order>> GetOrdersBySellerIdAsync(int sellerId)
        {
            return await _context.Orders
                            .Include(o=>o.Customer)
                            .Include(o => o.OrderItems)
                            .ThenInclude(o => o.Product)
                            .Where(o => o.SellerId == sellerId)
                            .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersBySellerId_StatusAsync(int sellerId, OrderStatus orderState)
        {
            return await _context.Orders
                            .Include(o => o.Customer)
                            .Include(o => o.OrderItems)
                            .ThenInclude(o => o.Product)
                            .Where(o => o.SellerId == sellerId && o.Status == orderState)
                            .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus orderState)
        {
                var order = await GetOrderByIdAsync(orderId);
            if (order == null) throw new Exception("Order is not exist");
            if (orderState == OrderStatus.Rejected|| orderState == OrderStatus.Refunded)
            {
                foreach (var item in order.OrderItems)
                {
                    item.Product.AvailableInStock += item.Quantity;
                    await _productService.UpdateProductAsync(item.Product);
                }
            }
            order.Status = orderState;
            if (order.Status == OrderStatus.Arrived)
                order.ArrivalDate = DateTime.Now;

            await _context.SaveChangesAsync();

        }
    }
}
