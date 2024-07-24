using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OtlobLap.Models
{
    public class Category
    {
        // Properties
        [Key]
        public int ID { get; set; }

        [DisplayName("اسم القسم")]
        [StringLength(50)] // Adjust the maximum length as needed
        public required string Name { get; set; } 
        public  string? BannerUrl { get; set; } 

        // Navigation Properties
        public List<ProductCategory>? ProductCategories { get; set; } 
    }
}
