using OtlobLap.Models;

namespace OtlobLap.ViewModels
{
	public class ProductsResultsVM
	{
		public List<Product>? Products { get; set; }
		public HashSet<Brand>? Brands { get; set; }
		public HashSet<Category>? Categories { get; set; }
	}
}
