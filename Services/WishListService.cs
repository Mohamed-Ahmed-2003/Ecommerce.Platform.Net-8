using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.Models;

namespace OtlobLap.Services
{

    public interface IWishListService
    {
        Task AddToWishListAsync(int customerId, int productId);
        Task<Wishlist> GetWishListAsync(int customerId);
        Task ClearWishListAsync(int customerId);
        Task RemoveWishListItemAsync(int wishItemId);
    }
    public class WishListService(AppDbContext context, IProductService productService
         , IProductService ProductService) : IWishListService
    {
        private readonly AppDbContext _context = context;
        private readonly IProductService _productService = productService;

        public async Task AddToWishListAsync(int customerId, int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
                throw new InvalidOperationException("The target product does not exist.");

            var currentWishlist = await GetWishListAsync(customerId);

            if (currentWishlist.Items.Find(item => item.ProductId == productId) != null)
                throw new InvalidOperationException("This product is already in your wishlist.");


            var wishlistItem = new WishlistItem
                {
                    ProductId = productId,
                    WishListId = currentWishlist.Id,
                };
                await _context.wishlistItems.AddAsync(wishlistItem);
                await _context.SaveChangesAsync();

            
        }

        public async Task<Wishlist> GetWishListAsync(int customerId)
        {
            if (customerId == null) throw new Exception("There is no customer to get his wishList ");

            var wishList = await _context.
                         WishLists
                         .Include(w =>w.Items)
                         .ThenInclude(item => item.Product)
                         .FirstOrDefaultAsync(wishList => wishList.CustomerId == customerId);

            if (wishList == null)
            {
                wishList = new Wishlist { CustomerId = customerId };
                await _context.WishLists.AddAsync(wishList);
                await _context.SaveChangesAsync();
            }

            return wishList;
        }


        public async Task RemoveWishListItemAsync(int wishItemId)
        {
            var wislistItem = await _context.wishlistItems.FindAsync(wishItemId);

            if (wislistItem == null) 
                throw new Exception("Faild to remove the wishlist item!");

             _context.wishlistItems.Remove(wislistItem);
            await _context.SaveChangesAsync();
        }

        public async Task ClearWishListAsync(int customerId)
        {
            var wishList = await GetWishListAsync(customerId);
            _context.WishLists.Remove(wishList);
            await _context.SaveChangesAsync();
        }
    }

}
