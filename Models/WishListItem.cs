using OtlobLap.ViewModels;

namespace OtlobLap.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public int WishListId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public Wishlist? Wishlist{ get; set; }

    }

}
