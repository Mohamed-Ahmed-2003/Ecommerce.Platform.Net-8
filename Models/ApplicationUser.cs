using Microsoft.AspNetCore.Identity;
using OtlobLap.Data;

namespace OtlobLap.Models
{

    public class ApplicationUser : IdentityUser<int>
    {
        public required string Name { get; set; }
        public byte[]? UserImage { get; set; }
        public required string Role { get; set; }

        public List<Product>? Products { get; set; }
        public List<Order>? Orders { get; set; }

        public Cart? Cart { get; set; }

    }
}






