using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.Models;

namespace OtlobLap.Services
{
    public enum UpdateStatus
    {
            Available,
            Zero,
            MoreThanAvailable,
            NULL

    }
    public interface ICartService
    {
        Task AddToCartAsync(int customerId, int productId, int quantity);
        Task<Cart> GetCartAsync(int customerId);
        Task ClearCartAsync (int customerId);
        Task<UpdateStatus> UpdateOrderItemQuantityAsync( int 
            Id, int newQuantity);
        Task RemoveOrderItemAsync(int orderItemId);

        Task<Tuple<decimal, decimal>> GetCartPrices(int userId);
        
    }
    public class CartService(AppDbContext context, IProductService productService
         , IOrderItemService orderItemService ) : ICartService
    {
        private readonly AppDbContext _context = context;
        private readonly IProductService _productService = productService;
        private readonly IOrderItemService _orderItemService = orderItemService;

        public async Task AddToCartAsync(int customerId, int productId, int quantity)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (product != null)
            {
                var existingOrderItem = await _context.OrderItems
                    .FirstOrDefaultAsync(item => item.CustomerId == customerId
                                        && item.ProductId == productId
                                        && item.CartId != null);

                if (existingOrderItem?.Quantity + quantity > product.AvailableInStock)
                        throw new Exception("quantity is not available");
                if (existingOrderItem != null )
                {
                    await UpdateOrderItemQuantityAsync(existingOrderItem.ID, quantity);
                }
                else
                {
                    if (quantity > product.AvailableInStock)
                        throw new Exception("quantity is not available");

                    var newOrderItem = new OrderItem
                    {
                        CustomerId = customerId,
                        ProductId = productId,
                        Quantity = quantity,
                        Price = product.Price * quantity,
                    };

                    var userCart = await GetCartAsync(customerId);

                    if (userCart != null)
                    {
                        if (userCart.OrderItems == null)
                            userCart.OrderItems = new List<OrderItem>();

                        userCart.OrderItems.Add(newOrderItem);
                    }

                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<Cart> GetCartAsync(int customerId)
        {
            if (customerId == null) throw new Exception("There is no customer to get his cart ");

           var cart = await _context.
                        Carts
                        .Include(C => C.OrderItems)
                        .ThenInclude(O=>O.Product)
                        .ThenInclude(p=>p.Discount)
                        .FirstOrDefaultAsync(cart => cart.CustomerId == customerId);
           

            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

           
            return cart;
        }

        public async Task<UpdateStatus> UpdateOrderItemQuantityAsync( int orderItemId, int newQuantity)

        {
                return await _orderItemService.UpdateOrderItemQuantityAsync(orderItemId,newQuantity);
        
       

         }
        
        public async Task RemoveOrderItemAsync(int OrderItemId)
        {
             await _orderItemService.RemoveOrderItemAsync(OrderItemId);
        }

        public async Task ClearCartAsync(int customerId)
        {
            var cart = await  GetCartAsync(customerId);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }

        public async Task<Tuple<decimal, decimal>> GetCartPrices(int userId)
        {
            var (total, discountedTotal) = (0m, 0m);

            var cart = await GetCartAsync(userId);
            foreach (var item in cart.OrderItems)
            {
                total += item.Price;
                if (item.Product?.Discount != null)
                  discountedTotal += item.Price * (item.Product.Discount.Percentage / 100m);  
            }
            return new Tuple<decimal, decimal>(total, discountedTotal);
        }

    }

}
