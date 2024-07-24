using System.ComponentModel.DataAnnotations;

namespace OtlobLap.ViewModels
{
    public class BannerVM { 
        public int Id { get; set; }
        public required IFormFile ImageFile { get; set; }
        public string? Description { get; set; }
        [Url]
        public string? LinkUrl { get; set; }
    }

}
