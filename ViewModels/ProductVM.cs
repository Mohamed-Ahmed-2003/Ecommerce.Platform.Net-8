using OtlobLap.Models;
using System.ComponentModel;

namespace OtlobLap.ViewModels
{
    public class ProductVM
    {
        public required Product Product { get; set; }
        public List<int> ? Categories { get; set; }
        [DisplayName(" صورة المنتج الرئيسية")]
        public IFormFile? MainProductImage { get; set; }

        [DisplayName("صور المنتج")]
        public ICollection<IFormFile>? ProductImages { get; set; }
    }
}
