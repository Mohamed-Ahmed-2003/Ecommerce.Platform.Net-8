namespace OtlobLap.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OtlobLap.Models;
    using OtlobLap.Services;
    using System.Threading.Tasks;
    using System;
    using Microsoft.AspNetCore.Authorization;
    using OtlobLap.Data;

    public class ReviewController(IReviewService reviewService , IProfileService profileService) : BaseController(profileService)
    {
        private readonly IReviewService _reviewService = reviewService;

        [Authorize(Utility.Permissions.Review.View)]
        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                return PartialView("_ReviewListPartial", reviews);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Utility.Permissions.Review.Add)]

        [HttpPost]
        public async Task<IActionResult> AddReview(Review review)
        {
            try
            {

                review.Date = DateTime.UtcNow;
                if (!ModelState.IsValid) return BadRequest();
                await _reviewService.AddReviewAsync(review);

				var addedReview = await _reviewService.GetReviewByIdAsync(review.Id);

				return PartialView("_ReviewBox", addedReview);
            }
            catch (Exception ex)
            {
                // Log the exception
                return BadRequest( ex.Message);
            }
        }
        [Authorize(Utility.Permissions.Review.Add)]
        [HttpPost]
        public async Task<IActionResult> UpdateReview(Review review)
        {
            try
            {
				review.Date = DateTime.UtcNow;
				if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _reviewService.UpdateReviewAsync(review);
                var updatedReview = await _reviewService.GetReviewByIdAsync(review.Id);
				return PartialView("_ReviewBox", updatedReview);
			}
			catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Utility.Permissions.Review.Remove)]

        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, ex.Message);
            }
        }
    }

}
