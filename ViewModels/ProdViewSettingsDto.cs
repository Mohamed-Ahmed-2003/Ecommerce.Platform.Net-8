using OtlobLap.Configuration;
using OtlobLap.Models;

namespace OtlobLap.ViewModels
{
    public class ProdViewSettingsDto
    {
        public string? SearchKey { get; set; }
        public ResultsFilter? Filter { get; set; } 
        public ProductsTag? Tag { get; set; } 
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int Page { get; set; } = 1;
        public ViewMode Mode { get; set; } = ViewMode.Vertical;
    }

}
