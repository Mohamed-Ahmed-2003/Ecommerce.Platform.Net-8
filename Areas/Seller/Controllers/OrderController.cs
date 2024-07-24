using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OtlobLap.Controllers;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.Services;

namespace OtlobLap.Areas.Seller.Controllers
{
    [Area(nameof(Utility.Roles.Seller))]
    [Authorize(Roles = nameof(Utility.Roles.Seller))]
    public class OrderController(IOrderService orderService, IProfileService profileService) : BaseController(profileService)
    {
        private readonly IOrderService _orderService = orderService;
        [Authorize(Utility.Permissions.Order.View)]
        public async Task<IActionResult> Index(OrderStatus orderStatus)
        {
            int.TryParse(Request.Cookies["UserId"], out var userID);
            if (userID == 0) return RedirectToAction("Login", "Account");
            var orders = await _orderService.GetOrdersBySellerId_StatusAsync(userID, orderStatus);

            ViewBag.view = Utility.GetTranslation(orderStatus.ToString());

            return View(orders);
        }
        [Authorize(Utility.Permissions.Order.Add)]

        public async Task<IActionResult> ExecuteOrder ([Bind("Status,ID")] Order order)
        {
            await _orderService.UpdateOrderStatusAsync(order.ID,order.Status);
                return RedirectToAction("Index");
        } 
        
        public  IActionResult GetOrderStatusOptions ()
        {
            var options = Enum.GetValues<OrderStatus>().Cast<OrderStatus>().Select(e=> new {value = (int)e ,text = e.ToString() } )
                .ToList();
            return new JsonResult(options);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.Controller = Utility.GetTranslation("Orders");
        }
    }
}
