
using OtlobLap.Models;

namespace OtlobLap.ViewModels
{
    public class CategoryProductsVM
    {
     
        public required Category Category { get; set; }
        public required ProductsResultsVM Results { get; set; }
        public ProdViewSettingsDto? SettingsDto { get; set; }
    }
}
