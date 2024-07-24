namespace OtlobLap.Services
{
    using Microsoft.EntityFrameworkCore;
    using OtlobLap.Data;
    using OtlobLap.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IReviewService
    {
        Task<Review> GetReviewByIdAsync(int id);
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
    }

    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _Context; // Assuming you have a DbContext named YourDbContext

        public ReviewService(AppDbContext dbContext)
        {
            _Context = dbContext;
        }

        public async Task<Review> GetReviewByIdAsync(int id)
        {
            var review = await _Context.Reviews.Include(rev=>rev.Customer).SingleOrDefaultAsync(rev=>rev.Id == id);

            if (review is null)
                throw new Exception ("Review is not found");

            return review;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _Context.Reviews.ToListAsync();
        }

        public async Task AddReviewAsync(Review review)
        {
            var existingReview = await _Context.Reviews.FirstOrDefaultAsync (r => r.ProductId == review.ProductId && r.CustomerId == review.CustomerId);

			// If a review already exists, throw an exception
			if (existingReview != null)
			{
				throw new Exception("Sorry, you can add only 1 review");
			}
            await _Context.Reviews.AddAsync(review);
			await _Context.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(Review review)
        {
            _Context.Reviews.Update(review);
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _Context.Reviews.FindAsync(id);
            if (review != null)
            {
                _Context.Reviews.Remove(review);
                await _Context.SaveChangesAsync();
            }
        }
    }

}
