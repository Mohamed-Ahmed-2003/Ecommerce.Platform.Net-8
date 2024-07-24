using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.Services;

namespace OtlobLap.Controllers
{
    public class WishlistController(IWishListService wishlistService, UserManager<ApplicationUser> userManager, IProfileService profileService) : BaseController(profileService)
    {
        private readonly IWishListService _wishlistService = wishlistService;

        [Authorize(Utility.Permissions.WishList.View)]

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            if (ViewBag.UserId is null)
                return RedirectToAction("Login", "Account");

            var wishlist = await _wishlistService.GetWishListAsync(ViewBag.UserId);

            return View(wishlist.Items);
        }
        [Authorize(Utility.Permissions.WishList.Add)]

        public async Task<IActionResult> AddToWishlist(int productId)
        {

            if (ViewBag.UserId is null)
                return RedirectToAction("Login", "Account");

            try
            {
                await _wishlistService.AddToWishListAsync(ViewBag.UserId, productId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize(Utility.Permissions.WishList.Remove)]

        public async Task<IActionResult> RemoveFromWishlist(int Id)
        {
            try
            {
                await _wishlistService.RemoveWishListItemAsync(Id);
                 return Ok();
            }
            catch 
            {
                return BadRequest();
            }
;
        }
        [Authorize(Utility.Permissions.WishList.Remove)]

        [HttpPost]
        public async Task<IActionResult> ClearWishlist()
        {
            if (ViewBag.UserId is null)
                return RedirectToAction("Login", "Account");
            try
            {
                await _wishlistService.ClearWishListAsync(ViewBag.UserId);
                TempData["SuccessMessage"] = "Wishlist cleared successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to clear wishlist. {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
