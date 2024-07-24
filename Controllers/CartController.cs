using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.Services;

namespace OtlobLap.Controllers
{
    public class CartController (ICartService cartService,
                 UserManager<ApplicationUser> userManager,
                IProfileService profileService) : BaseController(profileService)
    {
        private readonly ICartService _cartService = cartService;
        public async Task<IActionResult> Index()
        {
            int.TryParse(Request.Cookies["UserId"], out var userID);
            if (userID == 0) return RedirectToAction("Login", "Account");

            var Cart = await _cartService.GetCartAsync(userID);
            return View(Cart);
        }
        [Authorize(Utility.Permissions.Cart.Add)]
        public async Task<IActionResult> AddToCart(int productId,int quan = 1)
        {

            if (ViewBag.UserId is null)
                return  Unauthorized();

			try
            {
                await _cartService.AddToCartAsync(ViewBag.UserId, productId, quan);
                return Json (new {success = true});
            }catch 
            {
                return StatusCode (StatusCodes.Status500InternalServerError, new { error = "An error occurred while processing your request." });
            }
        }   
        [Authorize(Utility.Permissions.Cart.Remove)]
        public async Task<IActionResult> RemoveFromCart(int OrderItemId)
        {
            try
            {
                await _cartService.RemoveOrderItemAsync(OrderItemId);

                if (ViewBag.UserId is null)
                    return RedirectToAction("Login", "Account");

                var prices = await _cartService.GetCartPrices(ViewBag.UserId);
                return Ok(new { total = prices.Item1.ToString("C"),  discountedTotal = prices.Item2 > 0 ? (prices.Item1 - prices.Item2).ToString("C") : "" });

            }
            catch 
            {
                return BadRequest();
            }

        }
        [Authorize(Utility.Permissions.Cart.Add)]
        public async Task<IActionResult> UpdateQuantity (int orderItemId, int quan)
        {

            try
            {
                var updateState = await _cartService.UpdateOrderItemQuantityAsync(orderItemId, quan);

                if (updateState == UpdateStatus.Zero)
                {
                    throw new Exception("تم حذف العنصر المطلوب");
                }
                else if (updateState == UpdateStatus.MoreThanAvailable)
                {
                    throw new Exception("لا يمكن تخطي الحد الاقصي للعدد المتاح للمنتج");
                }

                if (ViewBag.UserId is null)
                    return Unauthorized();
                var prices = await _cartService.GetCartPrices(ViewBag.UserId);

                return Ok(new { mesg = "تم تحديث كمية المنتج",
                     total = prices.Item1.ToString("C")
                   , discountedTotal = prices.Item2 > 0 ?(prices.Item1 - prices.Item2).ToString("C") : "",
                

                }) ;
            }
            catch (Exception ex)
            {
                return BadRequest(new {mesg = ex.Message});
            }
        }
 

    }
}

