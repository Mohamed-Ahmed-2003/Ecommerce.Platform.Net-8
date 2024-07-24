using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.Services;
using Utility = OtlobLap.Data.Utility;

namespace OtlobLap.Controllers
{
    public class OrderController(IOrderService orderService , ICartService cartService,IProfileService profileService) : BaseController(profileService)
    {
        private readonly IOrderService _orderService =  orderService;
        private readonly ICartService _cartService = cartService;
        [Authorize(Utility.Permissions.Order.View)]
        public async Task<IActionResult> Index()
        {

            if (ViewBag.UserId is null )
                return RedirectToAction("Login", "Account");
            var orders = await _orderService.GetOrdersByCustomerIdAsync(ViewBag.UserId);
            return View(orders);
        }
   
    }
}
