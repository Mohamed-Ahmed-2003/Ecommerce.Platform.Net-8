using System.ComponentModel.DataAnnotations;

namespace OtlobLap.Models
{
    public class Review
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        [Range(1, 5)]
        public int Rating { get; set;}
        public required int CustomerId { get; set;}
        public int ProductId { get; set;}
        public DateTime Date {  get; set;}
        public Product? product{ get; set;}
		public ApplicationUser? Customer { get; set;}
    }
}
