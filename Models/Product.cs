namespace OtlobLap.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum ProductsTag
    {
        NewArrivals,
        BestSellers,
        Recommended,
        TopOffers
    }
	public enum ResultsFilter
	{
		PriceOrderAsc,
		PriceOrderDesc,
		DateAdded,
		AverageRating,
	}


	public class Product
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        [Display(Name = "ProductName")]
        public required string Name { get; set; }


        [Display(Name = "ShortDescription")]
        public string? ShortDescription { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "AvailableInStock")]
        public int AvailableInStock { get; set; }

        [Display(Name = "SellerID")]
        public int? SellerID { get; set; }
        public int? BrandId { get; set;}

        [Display(Name = "Price")]
        public decimal Price { get; set; }
        public bool Premium { get; set; }
        public bool IsHidden {  get; set; }
        public DateTime AddedDate { get; set; }
        public float AverageRating
        {
            get
            {
                if (Reviews is null || Reviews.Count == 0) return 0;
                return (float)Reviews.Average(rev => rev.Rating);
            }
        }

        [Display(Name = "MainProductImage")]
        public string? MainProductImage { get; set; }

        [Display(Name = "ProductImages")]
        public ICollection<ProductImage>? ProductImages { get; set; }

        public ApplicationUser? Seller { get; set; }
        public Discount? Discount { get; set; }


        [Display(Name = "Brand")]
        [ForeignKey(nameof(BrandId))]
        public Brand? Brand { get; set; }

        [Display(Name = "ProductCategories")]
        public ICollection<ProductCategory>? ProductCategories { get; set; }
        public List<Review>? Reviews { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }
}
