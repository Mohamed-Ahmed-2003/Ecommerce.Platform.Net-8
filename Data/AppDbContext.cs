using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OtlobLap.Models;
using OtlobLap.ViewModels;
using System.Reflection.Emit;

namespace OtlobLap.Data
{
    public class AppDbContext :IdentityDbContext<ApplicationUser,IdentityRole<int>, int>
    {
     
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions):base(dbContextOptions) { 
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("Users", "Identity");
            builder.Entity<IdentityRole<int>>().ToTable("Roles", "Identity");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles", "Identity");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims", "Identity");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogin", "Identity");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaim", "Identity");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserToken", "Identity");
            builder.Entity<ProductCategory>().HasKey(pc => new { pc.ProductId, pc.CategoryId });
            builder.Entity<Discount>().HasQueryFilter(d => d.EndDate > DateTime.UtcNow && d.IsActive);
            builder.Entity<Product>().HasQueryFilter(p => !p.IsHidden);
            builder.Entity<TopSellerDto>(e => e.HasNoKey().ToView(null));
            builder.Entity<Order>()
           .HasOne(o => o.Seller)
           .WithMany(s => s.Orders)
           .HasForeignKey(o => o.SellerId)
           .OnDelete(DeleteBehavior.Restrict);
            /* 

           migrationBuilder.Sql(@"
    CREATE PROCEDURE GetMostReliableSellers
    AS
    BEGIN
                  SELECT TOP 10
                    s.Name AS SellerName,
                    s.UserImage,
                    COALESCE(AVG(r.Rating), 0) AS AvgRating,
                    COALESCE(SUM(oi.Price), 0) AS TotalRevenue,
                    COALESCE(COUNT(o.ID), 0) AS TotalSales,
                    COALESCE(COUNT(r.ID), 0) AS TotalReviews
                FROM [Identity].Users S
                LEFT JOIN Products p ON s.ID = p.SellerID
                LEFT JOIN Reviews r ON p.ID = r.ProductID
                LEFT JOIN Orders o ON s.ID = o.SellerID
                LEFT JOIN OrderItems oi ON o.ID = oi.OrderID
                WHERE s.Role = 'Seller'
                GROUP BY s.Name, s.UserImage
                ORDER BY COALESCE(AVG(r.Rating), 0) DESC;
    END;
");

migrationBuilder.Sql(@"
    CREATE PROCEDURE GetSellersWithHighestRevenues
    AS
        BEGIN
          SELECT TOP 10
        s.Name AS SellerName,
        s.UserImage,
         COALESCE(SUM(oi.Price),0) AS TotalRevenue,
         COALESCE(COUNT(o.ID),0) AS TotalSales,
         COALESCE(COUNT(r.ID),0) AS TotalReviews,
        COALESCE(AVG(r.Rating), 0) AS AvgRating
    FROM [Identity].Users s
    LEFT JOIN Orders o ON s.ID = o.SellerID
    LEFT JOIN OrderItems oi ON o.ID = oi.OrderID
    LEFT JOIN Products p ON s.ID = p.SellerID
    LEFT JOIN Reviews r ON p.ID = r.ProductID
    LEFT JOIN [Identity].UserRoles UR ON UR.UserId = S.Id
    LEFT JOIN [Identity].Roles ON Roles.Id = UR.RoleId 
    WHERE Roles.Name = 'Seller'
    GROUP BY s.Name, s.UserImage
ORDER BY TotalRevenue DESC;
    END;
");
}


         */


        }
        // not mapped dbset
        public DbSet<TopSellerDto> TopSellerDto { get; set; }
        // DbSet for each model class

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<Cart> Carts{ get; set; }
        public DbSet<WishlistItem> wishlistItems{ get; set; }
        public DbSet<Wishlist> WishLists{ get; set; }
        public DbSet<ProductImage> ProductImages{ get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Banner> Banners{ get; set; }
        public DbSet<Brand> Brands{ get; set; }

    }
}
