using OtlobLap.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace OtlobLap.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public required int CustomerId { get; set; }

        public ApplicationUser? Customer{ get; set; }
        public List<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }

}
