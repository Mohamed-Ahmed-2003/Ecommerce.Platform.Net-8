using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.Models;

namespace OtlobLap.Services
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<OrderItem> GetOrderItemByIdAsync(int orderItemId);
        Task<UpdateStatus> UpdateOrderItemQuantityAsync(int orderItemId, int newQuantity);
        Task RemoveOrderItemAsync(int orderItemId);
    }

    public class OrderItemService(AppDbContext context,IProductService productService) : IOrderItemService
    {
        private readonly AppDbContext _context = context;
        private readonly IProductService _productService = productService;

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Include(item=>item.Product)
                .Where(item => item.OrderID == orderId)
                .ToListAsync();
        }

        public async Task<OrderItem> GetOrderItemByIdAsync(int orderItemId)
        {
            return await _context.OrderItems.Include(item=>item.Product).FirstOrDefaultAsync
                (o=>o.ID == orderItemId);
        }

        public async Task<UpdateStatus> UpdateOrderItemQuantityAsync(int orderItemId, int quantityValue)
        {
            var orderItem = await _context.OrderItems.Include(o=>o.Product)
                .FirstOrDefaultAsync(o=>o.ID == orderItemId);

            if (orderItem != null && orderItem.Product != null)
            {
                int newQuantity = orderItem.Quantity + quantityValue;

                 UpdateStatus updateStatus = UpdateStatus.Available;
                if (newQuantity <= 0)
                { 
                    await RemoveOrderItemAsync(orderItemId);
                    return UpdateStatus.Zero;
                }
                else if (newQuantity > orderItem.Product.AvailableInStock )
                {
                    newQuantity = orderItem.Product.AvailableInStock;
                    updateStatus = UpdateStatus.MoreThanAvailable;

                }
                orderItem.Quantity = newQuantity;
                orderItem.Price = orderItem.Quantity * orderItem.Product.Price;
                await _context.SaveChangesAsync();

                return updateStatus;
            }
            return UpdateStatus.NULL;
        }

        public  async Task RemoveOrderItemAsync(int orderItemId)
        {
            var item = await  GetOrderItemByIdAsync (orderItemId);

            if (item != null)
            {
                _context.OrderItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

   
    }

}
