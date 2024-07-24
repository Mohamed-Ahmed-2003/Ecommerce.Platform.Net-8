using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtlobLap.Migrations
{
    /// <inheritdoc />
    public partial class Sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
