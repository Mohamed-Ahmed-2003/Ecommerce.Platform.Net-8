
namespace OtlobLap.Configuration
{
    public enum ViewMode
    {
        Vertical,
        Horizontal
    }


public class ProductsViewOptions
    {
        public int ProductsPageSize { get; set; }
        public int SimilarProductsPageSize { get; set; }
        public int ViewMode { get; set; }
    }


}
