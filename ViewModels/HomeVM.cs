using OtlobLap.Models;

namespace OtlobLap.ViewModels
{
    public class HomeVM
    {
        public List<Banner>? Banners { get; set; }
        public List<Product>? PremiumProducts { get; set; }
        public List<Category>? Categories{ get; set; }

        public  ProdViewSettingsDto? SettingsDto { get; set; }

    }
}
