namespace OtlobLap.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? BrandImage { get; set; }

        public List<Product>? Products { get; set; }

    }
}
