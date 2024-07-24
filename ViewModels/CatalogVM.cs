using System.ComponentModel.DataAnnotations;

namespace OtlobLap.ViewModels
{
    public class CatalogVM
    {
        public int Id { get; set; }
        public  IFormFile? ImageFile { get; set; }
        public required string Name { get; set; }

    }
}
