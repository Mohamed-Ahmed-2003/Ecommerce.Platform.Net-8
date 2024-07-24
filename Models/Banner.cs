using System.ComponentModel.DataAnnotations;

namespace OtlobLap.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public required string ImageUrl { get; set; }
        public string? Description { get; set; }
        [Url]
        public string? Link { get; set; }
    }

}
